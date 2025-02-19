using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class DriveSafetyBrakeConfig : BaseModelBuilder<DriveSafetyBrake>
{
    public override void Configure(EntityTypeBuilder<DriveSafetyBrake> builder)
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
        builder.Property(x => x.TypeExaminationCertificateId);
        builder.HasMany(t => t.ZiehlAbeggDrives)
               .WithOne(g => g.DriveSafetyBrake)
               .HasForeignKey(t => t.DriveSafetyBrakeId);
    }
}