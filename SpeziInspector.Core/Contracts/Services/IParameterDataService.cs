using Cogs.Collections;
using SpeziInspector.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeziInspector.Core.Contracts.Services
{
    public interface IParameterDataService
    {
        Task<IEnumerable<Parameter>> LoadParameterAsync(string path);

        Task SaveParameterAsync(Parameter parameter, string path);

        Task SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParamterDictionary, string path);
    }
}
