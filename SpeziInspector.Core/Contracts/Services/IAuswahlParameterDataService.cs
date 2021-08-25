using SpeziInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeziInspector.Core.Contracts.Services
{
    public interface IAuswahlParameterDataService
    {
        Dictionary<string, AuswahlParameter> AuswahlParameterDictionary { get; set; }
        List<string> GetListeAuswahlparameter(string name);
        Task<string> UpdateAuswahlparameterAsync();
        bool ParameterHasAuswahlliste(string name);
    }
}
