using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LiftDataManager.Controls;

public sealed partial class ParameterComboBox : UserControl
{
    public ParameterComboBox()
    {
        InitializeComponent();

        Loaded += OnLoadParameterComboBoxControl;
        Unloaded += OnUnLoadParameterComboBoxControl;
    }


    private void OnLoadParameterComboBoxControl(object sender, RoutedEventArgs e)
    {
        if (LiftParameter is not null)
        {
            LiftParameter.ErrorsChanged += OnErrorsChanged;
            LiftParameter.DropDownList.CollectionChanged += DropDownList_CollectionChanged;
        }
        SetBorderHeight();
    }

    private void OnUnLoadParameterComboBoxControl(object sender, RoutedEventArgs e)
    {
        if (LiftParameter is not null)
        {
            LiftParameter.ErrorsChanged -= OnErrorsChanged;
            LiftParameter.DropDownList.CollectionChanged -= DropDownList_CollectionChanged;
        }
    }

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        if (LiftParameter is not null)
        {
            SetParameterState(sender as Parameter);
        }
    }

    private void DropDownList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (sender is ObservableCollection<SelectionValue> dropDownList)
        {
            if (dropDownList.Count == 0)
            {
                return;
            }
            if (LiftParameter is null || string.IsNullOrEmpty(LiftParameter.Value))
            {
                return;
            }
            var selectedDropDownListValue = LiftParameterHelper.GetDropDownListValue(LiftParameter.DropDownList, LiftParameter.Value);
            if (selectedDropDownListValue is null)
            {
                return;
            }
            if (dropDownList.Contains(selectedDropDownListValue))
            {
                cmb_Liftparameter.SelectedItem = LiftParameterHelper.GetDropDownListValue(LiftParameter.DropDownList, LiftParameter.Value);
            }
        }
    }

    public bool IsControlActive
    {
        get => (bool)GetValue(IsControlActiveProperty);
        set => SetValue(IsControlActiveProperty, value);
    }

    public static readonly DependencyProperty IsControlActiveProperty =
        DependencyProperty.Register(nameof(IsControlActive), typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(true));

    public Parameter? LiftParameter
    {
        get
        {
            var liftparameter = GetValue(LiftParameterProperty) as Parameter;
            SetParameterState(liftparameter);
            return liftparameter;
        }
        set => SetValue(LiftParameterProperty, value);
    }

    public static readonly DependencyProperty LiftParameterProperty = DependencyProperty.Register(nameof(LiftParameter), typeof(Parameter), typeof(ParameterComboBox), new PropertyMetadata(null));

    private void SetParameterState(Parameter? liftParameter)
    {

        ErrorGlyph = string.Empty;
        ErrorType = string.Empty;

        if (liftParameter is null || !liftParameter.HasErrors)
            return;

        if (liftParameter.parameterErrors.TryGetValue("Value", out List<ParameterStateInfo>? errorList))
        {
            if (errorList is null || errorList.Count == 0)
            {
                return;
            }

            var error = errorList.OrderByDescending(p => p.Severity).FirstOrDefault();

            if (error is not null)
            {
                ErrorGlyph = error.Severity switch
                {
                    ErrorLevel.Informational => "\ue946",
                    ErrorLevel.Warning => "\ue7ba",
                    ErrorLevel.Error => "\ue730",
                    _ => string.Empty,
                };
                ErrorType = error.Severity.ToString();
            }
        }
    }

    public bool ShowDefaultHeader
    {
        get => (bool)GetValue(ShowDefaultHeaderProperty);
        set => SetValue(ShowDefaultHeaderProperty, value);
    }

    public static readonly DependencyProperty ShowDefaultHeaderProperty =
        DependencyProperty.Register(nameof(ShowDefaultHeader), typeof(bool), typeof(ParameterComboBox), new PropertyMetadata(false));

    public string Header
    {
        get => ShowDefaultHeader ? LiftParameter?.DisplayName! : (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public string ErrorGlyph
    {
        get { return (string)GetValue(ErrorGlyphProperty); }
        set { SetValue(ErrorGlyphProperty, value); }
    }

    public static readonly DependencyProperty ErrorGlyphProperty =
        DependencyProperty.Register(nameof(ErrorGlyph), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public string ErrorType
    {
        get { return (string)GetValue(ErrorTypeProperty); }
        set { SetValue(ErrorTypeProperty, value); }
    }

    public static readonly DependencyProperty ErrorTypeProperty =
        DependencyProperty.Register(nameof(ErrorType), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    public double BorderHeight
    {
        get { return (double)GetValue(BorderHeightProperty); }
        set { SetValue(BorderHeightProperty, value); }
    }

    public static readonly DependencyProperty BorderHeightProperty =
        DependencyProperty.Register(nameof(BorderHeight), typeof(double), typeof(ParameterComboBox), new PropertyMetadata(default));

    public string HighlightAction
    {
        get => GetHighlightAction();
        set { SetValue(HighlightActionProperty, value); }
    }

    public static readonly DependencyProperty HighlightActionProperty =
        DependencyProperty.Register(nameof(HighlightAction), typeof(string), typeof(ParameterComboBox), new PropertyMetadata(string.Empty));

    private void HighlightParameter_Click(object sender, RoutedEventArgs e)
    {
        if (LiftParameter is not null)
        {
            LiftParameter.IsKey = !LiftParameter.IsKey;
            HighlightAction = GetHighlightAction();
        }
    }

    private void SetBorderHeight()
    {
        var controlHeight = cmb_Liftparameter.ActualHeight;
        if (string.IsNullOrEmpty(Header))
        {
            if (controlHeight > 8d)
            {
                BorderHeight = controlHeight - 8d;
            }
        }
        else
        {
            if (controlHeight > 24d)
            {
                BorderHeight = controlHeight - 24d;
            }
        }
    }

    private string GetHighlightAction() => LiftParameter!.IsKey ? "Remove highlight" : "Highlight Parameter";

    private void NavigateToHighlightParameters_Click(object sender, RoutedEventArgs e)
    {
        LiftParameterNavigationHelper.NavigateToHighlightParameters();
    }

    private void NavigateToParameterDetails_Click(object sender, RoutedEventArgs e)
    {
        LiftParameterNavigationHelper.NavigateToParameterDetails(LiftParameter?.Name);
    }

    private void SetLiftParameterValue(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        if (!IsControlActive && LiftParameter is not null)
        {
            if (!string.IsNullOrWhiteSpace(LiftParameter.Value))
            {
                LiftParameter.Value = string.Empty;
                LiftParameter.DropDownListValue = null;
            }
        }
    }
}