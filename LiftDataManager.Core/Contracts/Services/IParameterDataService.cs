﻿using Cogs.Collections;

namespace LiftDataManager.Core.Contracts.Services;

public interface IParameterDataService
{
    bool CanConnectDataBase();

    string GetCurrentUser();

    LiftHistoryEntry GenerateLiftHistoryEntry(Parameter parameter);

    Task<IEnumerable<TransferData>> LoadParameterAsync(string path);

    Task<IEnumerable<TransferData>> LoadPdfOfferAsync(string path);

    Task<IEnumerable<LiftHistoryEntry>> LoadLiftHistoryEntryAsync(string path, bool includeCategory = false);

    Task<IEnumerable<Parameter>> InitializeParametereFromDbAsync();

    Task<KeyValuePair<string, string?>> SaveParameterAsync(Parameter parameter, string path);

    Task<List<KeyValuePair<string, string?>>> SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParameterDictionary, string path, bool adminmode);

    Task<bool> UpdateAutodeskTransferAsync(string path, List<ParameterDto> parameterDtos);

    Task<List<string>> SyncFromAutodeskTransferAsync(string path, ObservableDictionary<string, Parameter> ParameterDictionary);

    Task<bool> AddParameterListToHistoryAsync(List<LiftHistoryEntry> historyEntrys, string path, bool clearHistory);
}