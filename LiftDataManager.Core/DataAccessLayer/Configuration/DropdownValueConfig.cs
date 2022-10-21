namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class DropdownValueConfig : BaseViewBuilder<DropdownValue>
{
    public override void Configure(EntityTypeBuilder<DropdownValue> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name);
        builder.Property(x => x.Base);
    }
}
