using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarPanelConfig : BaseModelBuilder<CarPanel>
{
    public override void Configure(EntityTypeBuilder<CarPanel> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.WeightPerSquareMeter)
               .IsRequired();
    }
}