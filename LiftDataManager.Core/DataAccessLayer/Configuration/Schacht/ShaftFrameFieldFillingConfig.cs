using LiftDataManager.Core.DataAccessLayer.Models.Schacht;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Schacht;

public class ShaftFrameFieldFillingConfig : BaseModelBuilder<ShaftFrameFieldFilling>
{
    public override void Configure(EntityTypeBuilder<ShaftFrameFieldFilling> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}