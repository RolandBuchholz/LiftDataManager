using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorControlConfig : BaseModelBuilder<LiftDoorControl>
{
    public override void Configure(EntityTypeBuilder<LiftDoorControl> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}