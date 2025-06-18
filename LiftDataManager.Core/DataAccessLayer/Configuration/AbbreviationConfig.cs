namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class AbbreviationConfig : BaseModelBuilder<Abbreviation>
{
    public override void Configure(EntityTypeBuilder<Abbreviation> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.ShortName)
               .IsRequired();
    }
}
