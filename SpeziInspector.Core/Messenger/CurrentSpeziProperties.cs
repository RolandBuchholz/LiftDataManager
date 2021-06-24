using SpeziInspector.Core.Models;
using System.Collections.ObjectModel;

namespace SpeziInspector.Messenger
{
    public class  CurrentSpeziProperties
    {
        public bool Adminmode { get; set; }
        public bool AuftragsbezogeneXml { get; set; }
        public string FullPathXml { get; set; }
        public string SearchInput { get; set; }
        public ObservableCollection<Parameter> ParamterList { get; set; }

    }
}
