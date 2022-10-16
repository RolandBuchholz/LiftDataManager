﻿using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class SafetyGearModelTypeConfig : BaseModelBuilder<SafetyGearModelType>
{
    public override void Configure(EntityTypeBuilder<SafetyGearModelType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
        builder.Property(x => x.SafetyGearTypeId);
        builder.Property(x => x.TypeExaminationCertificateId);
    }
}