﻿using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class GlassPanelShadowJointConfig : BaseModelBuilder<GlassPanelShadowJoint>
{
    public override void Configure(EntityTypeBuilder<GlassPanelShadowJoint> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.DisplayName)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.IsFavorite);
        builder.Property(x => x.IsObsolete);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.OrderSelection);
    }
}