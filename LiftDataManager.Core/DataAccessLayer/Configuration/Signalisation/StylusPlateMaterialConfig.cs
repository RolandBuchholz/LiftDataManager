using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class StylusPlateMaterialConfig : BaseModelBuilder<StylusPlateMaterial>
{
    public override void Configure(EntityTypeBuilder<StylusPlateMaterial> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}