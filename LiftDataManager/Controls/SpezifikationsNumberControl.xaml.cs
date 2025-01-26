using System.Text.RegularExpressions;
using System.Windows.Input;

namespace LiftDataManager.Controls;

public sealed partial class SpezifikationsNumberControl : UserControl
{
    [GeneratedRegex(@"^[a-zA-Z0-9ƒ÷‹‰ˆ¸ﬂ _()-]*$")]
    private static partial Regex CustomFileNameRegex();
    public Regex CustomFileName { get; set; } = CustomFileNameRegex();
    public List<SpezifikationTyp>? SpezifikationTyps { get; set; }
    public int[] Years { get; set; } = Enumerable.Range(10, 20).ToArray();
    public string[] Months { get; } = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"];

    public SpezifikationsNumberControl()
    {
        InitializeComponent();
        Loaded += OnSpezifikationsNumberControlLoaded;
    }

    private void OnSpezifikationsNumberControlLoaded(object sender, RoutedEventArgs e)
    {
        SpezifikationTyps = [.. SpezifikationTyp.List];
        if (!RequestEnabled)
        {
            SpezifikationTyps.Remove(SpezifikationTyp.Request);
            SpezifikationTyps.Remove(SpezifikationTyp.MailRequest);
        }
        if (IsVaultDisabled)
        {
            SpezifikationTyps.Remove(SpezifikationTyp.Offer);
            SpezifikationTyps.Remove(SpezifikationTyp.Planning);
        }
        cmb_SpezifikationTyp.ItemsSource = SpezifikationTyps;

        if (IsVaultDisabled || 
            !IsOrderRelated ||
            string.IsNullOrWhiteSpace(SpezifikationName))
        {
            return;
        }

        var speziNameArray = SpezifikationName.Split("-");
        switch (SpezifikationTyp)
        {
            case var s when s.Equals(SpezifikationTyp.Order):
                NumberBoxText = SpezifikationName.ConvertToInt();
                break;
            case var s when s.Equals(SpezifikationTyp.Offer):
                if (speziNameArray is not null && speziNameArray.Length == 3)
                {
                    cmb_Year.SelectedItem = speziNameArray[0].ConvertToInt();
                    cmb_Month.SelectedItem = speziNameArray[1];
                    NumberBoxText = speziNameArray[2].ConvertToInt();
                }
                break;
            case var s when s.Equals(SpezifikationTyp.Planning):
                if (speziNameArray is not null && speziNameArray.Length == 3)
                {
                    cmb_Year.SelectedItem = speziNameArray[1].ConvertToInt();
                    NumberBoxText = speziNameArray[2].ConvertToInt();
                }
                break;
        }
    }

    public bool RequestEnabled { get; set; }

    private int selectedYear;
    public int SelectedYear
    {
        get => selectedYear;
        set
        {
            selectedYear = value;
            SetSpezifikationName();
        }
    }

    private string? selectedMonth;
    public string? SelectedMonth
    {
        get => selectedMonth;
        set
        {
            selectedMonth = value;
            SetSpezifikationName();
        }
    }

    public SpezifikationTyp SpezifikationTyp
    {
        get => (SpezifikationTyp)GetValue(SpezifikationTypProperty);
        set
        {
            SetValue(SpezifikationTypProperty, value);
            SetControlStyle(value);
        }
    }

    public static readonly DependencyProperty SpezifikationTypProperty =
        DependencyProperty.Register(nameof(SpezifikationTyp), typeof(SpezifikationTyp), typeof(SpezifikationsNumberControl), new PropertyMetadata(SpezifikationTyp.Order));

    public bool IsVaultDisabled
    {
        get => (bool)GetValue(IsVaultDisabledProperty);
        set => SetValue(IsVaultDisabledProperty, value);
    }

    public static readonly DependencyProperty IsVaultDisabledProperty =
        DependencyProperty.Register(nameof(IsVaultDisabled), typeof(bool), typeof(SpezifikationsNumberControl), new PropertyMetadata(false));

    public int? NumberBoxText
    {
        get => (int?)GetValue(NumberBoxTextProperty);
        set
        {
            SetValue(NumberBoxTextProperty, value);
            SetSpezifikationName();
        }
    }

    public static readonly DependencyProperty NumberBoxTextProperty =
        DependencyProperty.Register(nameof(NumberBoxText), typeof(int?), typeof(SpezifikationsNumberControl), new PropertyMetadata(null));

    public string SpezifikationName
    {
        get => (string)GetValue(SpezifikationNameProperty);
        set
        {
            SetValue(SpezifikationNameProperty, value);
            IsValid = CheckSpezifikationNameIsValid(value);
            if (string.IsNullOrWhiteSpace(value) && NumberBoxText is not null)
            {
                NumberBoxText = null;
            }
        }
    }

    public static readonly DependencyProperty SpezifikationNameProperty =
        DependencyProperty.Register(nameof(SpezifikationName), typeof(string), typeof(SpezifikationsNumberControl), new PropertyMetadata(string.Empty));

    public string FilePickerText
    {
        get { return (string)GetValue(FilePickerTextProperty); }
        set { SetValue(FilePickerTextProperty, value); }
    }

    public static readonly DependencyProperty FilePickerTextProperty =
        DependencyProperty.Register(nameof(FilePickerText), typeof(string), typeof(SpezifikationsNumberControl), new PropertyMetadata(string.Empty));

    public bool IsValid
    {
        get => (bool)GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    public static readonly DependencyProperty IsValidProperty =
        DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(SpezifikationsNumberControl), new PropertyMetadata(false));

