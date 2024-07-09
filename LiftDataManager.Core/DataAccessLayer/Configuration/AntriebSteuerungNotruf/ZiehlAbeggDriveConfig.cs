using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class ZiehlAbeggDriveConfig : BaseModelBuilder<ZiehlAbeggDrive>
{
    public override void Configure(EntityTypeBuilder<ZiehlAbeggDrive> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.TypeExaminationCertificateId);
    }
}