using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorMaterialConfig : BaseModelBuilder<LiftDoorMaterial>
{
    public override void Configure(EntityTypeBuilder<LiftDoorMaterial> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}