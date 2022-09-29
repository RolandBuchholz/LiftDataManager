using LiftDataManager.Core.DataAccessLayer.Models.Schacht;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Schacht;

public class WallMaterialConfig : BaseModelBuilder<WallMaterial>
{
    public override void Configure(EntityTypeBuilder<WallMaterial> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}