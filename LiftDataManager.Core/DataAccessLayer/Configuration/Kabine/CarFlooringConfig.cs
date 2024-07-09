using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class CarFlooringConfig : BaseModelBuilder<CarFlooring>
{
    public override void Configure(EntityTypeBuilder<CarFlooring> builder)
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
        builder.Property(x => x.WeightPerSquareMeter)
               .IsRequired();
        builder.Property(x => x.Thickness)
               .IsRequired();
        builder.Property(x => x.SpecialSheet);
        builder.HasMany(t => t.CarFloorColorTyps)
               .WithOne(g => g.CarFlooring)
               .HasForeignKey(t => t.CarFloorId);
    }
}