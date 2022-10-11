﻿using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class EmergencyDeviceConfig : BaseModelBuilder<EmergencyDevice>
{
    public override void Configure(EntityTypeBuilder<EmergencyDevice> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}