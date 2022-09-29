namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class ParameterTypeCodeConfig : BaseModelBuilder<ParameterTypeCode>
{
    public override void Configure(EntityTypeBuilder<ParameterTypeCode> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}
