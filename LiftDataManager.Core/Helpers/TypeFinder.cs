using System.Reflection;

namespace LiftDataManager.Core.Helpers;

public class TypeFinder
{
    public static Type? FindType(string name)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var result = (from elem in from app in assemblies
                                   select (from tip in app.GetTypes()
                                           where tip.Name == name.Trim()
                                           select tip).FirstOrDefault()
                      where elem != null
                      select elem).FirstOrDefault();
        return result;
    }
    public static Type? FindLiftmanagerDBModelType(string name)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName is not null && x.FullName.StartsWith("LiftDataManager.Core"));
        return assemblies?.FirstOrDefault()?.GetTypes().Where(x => x.Name == name.Trim())
                                            .FirstOrDefault(x => x.FullName is not null && x.FullName.StartsWith("LiftDataManager.Core.DataAccessLayer"));
    }
}
