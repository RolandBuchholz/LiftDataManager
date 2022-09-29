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
        builder.Property(x => x.ProductId);
        builder.Property(x => x.ProductName)
            .HasMaxLength(50);
    }
}
