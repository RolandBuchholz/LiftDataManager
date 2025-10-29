using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorTelescopicApronConfig : BaseModelBuilder<LiftDoorTelescopicApron>
{
    public override void Configure(EntityTypeBuilder<LiftDoorTelescopicApron> builder)
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
        builder.Property(x => x.SAISDescription)
                               .HasMaxLength(50);
        builder.Property(x => x.SAISIdentificationNumber)
                               .HasMaxLength(50);
    }
}