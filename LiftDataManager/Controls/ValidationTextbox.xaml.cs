namespace LiftDataManager.Controls;
public sealed class ValidationTextbox : TextBox
{
    public enum ValidationTypMode
    {
        None,
        Email,
        Phone
    }

    public ValidationTextbox()
    {
        this.DefaultStyleKey = typeof(ValidationTextbox);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        TextChanged += ValidationTextbox_TextChanged;
        SetValidationState();
    }

    private void ValidationTextbox_TextChanged(object sender, TextChangedEventArgs e)
    {
        SetValidationState();
    }

    private void SetValidationState()
    {
        IsVaild = !EnableStringEmptyCheck || !string.IsNullOrWhiteSpace(Text);

        if (!IsVaild)
        {
            ErrorInfo = "*Pflichteingabe";
        }
        else if (IsVaild && ValidationMode != ValidationTypMode.None)
        {
            switch (ValidationMode)
            {
                case ValidationTypMode.None:
                    break;
                case ValidationTypMode.Email:
                    IsVaild = Text.IsEmail();
                    ErrorInfo = IsVaild ? string.Empty : "ungültige Emailadresse";
                    break;
                case ValidationTypMode.Phone:
                    IsVaild = Text.IsPhoneNumber();
                    ErrorInfo = IsVaild ? string.Empty : "ungültige Telefonnummer";
                    break;
                default:
                    break;
            }
        }

        VisualStateManager.GoToState(this, IsVaild ? "IsVaild" : "IsNotVaild", useTransitions: false);
    }

    public ValidationTypMode ValidationMode
    {
        get { return (ValidationTypMode)GetValue(ValidationModeProperty); }
        set { SetValue(ValidationModeProperty, value); }
    }


    public static readonly DependencyProperty ValidationModeProperty =
        DependencyProperty.Register(nameof(ValidationMode),
            typeof(ValidationTypMode),
            typeof(ValidationTextbox),
            new PropertyMetadata(ValidationTypMode.None));

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

    public string ErrorInfo
    {
        get { return (string)GetValue(ErrorInfoProperty); }
        set { SetValue(ErrorInfoProperty, value); }
    }

    public static readonly DependencyProperty ErrorInfoProperty =
        DependencyProperty.Register(nameof(ErrorInfo),
            typeof(string),
            typeof(ValidationTextbox),
            new PropertyMetadata("*Pflichteingabe"));
}
