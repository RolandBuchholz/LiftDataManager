using LiftDataManager.Core.DataAccessLayer.Configuration;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordConfiguration;

public class SafetyComponentManufacturerConfig : BaseModelBuilder<SafetyComponentManufacturer>
{
    public override void Configure(EntityTypeBuilder<SafetyComponentManufacturer> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.ZIPCode);
        builder.Property(x => x.City);
        builder.Property(x => x.Country);
        builder.HasMany(t => t.SafetyComponentRecords)
               .WithOne(g => g.SafetyComponentManufacturer)
               .HasForeignKey(t => t.SafetyComponentManufacturerId);
    }
}
