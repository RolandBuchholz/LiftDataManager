namespace LiftDataManager.Core.Messenger
{
    public class ParameterChangeInfo
    {
        public Models.Parameter.ParameterTypValue ParameterTyp
        {
            get; set;
        }
        public bool IsDirty
        {
            get; set;
        }
        public string ParameterName
        {
            get; set;
        }
        public string OldValue
        {
            get; set;
        }
        public string NewValue
        {
            get; set;
        }
    }
}
