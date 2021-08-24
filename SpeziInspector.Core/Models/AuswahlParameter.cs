using System.Collections.Generic;

namespace SpeziInspector.Core.Models
{
    public class AuswahlParameter
    {
        public string Name { get; set; }

        public List<string> Auswahlliste { get; set; } = new();
    }
}
