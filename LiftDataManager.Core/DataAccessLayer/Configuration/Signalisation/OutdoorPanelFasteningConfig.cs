﻿using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class OutdoorPanelFasteningConfig : BaseModelBuilder<OutdoorPanelFastening>
{
    public override void Configure(EntityTypeBuilder<OutdoorPanelFastening> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}