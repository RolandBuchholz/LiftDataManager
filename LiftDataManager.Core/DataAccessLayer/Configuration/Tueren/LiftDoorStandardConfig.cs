using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorStandardConfig : BaseModelBuilder<LiftDoorStandard>
{
    public override void Configure(EntityTypeBuilder<LiftDoorStandard> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}