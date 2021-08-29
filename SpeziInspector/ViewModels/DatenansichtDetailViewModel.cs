using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SpeziInspector.Contracts.ViewModels;
using SpeziInspector.Core.Contracts.Services;
using SpeziInspector.Core.Messenger;
using SpeziInspector.Core.Messenger.Messages;
using SpeziInspector.Core.Models;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SpeziInspector.ViewModels
{
    public class DatenansichtDetailViewModel : ObservableRecipient, INavigationAware
    {
        private readonly IParameterDataService _parameterDataService;
        private CurrentSpeziProperties _CurrentSpeziProperties;
        private bool Adminmode;
        private bool AuftragsbezogeneXml;
        public string FullPathXml;
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
        private Parameter _item;

        public Parameter Item
        {
            get
            {
                CheckIsDirty(_item);
                return _item;
            }

            set
            {
                SetProperty(ref _item, value);
                _item.PropertyChanged += OnPropertyChanged;
            }
        }
        public DatenansichtDetailViewModel(IParameterDataService parameterDataService)
        {
            _parameterDataService = parameterDataService;
            SaveParameter = new AsyncRelayCommand(SaveParameterAsync, () => CanSaveParameter && Adminmode && AuftragsbezogeneXml);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckIsDirty((Parameter)sender);
        }

        public IAsyncRelayCommand SaveParameter { get; }

        private void CheckIsDirty(Parameter Item)
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
        private string _SearchInputInfoSidebarPanelText;
        public string SearchInputInfoSidebarPanelText
        {
            get => _SearchInputInfoSidebarPanelText;

            set
            {
                SetProperty(ref _SearchInputInfoSidebarPanelText, value);
                _CurrentSpeziProperties.SearchInputInfoSidebarPanelText = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }
        private async Task SaveParameterAsync()
        {
            await _parameterDataService.SaveParameterAsync(Item, FullPathXml);
            CanSaveParameter = false;
            CanSaveParameter = false;
            Item.IsDirty = false;
        }
        public void OnNavigatedTo(object parameter)
        {
            if (parameter is not null)
            {
                _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
                if (_CurrentSpeziProperties.FullPathXml is not null) FullPathXml = _CurrentSpeziProperties.FullPathXml;
                if (_CurrentSpeziProperties.ParamterDictionary is not null) ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary;
                Adminmode = _CurrentSpeziProperties.Adminmode;
                AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
                SearchInputInfoSidebarPanelText = _CurrentSpeziProperties.SearchInputInfoSidebarPanelText;
                var data = ParamterDictionary.Values.Where(p => !string.IsNullOrWhiteSpace(p.Name));
                Item = data.First(i => i.Name == (string)parameter);
            }
        }
        public void OnNavigatedFrom()
        {
            Item.PropertyChanged -= OnPropertyChanged;
        }
    }
}
