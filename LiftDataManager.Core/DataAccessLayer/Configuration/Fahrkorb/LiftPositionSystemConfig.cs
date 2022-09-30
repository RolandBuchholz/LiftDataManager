namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class CarFrameTypeConfig : BaseModelBuilder<CarFrameType>
{
    public override void Configure(EntityTypeBuilder<CarFrameType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}