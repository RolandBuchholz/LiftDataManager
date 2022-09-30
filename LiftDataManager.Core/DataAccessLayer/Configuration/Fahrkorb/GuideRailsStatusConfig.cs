using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class LiftPositionSystemConfig : BaseModelBuilder<LiftPositionSystem>
{
    public override void Configure(EntityTypeBuilder<LiftPositionSystem> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}