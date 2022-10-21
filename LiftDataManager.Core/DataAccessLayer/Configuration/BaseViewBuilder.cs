namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public abstract class BaseViewBuilder<TModel> : IEntityTypeConfiguration<TModel> where TModel : ViewEntity
{
    public virtual void Configure(EntityTypeBuilder<TModel> builder)
    {
        builder.HasNoKey();
        builder.ToView(typeof(TModel).Name + "s");
    }
}