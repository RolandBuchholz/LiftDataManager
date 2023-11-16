using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class DriveSystemPositionConfig : BaseModelBuilder<DriveSystem>
{
    public override void Configure(EntityTypeBuilder<DriveSystem> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.DriveControlTyp)
               .HasMaxLength(50);
        builder.Property(x => x.DriveSystemTypeId);
    }
}