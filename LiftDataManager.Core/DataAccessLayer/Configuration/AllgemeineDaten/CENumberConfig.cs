namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class CENumberConfig : BaseModelBuilder<CENumber>
{
    public override void Configure(EntityTypeBuilder<CENumber> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}