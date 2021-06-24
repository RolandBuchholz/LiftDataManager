using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace SpeziInspector.Messenger.Messages
{
    public class SpeziPropertiesChangedMassage : ValueChangedMessage<CurrentSpeziProperties>
    {
        public SpeziPropertiesChangedMassage(CurrentSpeziProperties value) : base(value)
        {
        }
    }
}
