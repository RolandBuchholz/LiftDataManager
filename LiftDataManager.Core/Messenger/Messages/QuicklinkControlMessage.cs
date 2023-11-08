using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Core.Messenger.Messages;

public class QuicklinkControlMessage : ValueChangedMessage<QuickLinkControlParameters>
{
    public QuicklinkControlMessage(QuickLinkControlParameters value) : base(value)
    {
    }
}
