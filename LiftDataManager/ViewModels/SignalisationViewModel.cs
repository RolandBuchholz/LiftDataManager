﻿using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Contracts.Services;
using LiftDataManager.Contracts.ViewModels;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger.Messages;

namespace LiftDataManager.ViewModels
{
    public class SignalisationViewModel : DataViewModelBase, INavigationAware
    {
        public SignalisationViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
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
        }

        public void OnNavigatedTo(object parameter)
        {
            if (_CurrentSpeziProperties.ParamterDictionary.Values is not null) _ = CheckUnsavedParametresAsync();
        }

        public void OnNavigatedFrom()
        {
            WeakReferenceMessenger.Default.Unregister<ParameterDirtyMessage>(this);
        }
    }
}
