using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SpeziInspector.Core.Messenger.Messages
{
    public class SpeziPropertiesChangedMassage : ValueChangedMessage<CurrentSpeziProperties>
    {
        public SpeziPropertiesChangedMassage(CurrentSpeziProperties value) : base(value)
        {
        }
    }
}
