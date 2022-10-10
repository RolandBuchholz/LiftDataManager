using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class LiftCarTypeConfig : BaseModelBuilder<LiftCarType>
{
    public override void Configure(EntityTypeBuilder<LiftCarType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(20)
                    .IsRequired();
    }
}