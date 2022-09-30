using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class LoadWeighingDeviceConfig : BaseModelBuilder<LoadWeighingDevice>
{
    public override void Configure(EntityTypeBuilder<LoadWeighingDevice> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}