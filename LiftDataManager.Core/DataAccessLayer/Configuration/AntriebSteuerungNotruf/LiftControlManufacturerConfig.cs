using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class LiftControlManufacturerConfig : BaseModelBuilder<LiftControlManufacturer>
{
    public override void Configure(EntityTypeBuilder<LiftControlManufacturer> builder)
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
        builder.Property(x => x.DetectionDistance)
               .HasMaxLength(3)
               .IsRequired();
        builder.Property(x => x.DetectionDistanceSIL3)
               .HasMaxLength(3)
               .IsRequired();
        builder.Property(x => x.DeadTime)
               .HasMaxLength(3)
               .IsRequired();
        builder.Property(x => x.DeadTimeZAsbc4)
               .HasMaxLength(3)
               .IsRequired();
        builder.Property(x => x.DeadTimeSIL3)
               .HasMaxLength(3)
               .IsRequired();
        builder.Property(x => x.Speeddetector)
               .IsRequired();
        builder.Property(x => x.SpeeddetectorSIL3)
               .IsRequired();
    }
}