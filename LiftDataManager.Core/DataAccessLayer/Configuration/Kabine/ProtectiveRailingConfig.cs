using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class ProtectiveRailingConfig : BaseModelBuilder<ProtectiveRailing>
{
    public override void Configure(EntityTypeBuilder<ProtectiveRailing> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(20)
                    .IsRequired();
    }
}