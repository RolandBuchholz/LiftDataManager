using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarMaterialConfig : BaseModelBuilder<CarMaterial>
{
    public override void Configure(EntityTypeBuilder<CarMaterial> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(20)
                    .IsRequired();
        builder.Property(x => x.FrontBackWalls)
               .IsRequired();
        builder.Property(x => x.SideWalls)
               .IsRequired();
    }
}