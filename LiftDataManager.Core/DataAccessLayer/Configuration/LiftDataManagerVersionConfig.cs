namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class LiftDataManagerVersionConfig : BaseModelBuilder<LiftDataManagerVersion>
{
    public override void Configure(EntityTypeBuilder<LiftDataManagerVersion> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.Property(x => x.VersionsNumber)
               .IsRequired();
        builder.Property(x => x.VersionsDate)
               .IsRequired();
        builder.Property(x => x.VersionDescription)
               .IsRequired();
        builder.Property(x => x.Author)
               .IsRequired();
    }
}
