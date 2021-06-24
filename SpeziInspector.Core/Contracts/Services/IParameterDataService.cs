using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SpeziInspector.Core.Models;

namespace SpeziInspector.Core.Contracts.Services
{
    public interface IParameterDataService
    {
        Task<IEnumerable<Parameter>> LoadParameterAsync(string path);

        Task SaveParameterAsync(Parameter parameter, string path);

        void SaveAllParameterAsync(ObservableCollection<Parameter> ParamterList, string path);
    }
}