    public bool IsOrderRelated
    {
        get => (bool)GetValue(IsOrderRelatedProperty);
        set => SetValue(IsOrderRelatedProperty, value);
    }

    public static readonly DependencyProperty IsOrderRelatedProperty =
        DependencyProperty.Register(nameof(IsOrderRelated), typeof(bool), typeof(SpezifikationsNumberControl), new PropertyMetadata(false));

    public ICommand LoadCommand
    {
        get => (ICommand)GetValue(LoadCommandProperty);
        set => SetValue(LoadCommandProperty, value);
    }

    public static readonly DependencyProperty LoadCommandProperty =
        DependencyProperty.Register(nameof(LoadCommand), typeof(ICommand), typeof(SpezifikationsNumberControl), new PropertyMetadata(null));

    public ICommand PickFilePath
    {
        get => (ICommand)GetValue(PickFilePathCommandProperty);
        set => SetValue(PickFilePathCommandProperty, value);
    }

    public static readonly DependencyProperty PickFilePathCommandProperty =
        DependencyProperty.Register(nameof(PickFilePath), typeof(ICommand), typeof(SpezifikationsNumberControl), new PropertyMetadata(null));

    private void SetControlStyle(SpezifikationTyp value)
    {
        if (IsVaultDisabled)
        {
            cmb_SpezifikationTyp.Visibility = Visibility.Collapsed;
            tbx_Numberbox.Visibility = Visibility.Collapsed;
            cmb_Year.Visibility = Visibility.Collapsed;
            cmb_Month.Visibility = Visibility.Collapsed;
            btn_Request.Visibility = Visibility.Visible;
            tbx_CustomNumberbox.Visibility = Visibility.Visible;
            return;
        }
        NumberBoxText = null;
        cmb_SpezifikationTyp.Visibility = Visibility.Visible;
        tbx_CustomNumberbox.Visibility = Visibility.Collapsed;
        switch (value)
        {
            case var s when s.Equals(SpezifikationTyp.Order):
                tbx_Numberbox.Visibility = Visibility.Visible;
                tbx_Numberbox.MaxLength = 7;
                tbx_Numberbox.MinWidth = 125;
                tbx_Numberbox.PlaceholderText = "Auftragsnummer";
                cmb_Year.Visibility = Visibility.Collapsed;
                cmb_Month.Visibility = Visibility.Collapsed;
                btn_Request.Visibility = Visibility.Collapsed;
                break;
            case var s when s.Equals(SpezifikationTyp.Offer):
                tbx_Numberbox.Visibility = Visibility.Visible;
                tbx_Numberbox.MaxLength = 4;
                tbx_Numberbox.MinWidth = 64;
                tbx_Numberbox.PlaceholderText = "0000";
                cmb_Year.Visibility = Visibility.Visible;
                cmb_Month.Visibility = Visibility.Visible;
                btn_Request.Visibility = Visibility.Collapsed;
                break;
            case var s when s.Equals(SpezifikationTyp.Planning):
                tbx_Numberbox.Visibility = Visibility.Visible;
                tbx_Numberbox.MaxLength = 4;
                tbx_Numberbox.MinWidth = 64;
                tbx_Numberbox.PlaceholderText = "0000";
                cmb_Year.Visibility = Visibility.Visible;
                cmb_Month.Visibility = Visibility.Collapsed;
                btn_Request.Visibility = Visibility.Collapsed;
                break;
            case var s when s.Equals(SpezifikationTyp.Request) || s.Equals(SpezifikationTyp.MailRequest):
                FilePickerText = SpezifikationTyp == SpezifikationTyp.Request ? "Anfrageformular (pdf)" : "Mailanfrage (gespeicherte e-mail)";
                btn_Request.Width = SpezifikationTyp == SpezifikationTyp.Request ? 340 : 369;
                tbx_Numberbox.Visibility = Visibility.Collapsed;
                cmb_Year.Visibility = Visibility.Collapsed;
                cmb_Month.Visibility = Visibility.Collapsed;
                btn_Request.Visibility = Visibility.Visible;
                break;
        }
    }

    private void SetSpezifikationName()
    {
        if (IsOrderRelated ||
           SpezifikationTyp is null ||
           IsVaultDisabled)
        {
            return;
        }

        SpezifikationName = SpezifikationTyp switch
        {
            var s when s.Equals(SpezifikationTyp.Order) => $"{NumberBoxText}",
            var s when s.Equals(SpezifikationTyp.Offer) => $"{SelectedYear}-{SelectedMonth}-{NumberBoxText:0000}",
            var s when s.Equals(SpezifikationTyp.Planning) => $"VP-{SelectedYear}-{NumberBoxText:0000}",
            _ => string.Empty,
        };
    }

    private bool CheckSpezifikationNameIsValid(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        if (IsVaultDisabled)
        {
            return true;
        }
        return (value.Length >= 6 && SpezifikationTyp.Equals(SpezifikationTyp.Order)) ||
               (value.Length == 10 && SpezifikationTyp.Equals(SpezifikationTyp.Offer)) ||
               (value.Length == 10 && SpezifikationTyp.Equals(SpezifikationTyp.Planning)) ||
               (!string.IsNullOrWhiteSpace(value) && SpezifikationTyp.Equals(SpezifikationTyp.Request)) ||
               (!string.IsNullOrWhiteSpace(value) && SpezifikationTyp.Equals(SpezifikationTyp.MailRequest));
    }
}
