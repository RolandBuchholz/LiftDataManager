using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class CarPanelMaterialConfig : BaseModelBuilder<CarPanelMaterial>
{
    public override void Configure(EntityTypeBuilder<CarPanelMaterial> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}