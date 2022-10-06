using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class GuideModelTypeConfig : BaseModelBuilder<GuideModelType>
{
    public override void Configure(EntityTypeBuilder<GuideModelType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.GuideTypeId);
    }
}