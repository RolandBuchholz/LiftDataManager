using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class CarFrameBaseTypeConfig : BaseModelBuilder<CarFrameBaseType>
{
    public override void Configure(EntityTypeBuilder<CarFrameBaseType> builder)
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
        builder.HasMany(t => t.CarFrameTypes)
               .WithOne(g => g.CarFrameBaseType)
               .HasForeignKey(t => t.CarFrameBaseTypeId);
    }
}