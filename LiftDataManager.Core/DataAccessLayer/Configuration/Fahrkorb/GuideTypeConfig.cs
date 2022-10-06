using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class GuideTypeConfig : BaseModelBuilder<GuideType>
{
    public override void Configure(EntityTypeBuilder<GuideType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
        builder.HasMany(t => t.GuideModelTypes)
            .WithOne(g => g.GuideType)
            .HasForeignKey(t => t.GuideTypeId);
    }
}