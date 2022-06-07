namespace LiftDataManager.Core.Models
{
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

        private int _ExitCode;
        public int ExitCode
        {
            get { return _ExitCode; }
            set
            {
                _ExitCode = value;
                ExitState = (ExitCodeEnum)_ExitCode;
            }
        }

        public bool Success { get; set; }
        public string FileName { get; set; }
        public string FullFileName { get; set; }
        public string CheckOutState { get; set; }
        public bool IsCheckOut { get; set; }
        public string CheckOutPC { get; set; }
        public string EditedBy { get; set; }
        public string ErrorState { get; set; }
        public ExitCodeEnum ExitState { get; set; }
    }
}
