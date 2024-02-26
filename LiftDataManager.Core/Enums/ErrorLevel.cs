using System.ComponentModel.DataAnnotations;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// ErrorLevels with humanized Displayname
/// </summary>
public enum ErrorLevel
{
    /// <summary>
    /// ErrorLevel valid with german humanized Displayname
    /// </summary>
    [Display(Name = "Gültig")]
    Valid,

    /// <summary>
    /// ErrorLevel informational with german humanized Displayname
    /// </summary>
    [Display(Name = "Information")]
    Informational,

    /// <summary>
    /// ErrorLevel warning with german humanized Displayname
    /// </summary>
    [Display(Name = "Warnung")]
    Warning,

    /// <summary>
    /// ErrorLevel error with german humanized Displayname
    /// </summary>
    [Display(Name = "Fehler")]
    Error
}