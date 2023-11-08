using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Core.Messenger.Messages;

public class SpeziPropertiesChangedMessage : ValueChangedMessage<CurrentSpeziProperties>
{
    public SpeziPropertiesChangedMessage(CurrentSpeziProperties value) : base(value)
    {
    }
}
