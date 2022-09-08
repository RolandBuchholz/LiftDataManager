namespace LiftDataManager.Core.Contracts.Services;
public interface IValidationParameterDataService
{
    Task<List<ParameterStateInfo>> ValidateParameterAsync(string name, string value);
}
