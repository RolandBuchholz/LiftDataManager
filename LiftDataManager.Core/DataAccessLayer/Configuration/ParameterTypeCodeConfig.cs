namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class ParameterTypeCodeConfig : BaseModelBuilder<ParameterTypeCode>
{
    public override void Configure(EntityTypeBuilder<ParameterTypeCode> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(20)
                    .IsRequired();
        builder.HasMany(t => t.ParameterDtos)
               .WithOne(g => g.ParameterTypeCode)
               .HasForeignKey(t => t.ParameterTypeCodeId);
    }
}
