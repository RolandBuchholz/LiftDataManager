using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;
using System;

namespace LiftDataManager.ViewModels
{
    public class SchachtViewModel : DataViewModelBase, INavigationAware
    {


        public SchachtViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
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

            SetHauptzugangCommand = new RelayCommand<string>(SetHauptzugang);
            ResetHauptzugangCommand = new RelayCommand(ResetHauptzugang);
        }

        public IRelayCommand SetHauptzugangCommand { get; }
        public IRelayCommand ResetHauptzugangCommand { get; }

        private void SetHauptzugang(string parameter)
        {
            if (!string.Equals(ParamterDictionary["var_Haupthaltestelle"].Value , parameter, StringComparison.OrdinalIgnoreCase))
            {
                ParamterDictionary["var_Haupthaltestelle"].Value = parameter;
                DisplayNameHauptzugang = parameter;
            }
        }

        private void ResetHauptzugang()
        {
            if (!string.Equals(ParamterDictionary["var_Haupthaltestelle"].Value, "NV", StringComparison.OrdinalIgnoreCase))
            {
                ParamterDictionary["var_Haupthaltestelle"].Value = "NV";
                DisplayNameHauptzugang = "NV";
            }
        }

        private string _DisplayNameHauptzugang;
        public string DisplayNameHauptzugang
        {
            get => ParamterDictionary["var_Haupthaltestelle"].Value == "NV"
                    ? (_DisplayNameHauptzugang = "Kein Hauptzugang gewählt")
                    : (_DisplayNameHauptzugang = ParamterDictionary["var_Haupthaltestelle"].Value.Replace("ZG_", ""));
            set => SetProperty(ref _DisplayNameHauptzugang, value);
        }

        public void OnNavigatedTo(object parameter)
        {
            SynchronizeViewModelParameter();
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = CheckUnsavedParametresAsync();
        }

        public void OnNavigatedFrom()
        {
            WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
        }
    }
}
