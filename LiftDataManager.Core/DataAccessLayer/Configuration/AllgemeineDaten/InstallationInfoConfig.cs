namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class InstallationInfoConfig : BaseModelBuilder<InstallationInfo>
{
    public override void Configure(EntityTypeBuilder<InstallationInfo> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}