using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;


namespace LiftDataManager.ViewModels;

public class DataViewModelBase : ObservableRecipient
{
    public readonly IParameterDataService _parameterDataService;
    public readonly IDialogService _dialogService;
    public readonly INavigationService _navigationService;

    private string _InfoSidebarPanelText;
    public bool Adminmode
    {
        get; set;
    }
    public bool AuftragsbezogeneXml
    {
        get; set;
    }
    public bool CheckOut
    {
        get; set;
    }
    public bool LikeEditParameter
    {
        get; set;
    }
    public string FullPathXml
    {
        get; set;
    }
    public bool CheckoutDialogIsOpen
    {
        get; set;
    }

    public CurrentSpeziProperties _CurrentSpeziProperties;
    public ObservableDictionary<string, Parameter> ParamterDictionary
    {
        get; set;
    }

    public DataViewModelBase(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService)
    {
        _parameterDataService = parameterDataService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        SaveAllSpeziParametersAsync = new AsyncRelayCommand(SaveAllParameterAsync, () => CanSaveAllSpeziParameters && Adminmode && AuftragsbezogeneXml);
    }

    public IAsyncRelayCommand SaveAllSpeziParametersAsync
    {
        get;
    }

    protected virtual void SynchronizeViewModelParameter()
    {
        _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (_CurrentSpeziProperties.FullPathXml is not null) { FullPathXml = _CurrentSpeziProperties.FullPathXml; }
        if (_CurrentSpeziProperties.ParamterDictionary is not null) { ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary; }
        Adminmode = _CurrentSpeziProperties.Adminmode;
        AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
        CheckOut = _CurrentSpeziProperties.CheckOut;
        LikeEditParameter = _CurrentSpeziProperties.LikeEditParameter;
        InfoSidebarPanelText = _CurrentSpeziProperties.InfoSidebarPanelText;
    }

    public async Task SaveAllParameterAsync()
    {
        var infotext = await _parameterDataService.SaveAllParameterAsync(ParamterDictionary, FullPathXml);
        InfoSidebarPanelText += infotext;
        await CheckUnsavedParametresAsync();
    }

    protected async virtual Task CheckUnsavedParametresAsync()
    {
        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            var dirty = ParamterDictionary.Values.Any(p => p.IsDirty);

            if (CheckOut)
            {
                CanSaveAllSpeziParameters = dirty;
            }
            else if (dirty && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                var dialogResult = await _dialogService.WarningDialogAsync(App.MainRoot,
                                    $"Datei eingechecked (schreibgeschützt)",
                                    $"Die AutodeskTransferXml wurde noch nicht ausgechecked!\n" +
                                    $"Es sind keine Änderungen möglich!\n" +
                                    $"\n" +
                                    $"Soll zur HomeAnsicht gewechselt werden um die Datei aus zu checken?",
                                    "Zur HomeAnsicht", "Schreibgeschützt bearbeiten");
                if (dialogResult)
                {
                    CheckoutDialogIsOpen = false;
                    _navigationService.NavigateTo("LiftDataManager.ViewModels.HomeViewModel");

                }
                else
                {
                    CheckoutDialogIsOpen = false;
                    LikeEditParameter = false;
                    _CurrentSpeziProperties.LikeEditParameter = LikeEditParameter;
                    _ = Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
                }
            }
        }
    }

    protected void SetInfoSidebarPanelText(ParameterDirtyMessage m)
    {
        if (m.Value.ParameterTyp == Parameter.ParameterTypValue.Date)
        {
            string datetimeOld;
            string datetimeNew;
            try
            {
                if (m.Value.OldValue is not null)
                {
                    var excelDateOld = Convert.ToDouble(m.Value.OldValue, CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                    datetimeOld = DateTime.FromOADate(excelDateOld).ToShortDateString();
                }
                else
                {
                    datetimeOld = string.Empty;
                }

                if (m.Value.NewValue is not null)
                {
                    var excelDateNew = Convert.ToDouble(m.Value.NewValue, CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                    datetimeNew = DateTime.FromOADate(excelDateNew).ToShortDateString();
                }
                else
                {
                    datetimeNew = string.Empty;
                }
                InfoSidebarPanelText += $"{m.Value.ParameterName} : {datetimeOld} => {datetimeNew} geändert \n";
            }
            catch
            {
                InfoSidebarPanelText += $"{m.Value.ParameterName} : {m.Value.OldValue} => {m.Value.NewValue} geändert \n";
            }
        }
        else
        {
            InfoSidebarPanelText += $"{m.Value.ParameterName} : {m.Value.OldValue} => {m.Value.NewValue} geändert \n";
        }
    }

    private bool _CanSaveAllSpeziParameters;
    public bool CanSaveAllSpeziParameters
    {
        get => _CanSaveAllSpeziParameters;
        set
        {
            SetProperty(ref _CanSaveAllSpeziParameters, value);
            SaveAllSpeziParametersAsync.NotifyCanExecuteChanged();
        }
    }

    public string InfoSidebarPanelText
    {
        get => _InfoSidebarPanelText;

        set
        {
            SetProperty(ref _InfoSidebarPanelText, value);
            _CurrentSpeziProperties.InfoSidebarPanelText = value;
            Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }
}
