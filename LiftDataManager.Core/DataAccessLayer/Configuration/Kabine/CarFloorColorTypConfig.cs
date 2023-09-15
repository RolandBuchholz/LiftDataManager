using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarFloorColorTypConfig : BaseModelBuilder<CarFloorColorTyp>
{
    public override void Configure(EntityTypeBuilder<CarFloorColorTyp> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.Property(x => x.Image);
    }
}