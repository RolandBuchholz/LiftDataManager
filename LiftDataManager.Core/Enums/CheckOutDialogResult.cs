using System.ComponentModel.DataAnnotations;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// CheckOut dialog results with german humanized Displayname
/// </summary>
public enum CheckOutDialogResult
{
    /// <summary>
    /// CheckOut successful increase revision with german humanized Displayname
    /// </summary>
    [Display(Name = "CheckOut erfolgreich neue Revision")]
    SuccessfulIncreaseRevision = 0,

    /// <summary>
    /// CheckOut successful no revision change with german humanized Displayname
    /// </summary>
    [Display(Name = "CheckOut erfolgreich kleine Änderung")]
    SuccessfulNoRevisionChange = 1,

    /// <summary>
    //// CheckOut failed with german humanized Displayname
    /// </summary>
    [Display(Name = "CheckOut fehlgeschlagen")]
    CheckOutFailed = 2,

    /// <summary>
    /// work readonly with german humanized Displayname
    /// </summary>
    [Display(Name = "Schreibgeschützt arbeiten")]
    ReadOnly = 3,
}