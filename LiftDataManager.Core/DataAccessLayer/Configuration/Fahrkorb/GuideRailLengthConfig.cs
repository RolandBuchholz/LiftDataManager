﻿using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class GuideRailLengthConfig : BaseModelBuilder<GuideRailLength>
{
    public override void Configure(EntityTypeBuilder<GuideRailLength> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.RailLength);
    }
}