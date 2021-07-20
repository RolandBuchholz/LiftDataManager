using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SpeziInspector.Core.Messenger.Messages
{
    public class ParameterDirtyMessage : ValueChangedMessage<bool>
    {
        public ParameterDirtyMessage(bool value) : base(value)
        {
        }
    }
}
