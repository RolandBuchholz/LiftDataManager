using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class GuideRailsConfig : BaseModelBuilder<GuideRails>
{
    public override void Configure(EntityTypeBuilder<GuideRails> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
        builder.Property(x => x.UsageAsCarRail);
        builder.Property(x => x.UsageAsCwtRail);
    }
}