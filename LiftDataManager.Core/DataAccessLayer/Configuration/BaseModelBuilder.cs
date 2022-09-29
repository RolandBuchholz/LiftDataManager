namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public abstract class BaseModelBuilder<TModel> : IEntityTypeConfiguration<TModel> where TModel : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable(typeof(TModel).Name + "s");
    }
}