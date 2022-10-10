using LiftDataManager.Core.DataAccessLayer.Models.Kabine;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Kabine;

public class HandrailConfig : BaseModelBuilder<Handrail>
{
    public override void Configure(EntityTypeBuilder<Handrail> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.WeightPerMeter)
               .IsRequired();
    }
}