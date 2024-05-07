using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class DivisionBarConfig : BaseModelBuilder<DivisionBar>
{
    public override void Configure(EntityTypeBuilder<DivisionBar> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.WeightPerMeter)
               .IsRequired();
        builder.Property(x => x.Height)
               .IsRequired();
        builder.Property(x => x.Thickness)
               .IsRequired();
    }
}