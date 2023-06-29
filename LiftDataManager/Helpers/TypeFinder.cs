using System.Reflection;

namespace LiftDataManager.Helpers;

public class TypeFinder
{
    public static Type? FindType(string name)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var result = (from elem in (from app in assemblies
                                    select (from tip in app.GetTypes()
                                            where tip.Name == name.Trim()
                                            select tip).FirstOrDefault())
                      where elem != null
                      select elem).FirstOrDefault();
        return result;
    }
    public static Type? FindLiftmanagerType(string name)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName!.StartsWith("LiftDataManager"));

        var result = (from elem in (from app in assemblies
                                    select (from tip in app.GetTypes()
                                            where tip.Name == name.Trim()
                                            select tip).FirstOrDefault())
                      where elem != null
                      select elem).FirstOrDefault();
        return result;
    }
}
