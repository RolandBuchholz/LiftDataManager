using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class ButtonStylusPlateConfig : BaseModelBuilder<ButtonStylusPlate>
{
    public override void Configure(EntityTypeBuilder<ButtonStylusPlate> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}