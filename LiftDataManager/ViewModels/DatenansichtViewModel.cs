using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;
using Microsoft.UI.Xaml.Data;

namespace LiftDataManager.ViewModels;

public class DatenansichtViewModel : DataViewModelBase, INavigationAware
{
    public CollectionViewSource GroupedItems
    {
        get; set;
    }

    public DatenansichtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, async (r, m) =>
        {
            if (m is not null && m.Value.IsDirty)
            {
                SetInfoSidebarPanelText(m);
                await CheckUnsavedParametresAsync();
            }
        });

        GroupedItems = new CollectionViewSource
        {
            IsSourceGrouped = true
        };
    }

    private ICommand _itemClickCommand;
    public ICommand ItemClickCommand => _itemClickCommand ?? (_itemClickCommand = new RelayCommand<Parameter>(OnItemClick));

    override protected async Task CheckUnsavedParametresAsync()
    {
        if (LikeEditParameter && AuftragsbezogeneXml)
        {
            bool dirty = ParamterDictionary.Values.Any(p => p.IsDirty);

            if (CheckOut)
            {
                CanShowUnsavedParameters = dirty;
                CanSaveAllSpeziParameters = dirty;
            }
            else if (dirty && !CheckOut && !CheckoutDialogIsOpen)
            {
                CheckoutDialogIsOpen = true;
                bool dialogResult = await _dialogService.WarningDialogAsync(App.MainRoot,
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
                }
            }

        }
    }

    private bool _CanShowUnsavedParameters;
    public bool CanShowUnsavedParameters
    {
        get => _CanShowUnsavedParameters;
        set => SetProperty(ref _CanShowUnsavedParameters, value);
    }

    private string _SearchInput;
    public string SearchInput
    {
        get => _SearchInput;

        set
        {
            SetProperty(ref _SearchInput, value);
            _CurrentSpeziProperties.SearchInput = SearchInput;
            Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
        SearchInput = _CurrentSpeziProperties.SearchInput;
        if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = CheckUnsavedParametresAsync();
    }

    public void OnNavigatedFrom()
    {
        WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
    }

    private void OnItemClick(Parameter clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(DatenansichtDetailViewModel).FullName, clickedItem.Name);
        }
    }
}
