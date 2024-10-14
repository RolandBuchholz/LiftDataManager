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
    private bool canSwitchToAdminmode;

    [ObservableProperty]
    private string? passwortInfoText = "Kein PIN eingegeben";

    [ObservableProperty]
    private string? passwortInput;
    partial void OnPasswortInputChanged(string? value)
    {
        CheckpasswortInput();
    }
    [ObservableProperty]
    private bool adminmodeWarningAccepted;
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