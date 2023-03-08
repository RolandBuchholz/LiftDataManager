using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class LiftInverterTypeConfig : BaseModelBuilder<LiftInverterType>
{
    public override void Configure(EntityTypeBuilder<LiftInverterType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
        builder.Property(x => x.MaxFuseSize);
    }
}