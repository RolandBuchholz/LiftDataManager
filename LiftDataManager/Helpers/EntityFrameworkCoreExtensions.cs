using System.Reflection;

namespace LiftDataManager.Helpers;
public static partial class EntityFrameworkCoreExtensions
{
    static readonly MethodInfo? SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
    public static IQueryable? Query(this DbContext context, string entityName)
    {
        var entity = context.Model.FindEntityType(entityName);

        if (entity is not null)
        {
            return context.Query(entity.ClrType);
        }
        return null;
    }
    public static IQueryable? Query(this DbContext context, Type entityType)
    {
        if (SetMethod is not null)
        {
            return (IQueryable?)SetMethod?.MakeGenericMethod(entityType).Invoke(context, null);
        }
        return null;
    }
       
}