namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class CargoTypeConfig : BaseModelBuilder<CargoType>
{
    public override void Configure(EntityTypeBuilder<CargoType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(20)
                    .IsRequired();
        builder.HasMany(t => t.LiftTypes)
            .WithOne(g => g.CargoType)
            .HasForeignKey(t => t.CargoTypeId);
    }
}