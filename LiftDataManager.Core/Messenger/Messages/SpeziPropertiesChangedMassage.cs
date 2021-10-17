using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Core.Messenger.Messages
{
    public class SpeziPropertiesChangedMassage : ValueChangedMessage<CurrentSpeziProperties>
    {
        public SpeziPropertiesChangedMassage(CurrentSpeziProperties value) : base(value)
        {
        }
    }
}
