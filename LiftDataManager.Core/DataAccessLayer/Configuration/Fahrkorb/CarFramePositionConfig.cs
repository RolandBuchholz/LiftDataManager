using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class CarFramePositionConfig : BaseModelBuilder<CarFramePosition>
{
    public override void Configure(EntityTypeBuilder<CarFramePosition> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(1)
                    .IsRequired();
    }
}