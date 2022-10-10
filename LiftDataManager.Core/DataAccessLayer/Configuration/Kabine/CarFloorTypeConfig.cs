using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarFloorTypeConfig : BaseModelBuilder<CarFloorType>
{
    public override void Configure(EntityTypeBuilder<CarFloorType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(20)
                    .IsRequired();
    }
}