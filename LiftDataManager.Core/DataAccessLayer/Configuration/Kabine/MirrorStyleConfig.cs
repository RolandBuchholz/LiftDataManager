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
    }
}