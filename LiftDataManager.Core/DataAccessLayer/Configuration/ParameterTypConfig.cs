namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class ParameterTypConfig : BaseModelBuilder<ParameterTyp>
{
    public override void Configure(EntityTypeBuilder<ParameterTyp> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(20)
                    .IsRequired();
        builder.HasMany(t => t.ParameterDtos)
               .WithOne(g => g.ParameterTyp)
               .HasForeignKey(t => t.ParameterTypId);
    }
}
