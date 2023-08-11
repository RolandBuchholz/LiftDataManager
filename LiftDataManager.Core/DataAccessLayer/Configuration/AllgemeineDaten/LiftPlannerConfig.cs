namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class LiftPlannerConfig : BaseModelBuilder<LiftPlanner>
{
    public override void Configure(EntityTypeBuilder<LiftPlanner> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.FirstName)
               .HasMaxLength(50);
        builder.Property(x => x.Company)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.Street)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.StreetNumber)
               .HasMaxLength(10);
        builder.Property(x => x.PhoneNumber)
               .HasMaxLength(25);
        builder.Property(x => x.MobileNumber)
               .HasMaxLength(25);
        builder.Property(x => x.EmailAddress)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.ZipCodeId);
    }
}
