﻿using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorOpeningDirectionConfig : BaseModelBuilder<LiftDoorOpeningDirection>
{
    public override void Configure(EntityTypeBuilder<LiftDoorOpeningDirection> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
        builder.HasMany(t => t.ShaftDoors)
               .WithOne(g => g.LiftDoorOpeningDirection)
               .HasForeignKey(t => t.LiftDoorOpeningDirectionId);
        builder.HasMany(t => t.CarDoors)
               .WithOne(g => g.LiftDoorOpeningDirection)
               .HasForeignKey(t => t.LiftDoorOpeningDirectionId);
    }
}