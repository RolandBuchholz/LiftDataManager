namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class PersonsTable8Config : BaseModelBuilder<PersonsTable8>
{
    public override void Configure(EntityTypeBuilder<PersonsTable8> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Persons)
               .IsRequired();
        builder.Property(x => x.Area)
               .IsRequired();
    }
}
