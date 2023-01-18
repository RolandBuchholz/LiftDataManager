namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class DatabaseTableValueModificationConfig : BaseModelBuilder<DatabaseTableValueModification>
{
    public override void Configure(EntityTypeBuilder<DatabaseTableValueModification> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.Property(x => x.Timestamp)
               .IsRequired();
        builder.Property(x => x.TableName)
               .IsRequired();
        builder.Property(x => x.Operation)
               .HasMaxLength(10)
               .IsRequired();
        builder.Property(x => x.EntityId)
               .IsRequired();
        builder.Property(x => x.EntityName)
               .IsRequired();
        builder.Property(x => x.NewEntityValue);
    }
}