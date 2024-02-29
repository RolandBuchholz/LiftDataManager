using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarFloorSurfaceConfig : BaseModelBuilder<CarFloorSurface>
{
    public override void Configure(EntityTypeBuilder<CarFloorSurface> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(20)
                    .IsRequired();
    }
}