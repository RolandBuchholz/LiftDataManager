namespace LiftDataManager.Core.Models;

public class DownloadInfo
{
    public enum ExitCodeEnum
    {
        NoError = 0,
        UpDownLoadError = 1,
        LoginError = 2,
        UpdatePropertiesError = 3,
        PowerShellStartError = 4,
        MultipleAutoDeskTransferXml = 5,
        InvalideOrderNumber = 6,
        MissingVaultFile = 7,
        MissingAdskLicensingSDK_5 = 8,
        MissingVaultClient_DataStandard = 9,
        CheckedOutByOtherUser = 10,
        CheckedOutLinkedFilesByOtherUser = 11
    }

    public bool Success { get; set; }
    public string? FileName { get; set; }
    public string? FullFileName { get; set; }
    public string? CheckOutState { get; set; }
    public bool IsCheckOut { get; set; }
    public string? CheckOutPC { get; set; }
    public string? EditedBy { get; set; }
    public string? ErrorState { get; set; }
    public ExitCodeEnum ExitState { get; set; }

    private int _ExitCode;
    public int ExitCode
    {
        get => _ExitCode;
        set
        {
            _ExitCode = value;
            ExitState = (ExitCodeEnum)_ExitCode;
        }
    }

    public string DownloadInfoEnumToString()
    {
        return ExitState switch
        {
            ExitCodeEnum.NoError => "kein Fehler",
            ExitCodeEnum.UpDownLoadError => "Datei - Upload /Download/Reservierung enfernen fehlgeschlagen",
            ExitCodeEnum.LoginError => "Login Fehler im Vault",
            ExitCodeEnum.UpdatePropertiesError => "Eigenschaftsabgleich mit Vault fehlgeschlagen",
            ExitCodeEnum.PowerShellStartError => "Interner Fehler in der PowerShellStart Programm",
            ExitCodeEnum.MultipleAutoDeskTransferXml => "AutoDeskTransferXml mehrfach im Arbeitsbereich vorhanden",
            ExitCodeEnum.InvalideOrderNumber => "Invalide Auftrags bzw. Angebotsnummer",
            ExitCodeEnum.MissingVaultFile => "Datei im Vault oder Arbeitsbereich nicht gefunden",
            ExitCodeEnum.MissingAdskLicensingSDK_5 => "Fehlende AdskLicensingSDK_5.dll im Powershell Ordner",
            ExitCodeEnum.MissingVaultClient_DataStandard => "Vault Client 2022 oder DataStandard wurde nicht gefunden",
            ExitCodeEnum.CheckedOutByOtherUser => "AutoDeskTransferXml durch anderen Benutzer ausgechecked",
            ExitCodeEnum.CheckedOutLinkedFilesByOtherUser => "AutoDeskTransferXml verbundene Dateien durch anderen Benutzer ausgechecked",
            _ => "keine ExitCode vorhanden",
        };
    }
}
