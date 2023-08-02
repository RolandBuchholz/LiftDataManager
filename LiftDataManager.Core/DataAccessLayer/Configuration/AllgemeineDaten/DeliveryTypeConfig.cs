namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class DeliveryTypeConfig : BaseModelBuilder<DeliveryType>
{
    public override void Configure(EntityTypeBuilder<DeliveryType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}
