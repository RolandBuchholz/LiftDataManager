using System.ComponentModel.DataAnnotations;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// Shaft and car sides with german humanized Displayname
/// </summary>
public enum CarSide
{
    /// <summary>
    /// Shaft and car side A with german humanized Displayname
    /// </summary>
    [Display(Name = "Wand A")]
    A = 1,

    /// <summary>
    //// Shaft and car side B with german humanized Displayname
    /// </summary>
    [Display(Name = "Wand B")]
    B = 2,

    /// <summary>
    /// Shaft and car side C with german humanized Displayname
    /// </summary>
    [Display(Name = "Wand C")]
    C = 3,

    /// <summary>
    /// Shaft and car side D with german humanized Displayname
    /// </summary>
    [Display(Name = "Wand D")]
    D = 4
}
