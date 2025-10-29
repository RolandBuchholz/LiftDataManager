using LiftDataManager.Core.DataAccessLayer.Configuration;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

namespace LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordConfiguration;

public class SafetyComponentRecordConfig : BaseModelBuilder<SafetyComponentRecord>
{
    public override void Configure(EntityTypeBuilder<SafetyComponentRecord> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.LiftCommissionId);
        builder.Property(x => x.CompleteRecord);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.Release);
        builder.Property(x => x.Revision);
        builder.Property(x => x.IdentificationNumber);
        builder.Property(x => x.SerialNumber);
        builder.Property(x => x.BatchNumber);
        builder.Property(x => x.SafetyComponentManfacturerId);
        builder.Property(x => x.Imported);
        builder.Property(x => x.CreationDate);
        builder.Property(x => x.Active);
    }
}
