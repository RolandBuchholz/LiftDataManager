using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class ControlCabinetPositionConfig : BaseModelBuilder<ControlCabinetPosition>
{
    public override void Configure(EntityTypeBuilder<ControlCabinetPosition> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}