namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class LiftTypeConfig : BaseModelBuilder<LiftType>
{
    public override void Configure(EntityTypeBuilder<LiftType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.DriveTypeId);
        builder.Property(x => x.CargoTypeId);
    }
}