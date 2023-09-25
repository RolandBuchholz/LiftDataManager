using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class HydraulicLockConfig : BaseModelBuilder<HydraulicLock>
{
    public override void Configure(EntityTypeBuilder<HydraulicLock> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}