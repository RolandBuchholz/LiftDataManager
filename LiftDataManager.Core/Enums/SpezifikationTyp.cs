using Ardalis.SmartEnum;

namespace LiftDataManager.Core.Enums;
/// <summary>
/// SpezifikationTyps with german humanized Displayname
/// </summary>

public sealed class SpezifikationTyp : SmartEnum<SpezifikationTyp, ushort>
{
    //    /// <summary>
    //    /// SpezifikationTyp order with german humanized Displayname
    //    /// </summary>
    public static readonly SpezifikationTyp Order = new("Auftrag", 1);

    //    /// <summary>
    //    /// SpezifikationTyp offer with german humanized Displayname
    //    /// </summary>
    public static readonly SpezifikationTyp Offer = new("Angebot", 2);

    //    /// <summary>
    //    /// SpezifikationTyp planning with german humanized Displayname
    //    /// </summary>
    public static readonly SpezifikationTyp Planning = new("Vorplanung", 3);

    //    /// <summary>
    //    /// SpezifikationTyp request with german humanized Displayname
    //    /// </summary>
    public static readonly SpezifikationTyp Request = new("Anfrageformular", 4);

    //    /// <summary>
    //    /// SpezifikationTyp Mailrequest with german humanized Displayname
    //    /// </summary>
    public static readonly SpezifikationTyp MailRequest = new("Mailanfrage", 5);

    private SpezifikationTyp(string name, ushort value) : base(name, value)
    {
    }
}