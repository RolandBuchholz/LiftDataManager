namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class GoodsLiftStandardConfig : BaseModelBuilder<GoodsLiftStandard>
{
        public override void Configure(EntityTypeBuilder<GoodsLiftStandard> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.Name)
                        .HasMaxLength(50)
                        .IsRequired();
        }
}
