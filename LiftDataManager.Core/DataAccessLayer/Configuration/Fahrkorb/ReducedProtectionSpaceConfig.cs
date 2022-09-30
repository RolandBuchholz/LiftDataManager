using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class ReducedProtectionSpaceConfig : BaseModelBuilder<ReducedProtectionSpace>
{
    public override void Configure(EntityTypeBuilder<ReducedProtectionSpace> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}