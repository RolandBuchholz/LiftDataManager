﻿using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class DivisionBarConfig : BaseModelBuilder<DivisionBar>
{
    public override void Configure(EntityTypeBuilder<DivisionBar> builder)
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
        builder.Property(x => x.WeightPerMeter)
               .IsRequired();
        builder.Property(x => x.Height)
               .IsRequired();
        builder.Property(x => x.Thickness)
               .IsRequired();
    }
}