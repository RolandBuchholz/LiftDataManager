using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class LightFieldSizeConfig : BaseModelBuilder<LightFieldSize>
{
    public override void Configure(EntityTypeBuilder<LightFieldSize> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}