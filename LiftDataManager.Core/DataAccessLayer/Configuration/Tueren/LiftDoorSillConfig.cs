using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorSillConfig : BaseModelBuilder<LiftDoorSill>
{
    public override void Configure(EntityTypeBuilder<LiftDoorSill> builder)
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
        builder.Property(x => x.Manufacturer)
               .HasMaxLength(50);
        builder.Property(x => x.SillFilterTyp)
               .HasMaxLength(50);
        builder.Property(x => x.SillMountTyp);
        builder.Property(x => x.IsVandalResistant);
    }
}