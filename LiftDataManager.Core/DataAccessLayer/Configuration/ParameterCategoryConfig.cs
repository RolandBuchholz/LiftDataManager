namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class ParameterCategoryConfig : BaseModelBuilder<ParameterCategory>
{
    public override void Configure(EntityTypeBuilder<ParameterCategory> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();   
    }
}
