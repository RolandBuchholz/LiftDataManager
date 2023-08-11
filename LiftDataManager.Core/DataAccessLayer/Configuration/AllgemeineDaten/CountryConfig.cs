namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class CountryConfig : BaseModelBuilder<Country>
{
    public override void Configure(EntityTypeBuilder<Country> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(25)
               .IsRequired();
        builder.Property(x => x.ShortMark)
               .HasMaxLength(3);
        builder.HasMany(t => t.ZipCodes)
               .WithOne(g => g.Country)
               .HasForeignKey(t => t.CountryId);
    }
}
