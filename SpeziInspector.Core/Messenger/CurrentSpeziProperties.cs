﻿using Cogs.Collections;
using SpeziInspector.Core.Models;

namespace SpeziInspector.Core.Messenger
{
    public class CurrentSpeziProperties
    {
        public bool Adminmode { get; set; }
        public bool AuftragsbezogeneXml { get; set; }
        public string FullPathXml { get; set; }
        public string SearchInput { get; set; }
        public ObservableDictionary<string, Parameter> ParamterDictionary { get; set; }
    }
}