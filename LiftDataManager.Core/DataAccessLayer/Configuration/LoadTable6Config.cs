namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class LoadTable6Config : BaseModelBuilder<LoadTable6>
{
    public override void Configure(EntityTypeBuilder<LoadTable6> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Load)
               .IsRequired();
        builder.Property(x => x.Area)
               .IsRequired();
    }
}
