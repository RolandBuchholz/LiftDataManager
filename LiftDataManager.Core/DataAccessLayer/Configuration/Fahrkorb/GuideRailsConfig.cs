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
        builder.Property(x => x.DisplayName)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.IsFavorite);
        builder.Property(x => x.IsObsolete);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.OrderSelection);
        builder.Property(x => x.UsageAsCarRail);
        builder.Property(x => x.UsageAsCwtRail);
        builder.Property(x => x.Machined);
        builder.Property(x => x.RailHead);
        builder.Property(x => x.Height);
        builder.Property(x => x.Width);
        builder.Property(x => x.Area);
        builder.Property(x => x.MomentOfInertiaX);
        builder.Property(x => x.MomentOfInertiaY);
        builder.Property(x => x.ModulusOfResistanceX);
        builder.Property(x => x.ModulusOfResistanceY);
        builder.Property(x => x.FlangeC);
        builder.Property(x => x.RadiusOfInertiaX);
        builder.Property(x => x.RadiusOfInertiaY);
        builder.Property(x => x.ThicknessF);
        builder.Property(x => x.ForgedClips)
               .IsRequired();
        builder.Property(x => x.ForgedClipsForce);
        builder.Property(x => x.SlidingClips)
               .IsRequired();
        builder.Property(x => x.SlidingClipsForce);
        builder.Property(x => x.Weight);
        builder.Property(x => x.Material)
               .IsRequired();
    }
}