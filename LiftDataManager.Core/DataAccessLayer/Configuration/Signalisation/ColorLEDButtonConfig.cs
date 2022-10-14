using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class ColorLEDButtonConfig : BaseModelBuilder<ColorLEDButton>
{
    public override void Configure(EntityTypeBuilder<ColorLEDButton> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}