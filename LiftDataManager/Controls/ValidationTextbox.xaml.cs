
namespace LiftDataManager.Controls;
public sealed class ValidationTextbox : TextBox
{
    public ValidationTextbox()
    {
        this.DefaultStyleKey = typeof(ValidationTextbox);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        SetValidationState();
        TextChanged += ValidationTextbox_TextChanged;
    }
    
    private void ValidationTextbox_TextChanged(object sender, TextChangedEventArgs e)
    {
        IsVaild = string.IsNullOrWhiteSpace(Text);
        SetValidationState();
    }

    private void SetValidationState() 
    {
        VisualStateManager.GoToState(this, IsVaild ? "IsNotVaild" : "IsVaild", useTransitions: false);
    }

    public bool IsVaild
    {
        get { return (bool)GetValue(IsVaildProperty); }
        set { SetValue(IsVaildProperty, value); }
    }

    public static readonly DependencyProperty IsVaildProperty =
        DependencyProperty.Register(nameof(IsVaild),
            typeof(bool), 
            typeof(ValidationTextbox), 
            new PropertyMetadata(default));
    public bool EnableStringEmptyCheck
    {
        get { return (bool)GetValue(EnableStringEmptyCheckProperty); }
        set { SetValue(EnableStringEmptyCheckProperty, value); }
    }

    public static readonly DependencyProperty EnableStringEmptyCheckProperty =
        DependencyProperty.Register(nameof(EnableStringEmptyCheck), 
            typeof(bool), 
            typeof(ValidationTextbox), 
            new PropertyMetadata(default));
}
