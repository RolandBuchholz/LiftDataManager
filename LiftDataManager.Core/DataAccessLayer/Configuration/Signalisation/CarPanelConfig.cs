using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class CarPanelConfig : BaseModelBuilder<CarPanel>
{
    public override void Configure(EntityTypeBuilder<CarPanel> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.Weight)
               .IsRequired();
        builder.Property(x => x.Weight)
               .IsRequired();
    }
}