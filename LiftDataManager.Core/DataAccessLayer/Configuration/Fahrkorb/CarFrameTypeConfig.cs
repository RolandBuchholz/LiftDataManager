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
        builder.Property(x => x.DisplayName)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.IsFavorite);
        builder.Property(x => x.IsObsolete);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.OrderSelection);
        builder.Property(x => x.CarFrameWeight);
        builder.Property(x => x.IsCFPControlled)
               .IsRequired();
        builder.Property(x => x.HasMachineRoom)
               .IsRequired();
        builder.Property(x => x.CFPStartIndex);
        builder.Property(x => x.DriveTypeId);
        builder.Property(x => x.CarFrameBaseTypeId);
        builder.Property(x => x.CFPStartIndex);
        builder.Property(x => x.CarFrameDGB);
        builder.Property(x => x.CounterweightDGB);
        builder.Property(x => x.CarFrameDGBOffset);
        builder.Property(x => x.CarFrametoCWTDGBOffset);
        builder.Property(x => x.CounterweightDGBOffset);
    }
}