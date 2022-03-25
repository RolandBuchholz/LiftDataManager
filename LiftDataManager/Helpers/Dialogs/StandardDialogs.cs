using LiftDataManager.Core.Models;
using LiftDataManager.Services;
using System.Threading.Tasks;

namespace LiftDataManager.Helpers.Dialogs
{
    public static class StandardDialogs
    {
        public static async Task LiftDataManagerdownloadInfo(DownloadInfo downloadResult)
        {
            string title = "LiftDataManager InfoDialog";
            string content = DownloadInfoEnumToString(downloadResult);
            string closeButtonText = "Ok";

            await App.MainRoot.MessageDialogAsync(title,content,closeButtonText);
        }

        private static string DownloadInfoEnumToString(DownloadInfo downloadResult)
        {
            string enumText;
            switch (downloadResult.ExitState)
            {
                case DownloadInfo.ExitCodeEnum.NoError:
                    enumText = "kein Fehler";
                    break;
                case DownloadInfo.ExitCodeEnum.UpDownLoadError:
                    enumText = "Datei - Upload /Download/Reservierung enfernen fehlgeschlagen";
                    break;
                case DownloadInfo.ExitCodeEnum.LoginError:
                    enumText = "Login Fehler im Vault";
                    break;
                case DownloadInfo.ExitCodeEnum.UpdatePropertiesError:
                    enumText = "Eigenschaftsabgleich mit Vault fehlgeschlagen";
                    break;
                case DownloadInfo.ExitCodeEnum.PowerShellStartError:
                    enumText = "Interner Fehler in der PowerShellStart Programm";
                    break;
                case DownloadInfo.ExitCodeEnum.MultipleAutoDeskTransferXml:
                    enumText = "AutoDeskTransferXml mehrfach im Arbeitsbereich vorhanden";
                    break;
                case DownloadInfo.ExitCodeEnum.InvalideOrderNumber:
                    enumText = "Invalide Auftrags bzw. Angebotsnummer";
                    break;
                case DownloadInfo.ExitCodeEnum.MissingVaultFile:
                    enumText = "Datei im Vault oder Arbeitsbereich nicht gefunden";
                    break;
                case DownloadInfo.ExitCodeEnum.MissingAdskLicensingSDK_5:
                    enumText = "Fehlende AdskLicensingSDK_5.dll im Powershell Ordner";
                    break;
                case DownloadInfo.ExitCodeEnum.MissingVaultClient_DataStandard:
                    enumText = "Vault Client 2022 oder DataStandard wurde nicht gefunden";
                    break;
                default:
                    enumText = "keine ExitCode vorhanden";
                    break;
            }
            return enumText;
        }
    }
}
