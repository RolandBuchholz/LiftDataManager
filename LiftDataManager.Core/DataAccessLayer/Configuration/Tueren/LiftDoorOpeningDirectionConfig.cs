using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class LiftDoorOpeningDirectionConfig : BaseModelBuilder<LiftDoorOpeningDirection>
{
    public override void Configure(EntityTypeBuilder<LiftDoorOpeningDirection> builder)
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
        builder.HasMany(t => t.ShaftDoors)
               .WithOne(g => g.LiftDoorOpeningDirection)
               .HasForeignKey(t => t.LiftDoorOpeningDirectionId);
        builder.HasMany(t => t.CarDoors)
               .WithOne(g => g.LiftDoorOpeningDirection)
               .HasForeignKey(t => t.LiftDoorOpeningDirectionId);
    }
}