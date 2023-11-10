namespace LiftDataManager.Helpers;

public class RevisionHelper
{
    public static string GetNextRevision(string? revision)
    {
        if (string.IsNullOrWhiteSpace(revision))
            return "A";

        var cleanRevision = revision.Trim();
        char[] revCharacters = cleanRevision.ToCharArray();
        int nextChar;
        if (revCharacters.Length == 2)
        {
            if (revCharacters[1] == 'Z')
            {
                nextChar = revCharacters[0] + 1;
                return ((char)nextChar).ToString() + 'A';
            }
            nextChar = revCharacters[1] + 1;
            return revCharacters[0] + ((char)nextChar).ToString();
        }

        if (revCharacters[0] == 'Z')
            return "AA";

        nextChar = revCharacters[0] + 1;
        return ((char)nextChar).ToString();
    }
}
