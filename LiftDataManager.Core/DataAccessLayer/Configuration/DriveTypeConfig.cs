namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class DriveTypeConfig : BaseModelBuilder<Models.DriveType>
{
    public override void Configure(EntityTypeBuilder<Models.DriveType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.HasMany(t => t.CarFrameTypes)
               .WithOne(g => g.DriveType)
               .HasForeignKey(t => t.DriveTypeId);
        builder.HasMany(t => t.LiftTypes)
               .WithOne(g => g.DriveType)
               .HasForeignKey(t => t.DriveTypeId);
    }
}
