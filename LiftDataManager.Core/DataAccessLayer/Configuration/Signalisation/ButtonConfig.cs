using LiftDataManager.Core.DataAccessLayer.Models.Signalisation;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Signalisation;

public class ButtonConfig : BaseModelBuilder<Button>
{
    public override void Configure(EntityTypeBuilder<Button> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}