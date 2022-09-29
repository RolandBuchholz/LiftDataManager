namespace LiftDataManager.Core.DataAccessLayer.Configuration.Schacht;

public class FireClosureByConfig : BaseModelBuilder<FireClosureBy>
{
    public override void Configure(EntityTypeBuilder<FireClosureBy> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}