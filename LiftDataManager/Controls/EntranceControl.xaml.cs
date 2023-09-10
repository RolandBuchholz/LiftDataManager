using Cogs.Collections;
using CommunityToolkit.WinUI;

namespace LiftDataManager.Controls;

public sealed partial class EntranceControl : UserControl
{
    private string? _selectedEntrance;

    public EntranceControl()
    {
        InitializeComponent();
        Loaded += EntranceControl_Loaded;
    }

    private void EntranceControl_Loaded(object sender, RoutedEventArgs e)
    {
        SetBorderHauptZugang();
    }

    public ObservableDictionary<string, Parameter> ItemSource
    {
        get => (ObservableDictionary<string, Parameter>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register("ItemSource", typeof(ObservableDictionary<string, Parameter>), typeof(EntranceControl), new PropertyMetadata(null));

    public string DisplayNameHauptzugang
    {
        get => (string)GetValue(DisplayNameHauptzugangProperty);
        set => SetValue(DisplayNameHauptzugangProperty, value);
    }

    public static readonly DependencyProperty DisplayNameHauptzugangProperty =
        DependencyProperty.Register("DisplayNameHauptzugang", typeof(string), typeof(CommandBar), new PropertyMetadata(string.Empty));

    private void TextCommandBarFlyout_Opening(object sender, object e)
    {
        var entranceFlyout = sender as TextCommandBarFlyout;

        if (entranceFlyout is not null && entranceFlyout.Target is TextBox)
        {
            _selectedEntrance = entranceFlyout.Target.Name;
            var highLightParameter = new AppBarButton() { Icon = new SymbolIcon(Symbol.Highlight), Label = "Highlihght Parameter" };
            highLightParameter.Click += HighLightParameter_Click;
            var highLightParameterToolTip = new ToolTip { Content = "hinzufügen/entfernen Parameterhighlighting" };
            ToolTipService.SetToolTip(highLightParameter, highLightParameterToolTip);
            var goToParameterDetails = new AppBarButton() { Icon = new SymbolIcon(Symbol.PreviewLink), Label = "Show Parameterdetails" };
            goToParameterDetails.Click += NavigateToParameterDetails_Click;
            var setParameterDetailToolTip = new ToolTip { Content = "Show Parameterdetails" };
            ToolTipService.SetToolTip(goToParameterDetails, setParameterDetailToolTip);
            if (!_selectedEntrance.StartsWith("txtBox_Etagenhoehe"))
            {
                var setMainEntrance = new AppBarButton() { Icon = new SymbolIcon(Symbol.NewWindow), Label = "Set MainEntrance" };
                var separator = new AppBarSeparator();
                setMainEntrance.Click += SetMainEntrance_Click;
                var setMainEntranceToolTip = new ToolTip { Content = "Hauptzugansstelle setzen" };
                ToolTipService.SetToolTip(setMainEntrance, setMainEntranceToolTip);
                entranceFlyout.PrimaryCommands.Add(setMainEntrance);
                entranceFlyout.PrimaryCommands.Add(separator);
            }
            entranceFlyout.PrimaryCommands.Add(highLightParameter);
            entranceFlyout.PrimaryCommands.Add(goToParameterDetails);
        }
    }

    private void SetMainEntrance_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_selectedEntrance))
            return;
        if (_selectedEntrance.StartsWith("txtBox_"))
        {
            ItemSource["var_Haupthaltestelle"].Value = _selectedEntrance.Replace("txtBox_", "ZG_");
        }
        SetBorderHauptZugang();
    }

    private void Delete_Hauptzugang_Click(object sender, RoutedEventArgs e)
    {
        if (ItemSource is null)
            return;
        ItemSource["var_Haupthaltestelle"].Value = "NV";
        DisplayNameHauptzugang = "Kein Hauptzugang gewählt";
        SetBorderHauptZugang();
    }

    private void HighLightParameter_Click(object sender, RoutedEventArgs e)
    {
        if (ItemSource is null)
            return;
        var entrance = _selectedEntrance;
        if (entrance is not null && entrance.StartsWith("txtBox_"))
        {
            var entranceName = entrance.Contains("Etagenhoehe") ? entrance.Replace("txtBox", "var") : entrance.Replace("txtBox_", "var_Zugang");
            ItemSource[entranceName].IsKey = !ItemSource[entranceName].IsKey;
        }
    }

    private void NavigateToParameterDetails_Click(object sender, RoutedEventArgs e)
    {
        if (ItemSource is null)
            return;
        var entrance = _selectedEntrance;
        if (entrance is not null && entrance.StartsWith("txtBox_"))
        {
            var entranceName = entrance.Contains("Etagenhoehe") ? entrance.Replace("txtBox", "var") : entrance.Replace("txtBox_", "var_Zugang");

            var nav = App.GetService<INavigationService>();
            nav.NavigateTo("LiftDataManager.ViewModels.DatenansichtDetailViewModel", ItemSource[entranceName].Name);
        }
    }

    private void SetBorderHauptZugang()
    {
        if (ItemSource is null || string.IsNullOrWhiteSpace(ItemSource["var_Haupthaltestelle"].Value))
            return;

        var allEntranceBoxes = grd_Entrance.FindChildren().Where(f => f.Name.StartsWith("txtBox_"));

        foreach (var entranceBox in allEntranceBoxes)
        {
            if (entranceBox is TextBox box)
            {
                box.BorderThickness = new Thickness(0);
            }
        }

        if (!string.IsNullOrWhiteSpace(ItemSource["var_Haupthaltestelle"].Value) || ItemSource["var_Haupthaltestelle"].Value != "NV")
        {
            if (ItemSource["var_Haupthaltestelle"].Value!.StartsWith("ZG_"))
            {
                DisplayNameHauptzugang = ItemSource["var_Haupthaltestelle"].Value!.Replace("ZG_", "");
            }
            else
            {
                DisplayNameHauptzugang = "Kein Hauptzugang gewählt";
            }
        }
        else
        {
            DisplayNameHauptzugang = "Kein Hauptzugang gewählt";
        }

        if (!string.IsNullOrWhiteSpace(DisplayNameHauptzugang) && DisplayNameHauptzugang != "Kein Hauptzugang gewählt")
        {
            var borderBox = (TextBox)FindName("txtBox_" + DisplayNameHauptzugang);
            borderBox.BorderThickness = new Thickness(3);
            if (Application.Current.Resources.ThemeDictionaries.TryGetValue("SystemAccentColor", out object? accentColor))
            {
                if (accentColor is not null)
                {
                    borderBox.BorderBrush = new SolidColorBrush((Color)accentColor);
                }
            }
        }
    }
}