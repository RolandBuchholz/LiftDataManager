namespace LiftDataManager.Controls;
public sealed partial class PrePlanningNumberControl : UserControl
{
    public PrePlanningNumberControl()
    {
        InitializeComponent();
    }
    private string? _PrePlanningYear;
    public string? PrePlanningYear
    {
        get => _PrePlanningYear;
        set
        {
            _PrePlanningYear = value;
            SetPrePlanningNumber();
        }
    }

    private string? _PrePlanningId;
    public string? PrePlanningId
    {
        get => _PrePlanningId;
        set
        {
            _PrePlanningId = value;
            SetPrePlanningNumber();
        }
    }

    public string? PrePlanningNumber
    {
        get => (string)GetValue(PrePlanningNumberProperty);
        set => SetValue(PrePlanningNumberProperty, value);
    }

    public static readonly DependencyProperty PrePlanningNumberProperty =
        DependencyProperty.Register("PrePlanningNumber", typeof(string), typeof(PrePlanningNumberControl), new PropertyMetadata(string.Empty));

    private void SetPrePlanningNumber()
    {
        string? fullPrePlanningId;

        if (PrePlanningId is null)
        {
            fullPrePlanningId = "0000";
        }
        else
        {
            fullPrePlanningId = PrePlanningId;
        }

        while (fullPrePlanningId.Length < 4)
        {
            fullPrePlanningId = "0" + fullPrePlanningId;
        }

        PrePlanningNumber = string.Concat("VP-", PrePlanningYear.AsSpan(2, 2), "-", fullPrePlanningId);
    }

}
