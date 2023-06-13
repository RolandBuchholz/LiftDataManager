using LiftDataManager.Core.DataAccessLayer.Models.Schacht;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Schacht;

public class PitLadderConfig : BaseModelBuilder<PitLadder>
{
    public override void Configure(EntityTypeBuilder<PitLadder> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}