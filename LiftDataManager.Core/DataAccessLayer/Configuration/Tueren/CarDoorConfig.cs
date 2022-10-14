using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class CarDoorConfig : BaseModelBuilder<CarDoor>
{
    public override void Configure(EntityTypeBuilder<CarDoor> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.Manufacturer)
               .HasMaxLength(30)
               .IsRequired();
        builder.Property(x => x.SillWidth);
        builder.Property(x => x.MinimalMountingSpace);
        builder.Property(x => x.DoorPanelWidth);
        builder.Property(x => x.DoorPanelSpace);
        builder.Property(x => x.DoorPanelCount);
        builder.Property(x => x.TypeExaminationCertificateId);
    }
}


