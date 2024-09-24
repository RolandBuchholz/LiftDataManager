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
        //Text = "Hallllo";
    }

}
