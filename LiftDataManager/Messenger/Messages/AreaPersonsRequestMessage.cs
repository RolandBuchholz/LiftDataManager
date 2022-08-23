using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Core.Messenger.Messages;
public sealed class AreaPersonsRequestMessage : RequestMessage<CalculatedValues>
{
    public NutzlastberechnungViewModel Sender
    {
        get; set;
    }

    public AreaPersonsRequestMessage()
    {
        Sender = new NutzlastberechnungViewModel()
        {
            IsActive = true
        };
    }

}
