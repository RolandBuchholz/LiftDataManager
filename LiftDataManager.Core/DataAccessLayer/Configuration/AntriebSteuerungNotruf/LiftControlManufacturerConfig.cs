using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class LiftControlManufacturerConfig : BaseModelBuilder<LiftControlManufacturer>
{
    public override void Configure(EntityTypeBuilder<LiftControlManufacturer> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}