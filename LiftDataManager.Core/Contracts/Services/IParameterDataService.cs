using Cogs.Collections;
using LiftDataManager.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftDataManager.Core.Contracts.Services
{
    public interface IParameterDataService
    {
        Task<IEnumerable<Parameter>> LoadParameterAsync(string path);

        Task<string> SaveParameterAsync(Parameter parameter, string path);

        Task<string> SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParamterDictionary, string path);
    }
}
