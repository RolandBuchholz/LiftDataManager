using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class CarFrameTypeConfig : BaseModelBuilder<CarFrameType>
{
    public override void Configure(EntityTypeBuilder<CarFrameType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
        builder.Property(x => x.DriveType)
               .HasMaxLength(20)
               .IsRequired();
        builder.Property(x => x.CarFrameBaseType)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(x => x.CarFrameWeight);
        builder.Property(x => x.IsCFPControlled)
               .IsRequired();
        builder.Property(x => x.HasMachineRoom)
       .IsRequired();
    }
}