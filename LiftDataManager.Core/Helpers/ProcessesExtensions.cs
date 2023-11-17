namespace LiftDataManager.Core.Helpers;

static class ProcessesExtensions
{
    public static bool IsLocked(this FileInfo f)
    {
        try
        {
            var fpath = f.FullName;
            FileStream fs = File.OpenWrite(fpath);
            fs.Close();
            return false;
        }

        catch (Exception) { return true; }
    }
}
