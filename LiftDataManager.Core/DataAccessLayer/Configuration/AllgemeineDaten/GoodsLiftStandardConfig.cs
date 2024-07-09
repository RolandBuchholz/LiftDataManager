namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class GoodsLiftStandardConfig : BaseModelBuilder<GoodsLiftStandard>
{
    public override void Configure(EntityTypeBuilder<GoodsLiftStandard> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.DisplayName)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.IsFavorite);
        builder.Property(x => x.IsObsolete);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.OrderSelection);
    }
}
