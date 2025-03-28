﻿namespace LiftDataManager.Core.Messenger;

public class CurrentSpeziProperties
{
    public bool Adminmode { get; set; }
    public bool VaultDisabled { get; set; }
    public bool CustomAccentColor { get; set; }
    public bool AuftragsbezogeneXml { get; set; }
    public bool CheckOut { get; set; }
    public bool LikeEditParameter { get; set; }
    public bool HideInfoErrors { get; set; }
    public SpezifikationTyp? CurrentSpezifikationTyp { get; set; }
    public string? FullPathXml { get; set; }
    public string? SearchInput { get; set; }
}