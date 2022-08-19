using System.ComponentModel;

namespace LiftDataManager.ViewModels;

public class DatenansichtDetailViewModel : DataViewModelBase, INavigationAware
{
    private Parameter? _item;

    public Parameter? Item
    {
        get
        {
            CheckIsDirty(_item);
            return _item;
        }

        set
        {
            SetProperty(ref _item, value);
            if (_item != null)
            {
                _item.PropertyChanged += OnPropertyChanged;
            }
        }
    }

    public DatenansichtDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
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
        SaveParameter = new AsyncRelayCommand(SaveParameterAsync, () => CanSaveParameter && Adminmode && CheckOut);
    }

    public IAsyncRelayCommand SaveParameter
    {
        get;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs? e)
    {
        CheckIsDirty((Parameter?)sender);
    }

    private void CheckIsDirty(Parameter? Item)
    {
        if (Item is not null && Item.IsDirty)
        {
            CanSaveParameter = true;
        }
        else
        {
            CanSaveParameter = false;
        }
        SaveParameter.NotifyCanExecuteChanged();
    }

    private bool _CanSaveParameter;
    public bool CanSaveParameter
    {
        get => _CanSaveParameter;
        set
        {
            SetProperty(ref _CanSaveParameter, value);
            SaveParameter.NotifyCanExecuteChanged();
        }
    }

    private async Task SaveParameterAsync()
    {
        var infotext = await _parameterDataService.SaveParameterAsync(Item, FullPathXml);
        InfoSidebarPanelText += infotext;
        CanSaveParameter = false;
        if (Item != null)
        {
            Item.IsDirty = false;
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
        if (parameter is not null)
        {
            var data = ParamterDictionary.Values.Where(p => !string.IsNullOrWhiteSpace(p.Name));
            Item = data.First(i => i.Name == (string)parameter);
        }
    }

    public void OnNavigatedFrom()
    {
        if (Item != null)
        {
            Item.PropertyChanged -= OnPropertyChanged;
        }
    }
}
