﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LiftDataManager.ViewModels
{
    public class DatenansichtDetailViewModel : DataViewModelBase, INavigationAware
    {
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

        public DatenansichtDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
             base(parameterDataService, dialogService, navigationService)
        {
            WeakReferenceMessenger.Default.Register<ParameterDirtyMessage>(this, (r, m) =>
            {
                if (m is not null && m.Value.IsDirty)
                {
                    InfoSidebarPanelText += $"{m.Value.ParameterName} : {m.Value.OldValue} => {m.Value.NewValue} geändert \n";
                }
            });
            SaveParameter = new AsyncRelayCommand(SaveParameterAsync, () => CanSaveParameter && Adminmode && CheckOut);
        }

        public IAsyncRelayCommand SaveParameter { get; }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckIsDirty((Parameter)sender);
        }

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

        private async Task SaveParameterAsync()
        {
            var infotext = await _parameterDataService.SaveParameterAsync(Item, FullPathXml);
            InfoSidebarPanelText += infotext;
            CanSaveParameter = false;
            CanSaveParameter = false;
            Item.IsDirty = false;
        }

        public void OnNavigatedTo(object parameter)
        {
            if (parameter is not null)
            {
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
