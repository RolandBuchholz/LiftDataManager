namespace LiftDataManager.Controls;

public sealed class BaseValidationTextBox : Control
{
    public BaseValidationTextBox()
    {
        DefaultStyleKey = typeof(BaseValidationTextBox);
    }

    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(BaseValidationTextBox), new PropertyMetadata(string.Empty));

    public string Placeholder
    {
        get { return (string)GetValue(PlaceholderProperty); }
        set { SetValue(PlaceholderProperty, value); }
    }

    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(BaseValidationTextBox), new PropertyMetadata(string.Empty));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(BaseValidationTextBox), new PropertyMetadata(string.Empty));

    private bool hasError;

    public bool HasError
    {
        get { return hasError; }
        set { hasError = value; }
    }

}
