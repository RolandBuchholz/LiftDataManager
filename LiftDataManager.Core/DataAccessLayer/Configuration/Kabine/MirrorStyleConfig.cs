using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class MirrorStyleConfig : BaseModelBuilder<MirrorStyle>
{
    public override void Configure(EntityTypeBuilder<MirrorStyle> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(30)
               .IsRequired();
        builder.Property(x => x.DisplayName)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.IsFavorite);
        builder.Property(x => x.IsObsolete);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.OrderSelection);
    }
}