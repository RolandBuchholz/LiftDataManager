using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Core.Messenger.Messages;
public sealed class CarFrameWeightRequestMessageAsync : AsyncRequestMessage<CalculatedValues>
{
    public BausatzViewModel Sender
    {
        get; set;
    }

    public CarFrameWeightRequestMessageAsync()
    {
        Sender = new BausatzViewModel()
        {
            IsActive = true
        };
    }
}
