namespace LiftDataManager.Core.Models;
public class TransferData
{
    public TransferData(string name, string value, string comment, bool isKey)
    {
        Name = name;
        Value = value;
        Comment = comment;
        IsKey = isKey;
    }

    public string Name { get; set; }
    public string Value { get; set; }
    public string Comment;
    public bool IsKey;
}
