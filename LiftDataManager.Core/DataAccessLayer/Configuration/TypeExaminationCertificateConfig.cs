namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class TypeExaminationCertificateConfig : BaseModelBuilder<TypeExaminationCertificate>
{
    public override void Configure(EntityTypeBuilder<TypeExaminationCertificate> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.CertificateNumber)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.ManufacturerName)
               .HasMaxLength(50)
               .IsRequired();
        builder.HasMany(t => t.SafetyGearModelTypes)
               .WithOne(g => g.TypeExaminationCertificate)
               .HasForeignKey(t => t.TypeExaminationCertificateId);
        builder.HasMany(t => t.OverspeedGovernors)
               .WithOne(g => g.TypeExaminationCertificate)
               .HasForeignKey(t => t.TypeExaminationCertificateId);
        builder.HasMany(t => t.LiftPositionSystems)
               .WithOne(g => g.TypeExaminationCertificate)
               .HasForeignKey(t => t.TypeExaminationCertificateId);
        builder.HasMany(t => t.CarDoors)
               .WithOne(g => g.TypeExaminationCertificate)
               .HasForeignKey(t => t.TypeExaminationCertificateId);
        builder.HasMany(t => t.ShaftDoors)
               .WithOne(g => g.TypeExaminationCertificate)
               .HasForeignKey(t => t.TypeExaminationCertificateId);
    }
}
