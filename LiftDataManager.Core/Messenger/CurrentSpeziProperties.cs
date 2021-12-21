using Cogs.Collections;
using LiftDataManager.Core.Models;

namespace LiftDataManager.Core.Messenger
{
    public class CurrentSpeziProperties
    {
        public bool Adminmode { get; set; }
        public bool AuftragsbezogeneXml { get; set; }
        public string SpezifikationStatusTyp { get; set; }
        public string FullPathXml { get; set; }
        public string SearchInput { get; set; }
        public string InfoSidebarPanelText { get; set; }
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
    }
}
