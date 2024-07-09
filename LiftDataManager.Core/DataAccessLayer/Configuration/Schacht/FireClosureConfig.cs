﻿using LiftDataManager.Core.DataAccessLayer.Models.Schacht;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Schacht;

public class FireClosureConfig : BaseModelBuilder<FireClosure>
{
    public override void Configure(EntityTypeBuilder<FireClosure> builder)
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