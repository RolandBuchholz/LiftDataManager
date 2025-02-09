namespace LiftDataManager.Core.Models;
public class TransferData(string name, string value, string comment, bool isKey)
{
    public string Name { get; set; } = name;
    public string Value { get; set; } = value;
    public string Comment = comment;
    public bool IsKey = isKey;
}
