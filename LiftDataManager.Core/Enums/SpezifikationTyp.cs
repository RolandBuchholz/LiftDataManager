using System.ComponentModel.DataAnnotations;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// SpezifikationTyps with german humanized Displayname
/// </summary>
public enum SpezifikationTyp
{
    /// <summary>
    /// SpezifikationTyp order with german humanized Displayname
    /// </summary>
    [Display(Name = "Auftrag")]
    order,

    /// <summary>
    /// SpezifikationTyp offer with german humanized Displayname
    /// </summary>
    [Display(Name = "Angebot")]
    offer,

    /// <summary>
    /// SpezifikationTyp planning with german humanized Displayname
    /// </summary>
    [Display(Name = "Vorplanung")]
    planning,

    /// <summary>
    /// SpezifikationTyp request with german humanized Displayname
    /// </summary>
    [Display(Name = "Anfrage Formular")]
    request
}
