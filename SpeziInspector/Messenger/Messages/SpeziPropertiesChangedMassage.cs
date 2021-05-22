using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace SpeziInspector.Messenger.Messages
{
    class SpeziPropertiesChangedMassage : ValueChangedMessage<CurrentSpeziProperties>
    {
        public SpeziPropertiesChangedMassage(CurrentSpeziProperties value) : base(value)
        {
        }
    }
}
