using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Core.Messenger.Messages;
public sealed class CarWeightRequestMessageAsync : AsyncRequestMessage<CalculatedValues>
{
    public KabinengewichtViewModel Sender
    {
        get; set;
    }

    public CarWeightRequestMessageAsync()
    {
        Sender = new KabinengewichtViewModel()
        {
            IsActive = true
        };
    }
}
