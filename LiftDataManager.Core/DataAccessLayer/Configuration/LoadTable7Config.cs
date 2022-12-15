namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class LoadTable7Config : BaseModelBuilder<LoadTable7>
{
    public override void Configure(EntityTypeBuilder<LoadTable7> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Load)
               .IsRequired();
        builder.Property(x => x.Area)
               .IsRequired();
    }
}
