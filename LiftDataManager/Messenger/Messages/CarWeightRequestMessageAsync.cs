using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Messenger.Messages;
public sealed class CarWeightRequestMessageAsync : AsyncRequestMessage<CalculatedValues>
{
    public KabinengewichtViewModel Sender { get; set; }
    public CarWeightRequestMessageAsync()
    {
        Sender = App.GetService<KabinengewichtViewModel>();
        Sender.IsActive = true;
    }
}
