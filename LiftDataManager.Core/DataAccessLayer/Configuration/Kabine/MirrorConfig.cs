using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class MirrorConfig : BaseModelBuilder<Mirror>
{
    public override void Configure(EntityTypeBuilder<Mirror> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.Property(x => x.WeightPerSquareMeter)
               .IsRequired();
    }
}