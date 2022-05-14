using Cogs.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Messenger;
using LiftDataManager.Core.Messenger.Messages;
using LiftDataManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftDataManager.ViewModels
{
    public class DataViewModelBase : ObservableRecipient
    {
        private string _InfoSidebarPanelText;
        public bool Adminmode { get; set; }
        public bool AuftragsbezogeneXml { get; set; }
        public bool CheckOut { get; set; }
        public bool LikeEditParameter { get; set; }
        public string FullPathXml { get; set; }

        public CurrentSpeziProperties _CurrentSpeziProperties;
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }

        public DataViewModelBase()
        {
            _CurrentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
            if (_CurrentSpeziProperties.FullPathXml is not null) FullPathXml = _CurrentSpeziProperties.FullPathXml;
            if (_CurrentSpeziProperties.ParamterDictionary is not null) ParamterDictionary = _CurrentSpeziProperties.ParamterDictionary;
            Adminmode = _CurrentSpeziProperties.Adminmode;
            AuftragsbezogeneXml = _CurrentSpeziProperties.AuftragsbezogeneXml;
            CheckOut = _CurrentSpeziProperties.CheckOut;
            LikeEditParameter = _CurrentSpeziProperties.LikeEditParameter;
            InfoSidebarPanelText = _CurrentSpeziProperties.InfoSidebarPanelText;
        }

        public string InfoSidebarPanelText
        {
            get => _InfoSidebarPanelText;

            set
            {
                SetProperty(ref _InfoSidebarPanelText, value);
                _CurrentSpeziProperties.InfoSidebarPanelText = value;
                Messenger.Send(new SpeziPropertiesChangedMassage(_CurrentSpeziProperties));
            }
        }
    }
}
