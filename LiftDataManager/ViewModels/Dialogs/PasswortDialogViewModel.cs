namespace LiftDataManager.ViewModels.Dialogs;

public partial class PasswortDialogViewModel : ObservableObject
{
    public PasswortDialogViewModel()
    {
        _password = "0000";
    }

    [RelayCommand]
    public async Task PasswortDialogLoadedAsync(PasswortDialog sender)
    {
        _password = !string.IsNullOrWhiteSpace(sender.Passwort) ? sender.Passwort : "0000";
        await Task.CompletedTask;
    }

    private string _password;

    public int PasswordLenght => _password.Length;

    [ObservableProperty]
    public partial bool CanSwitchToMode { get; set; }

    [ObservableProperty]
    public partial string? PasswortInfoText { get; set; } = "Kein PIN eingegeben";

    [ObservableProperty]
    public partial string? PasswortInput { get; set; }
    partial void OnPasswortInputChanged(string? value)
    {
        CheckpasswortInput();
    }
    [ObservableProperty]
    public partial bool SwitchModeWarningAccepted { get; set; }
    partial void OnSwitchModeWarningAcceptedChanged(bool value)
    {
        CanSwitchToMode = (SwitchModeWarningAccepted == true &&
                                PasswortInfoText == "Passwort korrekt Zugriff gewährt");
    }

    private void CheckpasswortInput()
    {
        if (string.IsNullOrWhiteSpace(_password))
        {
            PasswortInfoText = "Kein Passwort eingegeben";
            CanSwitchToMode = false;
            return;
        }
        if (!string.Equals(PasswortInput, _password))
        {
            PasswortInfoText = "Incorrectes Passwort";
            CanSwitchToMode = false;
            return;
        }
        PasswortInfoText = "Passwort korrekt Zugriff gewährt";
        CanSwitchToMode = SwitchModeWarningAccepted;
    }
}