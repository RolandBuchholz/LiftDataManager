namespace LiftDataManager.Core.DataAccessLayer.Configuration.Schacht;

public class RailBracketFixingConfig : BaseModelBuilder<RailBracketFixing>
{
    public override void Configure(EntityTypeBuilder<RailBracketFixing> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}