using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class ControlTypeConfig : BaseModelBuilder<ControlType>
{
    public override void Configure(EntityTypeBuilder<ControlType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}