using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class CarPanelFittingConfig : BaseModelBuilder<CarPanelFitting>
{
    public override void Configure(EntityTypeBuilder<CarPanelFitting> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}