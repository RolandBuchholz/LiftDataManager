using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using SpeziInspector.Contracts.ViewModels;
using SpeziInspector.Core.Contracts.Services;
using SpeziInspector.Core.Models;
using SpeziInspector.Messenger;
using SpeziInspector.Messenger.Messages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SpeziInspector.ViewModels
{
    public class DatenansichtDetailViewModel : ObservableRecipient, INavigationAware
    {
        private readonly IParameterDataService _parameterDataService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        public ObservableCollection<Parameter> ParamterList { get; set; }
        private Parameter _item;

        public Parameter Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
                _item.PropertyChanged += CheckIsDirty;
            }
                
        }
        public DatenansichtDetailViewModel(IParameterDataService parameterDataService)
        {
            _parameterDataService = parameterDataService;
            SaveParameter = new RelayCommand(SaveParameterAsync, () => CanSaveParameter);
        }

        private void CheckIsDirty(object sender, PropertyChangedEventArgs e)
        {
            if (Item.IsDirty)
            {
                CanSaveParameter = true;
            }
        }

        public IRelayCommand SaveParameter { get; }

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
        private void SaveParameterAsync()
        {
            Debug.WriteLine(Item.Name+" in XML gespeichert :)");
            CanSaveParameter = false;
            Item.IsDirty = false;
        }
        public void OnNavigatedTo(object parameter)
        {
            if (parameter is not null)
            {
                _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
                if (_CurrentSpeziProperties.ParamterList is not null) { ParamterList = _CurrentSpeziProperties.ParamterList; }
                var data = ParamterList.Where(p => !string.IsNullOrWhiteSpace(p.Name));
                Item = data.First(i => i.Name == (string)parameter);
            }
        }
        public void OnNavigatedFrom()
        {
            Item.PropertyChanged -= CheckIsDirty;
        }
    }
}
