using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Core.Messenger.Messages;

public class RefreshModelStateMessage : ValueChangedMessage<ModelStateParameters>
{
    public RefreshModelStateMessage(ModelStateParameters value) : base(value)
    {
    }
}
