using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class BufferPropProfileConfig : BaseModelBuilder<BufferPropProfile>
{
    public override void Configure(EntityTypeBuilder<BufferPropProfile> builder)
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
        builder.Property(x => x.AreaOfProfile).IsRequired();
        builder.Property(x => x.MomentOfInertiaX).IsRequired();
        builder.Property(x => x.MomentOfInertiaY).IsRequired();
        builder.Property(x => x.RadiusOfInertiaX).IsRequired();
        builder.Property(x => x.RadiusOfInertiaY).IsRequired();
        builder.Property(x => x.CenterOfGravityAxisX).IsRequired();
        builder.Property(x => x.CenterOfGravityAxisY).IsRequired();
    }
}