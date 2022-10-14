using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class ColorDisplayConfig : BaseModelBuilder<ColorDisplay>
{
    public override void Configure(EntityTypeBuilder<ColorDisplay> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}