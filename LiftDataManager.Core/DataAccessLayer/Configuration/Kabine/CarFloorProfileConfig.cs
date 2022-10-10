using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarFloorProfileConfig : BaseModelBuilder<CarFloorProfile>
{
    public override void Configure(EntityTypeBuilder<CarFloorProfile> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.Property(x => x.Height)
               .IsRequired();
        builder.Property(x => x.WeightPerMeter)
               .IsRequired();
    }
}