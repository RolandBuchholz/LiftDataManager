using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class SafetyGearTypeConfig : BaseModelBuilder<SafetyGearType>
{
    public override void Configure(EntityTypeBuilder<SafetyGearType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.HasMany(t => t.SafetyGearModelTypes)
               .WithOne(g => g.SafetyGearType)
               .HasForeignKey(t => t.SafetyGearTypeId);
    }
}