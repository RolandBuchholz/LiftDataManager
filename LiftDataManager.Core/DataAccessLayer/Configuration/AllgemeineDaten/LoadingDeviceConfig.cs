namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class LoadingDeviceConfig : BaseModelBuilder<LoadingDevice>
{
    public override void Configure(EntityTypeBuilder<LoadingDevice> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}