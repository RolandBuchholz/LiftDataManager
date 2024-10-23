using Cogs.Collections;

namespace LiftDataManager.Core.Contracts.Services;

public interface IParameterDataService
{
    
    bool CanConnectDataBase();

    string GetCurrentUser();

    ObservableDictionary<string, Parameter> GetParameterDictionary();
    LiftHistoryEntry GenerateLiftHistoryEntry(Parameter parameter);

    Task<IEnumerable<TransferData>> LoadParameterAsync(string path);

    Task<IEnumerable<TransferData>> LoadPdfOfferAsync(string path);

    Task<IEnumerable<TransferData>> LoadMailOfferAsync(string path);

    Task<IEnumerable<LiftHistoryEntry>> LoadLiftHistoryEntryAsync(string path, bool includeCategory = false);

    Task<IEnumerable<Parameter>> InitializeParametereFromDbAsync();

    Task<Tuple<string, string, string?>> SaveParameterAsync(Parameter parameter, string path);

    Task<List<Tuple<string, string, string?>>> SaveAllParameterAsync(string path, bool adminmode);

    Task<IEnumerable<InfoCenterEntry>> UpdateParameterDictionary(string path, IEnumerable<TransferData> data, ObservableDictionary<string, Parameter> parameterDictionary, bool updateXml = true);

    Task<bool> UpdateAutodeskTransferAsync(string path, List<ParameterDto> parameterDtos);

    Task<List<InfoCenterEntry>> SyncFromAutodeskTransferAsync(string path, ObservableDictionary<string, Parameter> ParameterDictionary);

    Task<bool> AddParameterListToHistoryAsync(List<LiftHistoryEntry> historyEntrys, string path, bool clearHistory);

    Task StartAutoSaveTimerAsync(int period, string fullPath, bool adminMode);

    Task StopAutoSaveTimerAsync();
}