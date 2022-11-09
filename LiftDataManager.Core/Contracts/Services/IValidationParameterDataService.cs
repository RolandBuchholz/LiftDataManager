namespace LiftDataManager.Core.Contracts.Services;
public interface IValidationParameterDataService
{
    Task<List<ParameterStateInfo>> ValidateParameterAsync(string name, string displayname, string? value);

    Task ValidateAllParameterAsync();

    Task ValidateRangeOfParameterAsync(string[] range);
}
