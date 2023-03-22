using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LiftDataManager.ViewModels;

public partial class DatenansichtDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>
{
    public ObservableCollection<ParameterStateInfo> ErrorsList { get; set; }
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
                _item.PropertyChanged += OnPropertyChanged;
        }
    }

    public DatenansichtDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        ErrorsList ??= new();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs? e)
    {
        CheckIsDirty((Parameter?)sender);
    }

    private void CheckIsDirty(Parameter? Item)
    {
        if (Item is not null && Item.IsDirty)
        {
            CanSaveParameter = Item.DefaultUserEditable ? CheckOut : Adminmode && CheckOut;
        }
        else
        {
            CanSaveParameter = false;
        }
        SaveParameterCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveParameterCommand))]
    private bool canSaveParameter;

    [RelayCommand(CanExecute = nameof(CanSaveParameter))]
    private async Task SaveParameterAsync()
    {
        if (Item is null)
            return;
        if (FullPathXml == null)
            return;
        var infotext = await _parameterDataService!.SaveParameterAsync(Item, FullPathXml);
        InfoSidebarPanelText += infotext;
        CanSaveParameter = false;
        if (Item != null)
            Item.IsDirty = false;
    }

    private void SetParameterState(Parameter? liftParameter)
    {
        ErrorsList.Clear();

        if (liftParameter is null)
            return;

        if (!liftParameter.HasErrors)
        {
            ErrorsList.Add(new ParameterStateInfo(liftParameter.Name!, liftParameter.DisplayName!, true)
            {
                Severity = ParameterStateInfo.ErrorLevel.Valid,
                ErrorMessage = "Keine Information, Warnungen oder Fehler vorhanden"
            });
        }

        if (liftParameter.parameterErrors.TryGetValue("Value", out List<ParameterStateInfo>? errorList))
        {
            if (errorList is null)
                return;
            if (!errorList.Any())
                return;

            var sortedErrorList = errorList.OrderByDescending(p => p.Severity);
            foreach (var item in sortedErrorList)
            {
                ErrorsList.Add(item);
            }
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (parameter is not null && ParamterDictionary is not null)
        {
            var data = ParamterDictionary.Values.Where(p => !string.IsNullOrWhiteSpace(p.Name));
            Item = data.First(i => i.Name == (string)parameter);
            SetParameterState(Item);
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
        if (Item != null)
            Item.PropertyChanged -= OnPropertyChanged;
    }
}
