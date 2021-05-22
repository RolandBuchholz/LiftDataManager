using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using SpeziInspector.Contracts.ViewModels;
using SpeziInspector.Core.Contracts.Services;
using SpeziInspector.Core.Models;
using SpeziInspector.Messenger;
using SpeziInspector.Messenger.Messages;

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
            get 
            {
                if (_item is not null)
                {
                    CanSaveParameter = _item.IsDirty;
                }
                return _item;
            }
            set
            {
                if (_item is not null)
                {
                   CanSaveParameter = _item.IsDirty;
                }
                SetProperty(ref _item, value); 
            }
        }

        public DatenansichtDetailViewModel(IParameterDataService parameterDataService)
        {
            _parameterDataService = parameterDataService;
            SaveParameter = new RelayCommand(SaveParameterAsync, () => CanSaveParameter);
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
            Debug.WriteLine("Daten werden in XML gespeichert :)");
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
        }

    }
}
