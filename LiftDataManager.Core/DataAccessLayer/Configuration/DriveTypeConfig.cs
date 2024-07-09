namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class DriveTypeConfig : BaseModelBuilder<Models.DriveType>
{
    public override void Configure(EntityTypeBuilder<Models.DriveType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.DisplayName)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.IsFavorite);
        builder.Property(x => x.IsObsolete);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.OrderSelection);
        builder.HasMany(t => t.CarFrameTypes)
               .WithOne(g => g.DriveType)
               .HasForeignKey(t => t.DriveTypeId);
        builder.HasMany(t => t.LiftTypes)
               .WithOne(g => g.DriveType)
               .HasForeignKey(t => t.DriveTypeId);
    }
}
