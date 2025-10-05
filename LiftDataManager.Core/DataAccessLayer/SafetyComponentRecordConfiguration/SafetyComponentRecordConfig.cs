using LiftDataManager.Core.DataAccessLayer.Configuration;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordConfiguration;

public class SafetyComponentRecordConfig : BaseModelBuilder<SafetyComponentRecord>
{
    public override void Configure(EntityTypeBuilder<SafetyComponentRecord> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.IncompleteRecord);
        builder.Property(x => x.SchindlerCertified);
    }
}
