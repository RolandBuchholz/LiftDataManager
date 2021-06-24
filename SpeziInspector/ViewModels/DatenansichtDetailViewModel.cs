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
        private bool Adminmode;
        private bool AuftragsbezogeneXml;
        public string FullPathXml;
        public ObservableCollection<Parameter> ParamterList { get; set; }
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
            SaveParameter = new RelayCommand(SaveParameterAsync, () => CanSaveParameter && Adminmode && AuftragsbezogeneXml);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckIsDirty((Parameter)sender);
        }

        public IRelayCommand SaveParameter { get; }

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
        private void SaveParameterAsync()
        {
            _parameterDataService.SaveParameterAsync(Item, FullPathXml);
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
                if (_CurrentSpeziProperties.ParamterList is not null) ParamterList = _CurrentSpeziProperties.ParamterList;
                Adminmode = _CurrentSpeziProperties.Adminmode;
                AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
                var data = ParamterList.Where(p => !string.IsNullOrWhiteSpace(p.Name));
                Item = data.First(i => i.Name == (string)parameter);
            }
        }
        public void OnNavigatedFrom()
        {
            Item.PropertyChanged -= OnPropertyChanged;
        }
    }
}
