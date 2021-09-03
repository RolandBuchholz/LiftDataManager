using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SpeziInspector.Core.Messenger.Messages
{
    public class ParameterDirtyMessage : ValueChangedMessage<ParameterChangeInfo>
    {
        public ParameterDirtyMessage(ParameterChangeInfo value) : base(value)
        {
        }
    }
}
