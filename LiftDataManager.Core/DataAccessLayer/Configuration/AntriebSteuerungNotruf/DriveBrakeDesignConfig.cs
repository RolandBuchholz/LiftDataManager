using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class DriveBrakeDesignConfig : BaseModelBuilder<DriveBrakeDesign>
{
    public override void Configure(EntityTypeBuilder<DriveBrakeDesign> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}