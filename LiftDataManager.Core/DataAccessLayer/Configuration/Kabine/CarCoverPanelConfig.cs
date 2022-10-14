using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarCoverPanelConfig : BaseModelBuilder<CarCoverPanel>
{
    public override void Configure(EntityTypeBuilder<CarCoverPanel> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.WeightPerSquareMeter)
               .IsRequired();
    }
}