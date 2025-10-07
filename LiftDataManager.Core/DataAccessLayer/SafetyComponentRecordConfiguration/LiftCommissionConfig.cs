using LiftDataManager.Core.DataAccessLayer.Configuration;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordConfiguration;

public class LiftCommissionConfig : BaseModelBuilder<LiftCommission>
{
    public override void Configure(EntityTypeBuilder<LiftCommission> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.SAISEquipment)
               .HasMaxLength(12);
        builder.HasMany(t => t.SafetyComponentRecords)
               .WithOne(g => g.LiftCommission)
               .HasForeignKey(t => t.LiftCommissionId);
    }
}
