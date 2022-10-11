using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class EmergencyConnectionConfig : BaseModelBuilder<EmergencyConnection>
{
    public override void Configure(EntityTypeBuilder<EmergencyConnection> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}