﻿using static LiftDataManager.Core.Models.ParameterBase;

namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class ParameterDtoConfig : BaseModelBuilder<ParameterDto>
{
    public override void Configure(EntityTypeBuilder<ParameterDto> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.ParameterTypId);
        builder.Property(x => x.ParameterCategoryId);
        builder.Property(x => x.ParameterTypeCodeId);
        builder.Property(x => x.Name)
                 .HasMaxLength(50)
                 .IsRequired();
        builder.Property(x => x.DisplayName)
                 .HasMaxLength(50)
                 .IsRequired();
        builder.Property(x => x.Value);
        builder.Property(x => x.IsKey);
        builder.Property(x => x.DefaultUserEditable);
        builder.Property(x => x.Comment)
                 .IsRequired()
                 .HasMaxLength(50);
        //parameter.Property(x => x.DropDownList);
    }
}
