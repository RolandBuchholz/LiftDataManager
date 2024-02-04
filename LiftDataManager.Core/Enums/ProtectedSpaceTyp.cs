using System.ComponentModel.DataAnnotations;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// ProtectedSpaceTyps according to EN 81:20 with german humanized Displayname
/// </summary>
public enum ProtectedSpaceTyp
{
    /// <summary>
    /// ProtectedSpaceTyp according to EN 81:20 Typ1 with german humanized Displayname
    /// </summar
    [Display(Name = "Typ1 (Aufrecht)")]
    Typ1 = 1,

    /// <summary>
    /// ProtectedSpaceTyp according to EN 81:20 Typ2 with german humanized Displayname
    /// </summar
    /// 
    [Display(Name = "Typ2 (Hockend)")]
    Typ2 = 2,

    /// <summary>
    /// ProtectedSpaceTyp according to EN 81:20 Typ3 with german humanized Displayname
    /// </summar
    [Display(Name = "Typ3 (Liegend)")]
    Typ3 = 3
}
