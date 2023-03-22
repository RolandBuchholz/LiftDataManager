using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LiftDataManager.Controls;

public sealed partial class ParameterDetailControl : UserControl
{
    public ObservableCollection<ParameterStateInfo> ErrorsList { get; set; }

    public ParameterDetailControl()
    {
        InitializeComponent();
        ErrorsList ??= new();
    }

    public Parameter? ListDetailsMenuItem
    {
        get 
        {
            var liftparameter = GetValue(ListDetailsMenuItemProperty) as Parameter;
            SetParameterState(liftparameter);
            return liftparameter;
        } 
        set => SetValue(ListDetailsMenuItemProperty, value);
    }
    public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(Parameter), typeof(ParameterDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

    public ICommand SaveCommand
    {
        get => (ICommand)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public static readonly DependencyProperty SaveCommandProperty =
        DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(ParameterDetailControl), new PropertyMetadata(null));


    private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as ParameterDetailControl;
        control?.ForegroundElement.ChangeView(0, 0, 1);
    }

    private readonly SolidColorBrush errorcolorBrush = new(Colors.IndianRed);
    private readonly SolidColorBrush warningcolorBrush = new(Colors.Orange);
    private readonly SolidColorBrush infocolorBrush = new(Colors.Gray);

    public SolidColorBrush ParameterInfoForeground
    {
        get => (SolidColorBrush)GetValue(ParameterInfoForegroundProperty);
        set => SetValue(ParameterInfoForegroundProperty, value);
    }

    public static readonly DependencyProperty ParameterInfoForegroundProperty =
        DependencyProperty.Register("ParameterInfoForeground", typeof(SolidColorBrush), typeof(ParameterComboBox), new PropertyMetadata(null));

    private void SetParameterState(Parameter? liftParameter)
    {
        ParameterInfoForeground = infocolorBrush;
        ErrorsList.Clear();

        if (liftParameter is null) return;

        if (!liftParameter.HasErrors)
        {
            ErrorsList.Add(new ParameterStateInfo(liftParameter.Name!, liftParameter.DisplayName!, true)
            {
                Severity = ParameterStateInfo.ErrorLevel.Valid, ErrorMessage ="Keine Information, Warnungen oder Fehler vorhanden"
            });
        }

        if (liftParameter.parameterErrors.TryGetValue("Value", out List<ParameterStateInfo>? errorList))
        {
            if (errorList is null) return;
            if (!errorList.Any()) return;

            var sortedErrorList = errorList.OrderByDescending(p => p.Severity);
            var error = sortedErrorList.FirstOrDefault();

            if (error is not null)
            {
                ParameterInfoForeground = error.Severity switch
                {
                    ParameterStateInfo.ErrorLevel.Informational => infocolorBrush,
                    ParameterStateInfo.ErrorLevel.Warning => warningcolorBrush,
                    ParameterStateInfo.ErrorLevel.Error => errorcolorBrush,
                    _ => infocolorBrush,
                };
            }

            foreach (var item in sortedErrorList)
            {
                ErrorsList.Add(item);
            }
        } 
    }
}
