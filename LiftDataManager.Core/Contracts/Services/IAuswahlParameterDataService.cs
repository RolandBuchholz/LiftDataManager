using System.Collections.Generic;
using System.Threading.Tasks;
using LiftDataManager.Core.Models;

namespace LiftDataManager.Core.Contracts.Services
{
    public interface IAuswahlParameterDataService
    {
        Dictionary<string, AuswahlParameter> AuswahlParameterDictionary
        {
            get; set;
        }
        List<string> GetListeAuswahlparameter(string name);
        Task<string> UpdateAuswahlparameterAsync();
        bool ParameterHasAuswahlliste(string name);
    }
}
