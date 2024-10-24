using Cogs.Collections;

namespace LiftDataManager.Core.Contracts.Services;

/// <summary>
/// The default <see langword="interface"/> for a service that handles Validation ParameterDataService.
/// </summary>
public interface IValidationParameterDataService
{
    /// <summary>
    /// Set parameterDictionary
    /// </summary>
    /// <param name="ObservableDictionary<string, Parameter>">parameterDictionary</param>
    /// <returns>Task<List<ParameterStateInfo>></returns>
    Task InitializeValidationParameterDataServicerAsync(ObservableDictionary<string, Parameter> parameterDictionary);

    /// <summary>
    /// Set Path of Autodesktransferxml
    /// </summary>
    /// <param name="path">Autodesktransferxmlpath</param>
    /// <returns>Task<List<ParameterStateInfo>></returns>
    Task SetFullPathXmlAsync(string? path);

    /// <summary>
    /// Validate a liftparameter
    /// </summary>
    /// <param name="name">Liftparametername</param>
    /// <param name="displayname">Liftparameterdisplayname</param>
    /// <param name="value">Liftparametervalue</param>
    /// <returns>Task<List<ParameterStateInfo>></returns>
    Task<List<ParameterStateInfo>> ValidateParameterAsync(string name, string displayname, string? value);

    /// <summary>
    /// Validate all liftparameter
    /// </summary>
    /// <returns>Task</returns>
    Task ValidateAllParameterAsync();

    /// <summary>
    /// Validate range of Liftparameter
    /// </summary>
    /// <param name="range">Range of Liftparameter</param>
    /// <returns>Task</returns>
    Task ValidateRangeOfParameterAsync(string[] range);
}