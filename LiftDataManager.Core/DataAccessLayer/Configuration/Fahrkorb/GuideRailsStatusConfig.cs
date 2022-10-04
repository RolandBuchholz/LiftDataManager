using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class GuideRailsStatusConfig : BaseModelBuilder<GuideRailsStatus>
{
    public override void Configure(EntityTypeBuilder<GuideRailsStatus> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}