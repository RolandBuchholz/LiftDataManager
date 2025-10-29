using LiftDataManager.Core.DataAccessLayer.Configuration;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordConfiguration;

public class SafetyComponentManfacturerConfig : BaseModelBuilder<SafetyComponentManfacturer>
{
    public override void Configure(EntityTypeBuilder<SafetyComponentManfacturer> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.ZIPCode);
        builder.Property(x => x.City);
        builder.Property(x => x.Country);
        builder.HasMany(t => t.SafetyComponentRecords)
               .WithOne(g => g.SafetyComponentManfacturer)
               .HasForeignKey(t => t.SafetyComponentManfacturerId);
    }
}
