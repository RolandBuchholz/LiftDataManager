namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class ParameterCategoryConfig : BaseModelBuilder<ParameterCategory>
{
    public override void Configure(EntityTypeBuilder<ParameterCategory> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.HasMany(t => t.ParameterDtos)
               .WithOne(g => g.ParameterCategory)
               .HasForeignKey(t => t.ParameterCategoryId);
    }
}
