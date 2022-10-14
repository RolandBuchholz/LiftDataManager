using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorLockingDeviceConfig : BaseModelBuilder<LiftDoorLockingDevice>
{
    public override void Configure(EntityTypeBuilder<LiftDoorLockingDevice> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}