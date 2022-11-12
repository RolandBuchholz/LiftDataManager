using LiftDataManager.Core.DataAccessLayer.Models.Schacht;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Schacht;

public class MachineRoomPositionConfig : BaseModelBuilder<MachineRoomPosition>
{
    public override void Configure(EntityTypeBuilder<MachineRoomPosition> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}