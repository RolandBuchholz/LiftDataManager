using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class SafetyGearModelTypeConfig : BaseModelBuilder<SafetyGearModelType>
{
    public override void Configure(EntityTypeBuilder<SafetyGearModelType> builder)
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
        builder.Property(x => x.SafetyGearTypeId);
        builder.Property(x => x.TypeExaminationCertificateId);
        builder.Property(x => x.MinLoadOiledColddrawn);
        builder.Property(x => x.MaxLoadOiledColddrawn);
        builder.Property(x => x.MinLoadDryColddrawn);
        builder.Property(x => x.MaxLoadDryColddrawn);
        builder.Property(x => x.MinLoadOiledMachined);
        builder.Property(x => x.MaxLoadOiledMachined);
        builder.Property(x => x.MinLoadDryMachined);
        builder.Property(x => x.MaxLoadDryMachined);
        builder.Property(x => x.AllowableWidth);
    }
}