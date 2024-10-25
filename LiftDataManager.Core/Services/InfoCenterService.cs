using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models;
using Microsoft.Extensions.Logging;
using MvvmHelpers;

namespace LiftDataManager.Core.Services;

/// <summary>
/// A <see langword="class"/> that implements the <see cref="IInfoCenterService"/> <see langword="interface"/> add infos to the infoCenter.
/// </summary>
public class InfoCenterService : IInfoCenterService
{
    private ObservableRangeCollection<InfoCenterEntry> _infoCenterEntrys { get; set; }
    private readonly ILogger<InfoCenterService> _logger;

    public InfoCenterService(ILogger<InfoCenterService> logger)
    {
        _logger = logger;
        _infoCenterEntrys ??= [];
    }

    /// <inheritdoc/>
    public ObservableRangeCollection<InfoCenterEntry> GetInfoCenterEntrys()
    {
        if (_infoCenterEntrys is not null)
        {
            return _infoCenterEntrys;
        }
        else
        {
            _logger.LogCritical(60100, "InfoCenterEntrys are null");
            return [];
        }
    }

    /// <inheritdoc/>
    public async Task AddInfoCenterMessageAsync(string message)
    {
        _infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterMessage)
        {
            Message = await Task.FromResult(message)
        });
    }

    /// <inheritdoc/>
    public async Task AddInfoCenterWarningAsync(string warning)
    {
        _infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterWarning)
        {
            Message = await Task.FromResult(warning)
        });
    }

    /// <inheritdoc/>
    public async Task AddInfoCenterErrorAsync(string error)
    {
        _infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterError)
        {
            Message = await Task.FromResult(error)
        });
    }

    /// <inheritdoc/>
    public async Task AddInfoCenterParameterChangedAsync(string uniqueName, string parameterName, string oldValue, string newValue, bool autoUpdated)
    {
        _infoCenterEntrys.Add(new InfoCenterEntry(autoUpdated ? InfoCenterEntryState.InfoCenterAutoUpdate : InfoCenterEntryState.InfoCenterParameterChanged)
        {
            UniqueName = uniqueName,
            ParameterName = parameterName,
            OldValue = oldValue,
            NewValue = newValue
        });
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task AddInfoCenterSaveInfoAsync(Tuple<string, string, string?> savedParameter)
    {
        if (_infoCenterEntrys is null)
        {
            return;
        }
        if (string.Equals(savedParameter.Item1, "Error", StringComparison.CurrentCultureIgnoreCase))
        {
            _infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterError)
            {
                Message = savedParameter.Item3
            });
        }
        else
        {
            var obsoleteEntrys = _infoCenterEntrys.Where(x => x.UniqueName == savedParameter.Item1).ToList();
            _infoCenterEntrys.RemoveRange(obsoleteEntrys);
            _infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterSaveParameter)
            {
                UniqueName = savedParameter.Item1,
                ParameterName = savedParameter.Item2,
                NewValue = savedParameter.Item3
            });
        }
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task AddInfoCenterSaveAllInfoAsync(IEnumerable<Tuple<string, string, string?>> savedParameters)
    {
        if (_infoCenterEntrys is null)
        {
            return;
        }
        List<InfoCenterEntry> newEntrys = [];
        List<InfoCenterEntry> obsoleteEntrys = [];

        foreach (var savedParameter in savedParameters)
        {
            if (string.Equals(savedParameter.Item1, "Error", StringComparison.CurrentCultureIgnoreCase))
            {
                newEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterError)
                {
                    Message = savedParameter.Item3
                });
            }
            else
            {
                obsoleteEntrys.AddRange(_infoCenterEntrys.Where(x => x.UniqueName == savedParameter.Item1).ToList());
                newEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterSaveParameter)
                {
                    UniqueName = savedParameter.Item1,
                    ParameterName = savedParameter.Item2,
                    NewValue = savedParameter.Item3
                });
            }
        }
        _infoCenterEntrys.RemoveRange(obsoleteEntrys);
        _infoCenterEntrys.AddRange(newEntrys);
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task AddListofInfoCenterEntrysAsync(IEnumerable<InfoCenterEntry> newInfoCenterEntrys)
    {
        foreach (var newInfoCenterEntry in newInfoCenterEntrys)
        {
            _infoCenterEntrys.Add(newInfoCenterEntry);
        }
        await Task.CompletedTask;
    }
}