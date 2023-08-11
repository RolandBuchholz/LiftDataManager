namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class ZipCodeConfig : BaseModelBuilder<ZipCode>
{
    public override void Configure(EntityTypeBuilder<ZipCode> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.Property(x => x.ZipCodeNumber)
               .HasMaxLength(10)
               .IsRequired();
        builder.HasMany(t => t.LiftPlanners)
               .WithOne(g => g.ZipCode)
               .HasForeignKey(t => t.ZipCodeId);
        builder.Property(x => x.CountryId);
    }
}
