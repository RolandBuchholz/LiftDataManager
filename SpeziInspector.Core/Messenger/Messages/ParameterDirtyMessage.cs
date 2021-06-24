using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace SpeziInspector.Messenger.Messages
{
    public class ParameterDirtyMessage : ValueChangedMessage<bool>
    {
        public ParameterDirtyMessage(bool value) : base(value)
        {
        }

    }
}
