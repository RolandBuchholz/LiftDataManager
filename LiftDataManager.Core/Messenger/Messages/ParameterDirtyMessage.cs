using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Core.Messenger.Messages
{
    public class ParameterDirtyMessage : ValueChangedMessage<ParameterChangeInfo>
    {
        public ParameterDirtyMessage(ParameterChangeInfo value) : base(value)
        {
        }
    }
}
