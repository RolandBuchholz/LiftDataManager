using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class ControlCabinetSizeMaterialConfig : BaseModelBuilder<ControlCabinetSizeMaterial>
{
    public override void Configure(EntityTypeBuilder<ControlCabinetSizeMaterial> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}