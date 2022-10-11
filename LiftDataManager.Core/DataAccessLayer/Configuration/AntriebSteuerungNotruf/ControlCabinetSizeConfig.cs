using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class ControlCabinetSizeConfig : BaseModelBuilder<ControlCabinetSize>
{
    public override void Configure(EntityTypeBuilder<ControlCabinetSize> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}