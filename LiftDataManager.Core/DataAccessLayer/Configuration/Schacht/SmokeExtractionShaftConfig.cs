﻿namespace LiftDataManager.Core.DataAccessLayer.Configuration.Schacht;

public class SmokeExtractionShaftConfig : BaseModelBuilder<SmokeExtractionShaft>
{
    public override void Configure(EntityTypeBuilder<SmokeExtractionShaft> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}