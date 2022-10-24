using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.AntriebSteuerungNotruf;

public class DriveSystemTypeConfig : BaseModelBuilder<DriveSystemType>
{
    public override void Configure(EntityTypeBuilder<DriveSystemType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
        builder.HasMany(t => t.DriveSystems)
               .WithOne(g => g.DriveSystemType)
               .HasForeignKey(t => t.DriveSystemTypeId);
    }
}