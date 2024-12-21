namespace LiftDataManager.ViewModels.Dialogs;

public partial class PasswortDialogViewModel : ObservableObject
{
    public PasswortDialogViewModel()
    {
        //TODO Get _password vom settings
        _password = "2342";
    }
    private readonly string _password;

    public int PasswordLenght => _password.Length;

    [ObservableProperty]
    public partial bool CanSwitchToAdminmode { get; set; }

    [ObservableProperty]
    public partial string? PasswortInfoText { get; set; } = "Kein PIN eingegeben";

    [ObservableProperty]
    public partial string? PasswortInput { get; set; }
    partial void OnPasswortInputChanged(string? value)
    {
        CheckpasswortInput();
    }
    [ObservableProperty]
    public partial bool AdminmodeWarningAccepted { get; set; }
    partial void OnAdminmodeWarningAcceptedChanged(bool value)
    {
        CanSwitchToAdminmode = (AdminmodeWarningAccepted == true
                        && PasswortInfoText == "Passwort korrekt Zugriff gewährt");
    }

    private void CheckpasswortInput()
    {
        if (string.IsNullOrWhiteSpace(_password))
        {
            PasswortInfoText = "Kein Passwort eingegeben";
            CanSwitchToAdminmode = false;
            return;
        }
        if (!string.Equals(PasswortInput, _password))
        {
            PasswortInfoText = "Incorrectes Passwort";
            CanSwitchToAdminmode = false;
            return;
        }
        PasswortInfoText = "Passwort korrekt Zugriff gewährt";
        CanSwitchToAdminmode = AdminmodeWarningAccepted;
    }
}