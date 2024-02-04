using System.ComponentModel.DataAnnotations;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// ExitCodes Vault down and upload with german humanized Displayname
/// </summary>
public enum ExitCodeEnum
{
    /// <summary>
    /// ExitCode no error with german humanized Displayname
    /// </summary>
    [Display(Name = "Kein Fehler")]
    NoError = 0,

    /// <summary>
    /// ExitCode upload or download error with german humanized Displayname
    /// </summary>
    [Display(Name = "Datei - Upload /Download/Reservierung enfernen fehlgeschlagen")]
    UpDownLoadError = 1,

    /// <summary>
    /// ExitCode login error with german humanized Displayname
    /// </summary>
    [Display(Name = "Vault Login fehlgeschlagen")]
    LoginError = 2,

    /// <summary>
    /// ExitCode update properties error with german humanized Displayname
    /// </summary>
    [Display(Name = "Eigenschaftsabgleich mit Vault fehlgeschlagen")]
    UpdatePropertiesError = 3,

    /// <summary>
    /// ExitCode powerShell start error with german humanized Displayname
    /// </summary>
    [Display(Name = "Interner Fehler in der PowerShellStart Programm")]
    PowerShellStartError = 4,

    /// <summary>
    /// ExitCode multiple AutoDeskTransferXml with german humanized Displayname
    /// </summary>
    [Display(Name = "AutoDeskTransferXml mehrfach im Arbeitsbereich vorhanden")]
    MultipleAutoDeskTransferXml = 5,

    /// <summary>
    /// ExitCode invalide order number with german humanized Displayname
    /// </summary>
    [Display(Name = "Invalide Auftrags bzw. Angebotsnummer")]
    InvalideOrderNumber = 6,

    /// <summary>
    /// ExitCode missing vaultfile with german humanized Displayname
    /// </summary>
    [Display(Name = "Datei im Vault oder Arbeitsbereich nicht gefunden")]
    MissingVaultFile = 7,

    /// <summary>
    /// ExitCode Missing AdskLicensingSDK with german humanized Displayname
    /// </summary>
    [Display(Name = "Fehlende AdskLicensingSDK im Powershell Ordner")]
    MissingAdskLicensingSDK_5 = 8,

    /// <summary>
    /// ExitCode missing VaultClient DataStandard with german humanized Displayname
    /// </summary>
    [Display(Name = "Vault Client oder DataStandard wurde nicht gefunden")]
    MissingVaultClient_DataStandard = 9,

    /// <summary>
    /// ExitCode checked out by other user with german humanized Displayname
    /// </summary>
    [Display(Name = "AutoDeskTransferXml durch anderen Benutzer ausgechecked")]
    CheckedOutByOtherUser = 10,

    /// <summary>
    /// ExitCode checked out linked files by other user with german humanized Displayname
    /// </summary>
    [Display(Name = "AutoDeskTransferXml verbundene Dateien durch anderen Benutzer ausgechecked")]
    CheckedOutLinkedFilesByOtherUser = 11
}