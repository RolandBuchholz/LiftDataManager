using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class MaterialThicknessConfig : BaseModelBuilder<MaterialThickness>
{
    public override void Configure(EntityTypeBuilder<MaterialThickness> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(5)
                    .IsRequired();
    }
}