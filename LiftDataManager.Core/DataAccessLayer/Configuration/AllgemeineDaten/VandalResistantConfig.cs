namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class VandalResistantConfig : BaseModelBuilder<VandalResistant>
{
    public override void Configure(EntityTypeBuilder<VandalResistant> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}
