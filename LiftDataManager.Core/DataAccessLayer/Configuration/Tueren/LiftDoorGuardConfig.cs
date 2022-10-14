using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorGuardConfig : BaseModelBuilder<LiftDoorGuard>
{
    public override void Configure(EntityTypeBuilder<LiftDoorGuard> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}