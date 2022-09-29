namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class ParameterTypConfig : BaseModelBuilder<ParameterTyp>
{
    public override void Configure(EntityTypeBuilder<ParameterTyp> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}
