using Cogs.Collections;

namespace LiftDataManager.Core.Contracts.Services;

public interface IParameterDataService
{
    bool CanConnectDataBase();

    Task<IEnumerable<TransferData>> LoadParameterAsync(string path);

    Task<IEnumerable<Parameter>> InitializeParametereFromDbAsync();

    Task<string> SaveParameterAsync(Parameter parameter, string path);

    Task<string> SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParamterDictionary, string path, bool adminmode);
}