namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class CargoTypeConfig : BaseModelBuilder<CargoType>
{
    public override void Configure(EntityTypeBuilder<CargoType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.Property(x => x.DisplayName)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.IsFavorite);
        builder.Property(x => x.IsObsolete);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.OrderSelection);
        builder.HasMany(t => t.LiftTypes)
               .WithOne(g => g.CargoType)
               .HasForeignKey(t => t.CargoTypeId);
    }
}