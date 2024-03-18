using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class BufferPropMaterialConfig : BaseModelBuilder<BufferPropMaterial>
{
    public override void Configure(EntityTypeBuilder<BufferPropMaterial> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}