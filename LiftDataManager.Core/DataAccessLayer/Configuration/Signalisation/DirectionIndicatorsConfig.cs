using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class DirectionIndicatorsConfig : BaseModelBuilder<DirectionIndicators>
{
    public override void Configure(EntityTypeBuilder<DirectionIndicators> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}