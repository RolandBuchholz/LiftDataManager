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
        builder.Property(x => x.Manufacturer);
        builder.Property(x => x.SillMountTyp);
    }
}