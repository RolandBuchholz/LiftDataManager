namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class FireClosureConfig : BaseModelBuilder<BuildingType>
{
    public override void Configure(EntityTypeBuilder<BuildingType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
    }
}