using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorGroupConfig : BaseModelBuilder<LiftDoorGroup>
{
    public override void Configure(EntityTypeBuilder<LiftDoorGroup> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.CarDoorId);
        builder.Property(x => x.ShaftDoorId);
        builder.Property(x => x.DoorManufacturer)
               .HasMaxLength(20);
    }
}