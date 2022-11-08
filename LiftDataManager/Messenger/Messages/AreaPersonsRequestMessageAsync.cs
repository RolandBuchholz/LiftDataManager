﻿using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LiftDataManager.Core.Messenger.Messages;
public sealed class AreaPersonsRequestMessageAsync : AsyncRequestMessage<CalculatedValues>
{
    public NutzlastberechnungViewModel Sender {get; set;}

    public AreaPersonsRequestMessageAsync()
    {
        Sender = App.GetService<NutzlastberechnungViewModel>();
        Sender.IsActive = true;
    }
}
