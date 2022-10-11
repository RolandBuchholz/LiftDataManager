using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class EmergencyCallButtonConfig : BaseModelBuilder<EmergencyCallButton>
{
    public override void Configure(EntityTypeBuilder<EmergencyCallButton> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}