using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class OverspeedGovernorConfig : BaseModelBuilder<OverspeedGovernor>
{
    public override void Configure(EntityTypeBuilder<OverspeedGovernor> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}