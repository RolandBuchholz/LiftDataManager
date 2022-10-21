using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarFlooringConfig : BaseModelBuilder<CarFlooring>
{
    public override void Configure(EntityTypeBuilder<CarFlooring> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.WeightPerSquareMeter)
               .IsRequired();
        builder.Property(x => x.Thickness)
               .IsRequired();
        builder.Property(x => x.SpecialSheet);
    }
}