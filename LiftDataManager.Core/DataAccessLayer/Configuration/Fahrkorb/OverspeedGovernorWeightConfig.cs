﻿using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class OverspeedGovernorWeightConfig : BaseModelBuilder<OverspeedGovernorWeight>
{
    public override void Configure(EntityTypeBuilder<OverspeedGovernorWeight> builder)
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