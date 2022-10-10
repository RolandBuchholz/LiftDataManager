using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarFloorSheetConfig : BaseModelBuilder<CarFloorSheet>
{
    public override void Configure(EntityTypeBuilder<CarFloorSheet> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(3)
                    .IsRequired();
    }
}