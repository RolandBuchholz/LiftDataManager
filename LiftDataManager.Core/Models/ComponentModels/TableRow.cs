namespace LiftDataManager.Core.Models.ComponentModels;
public class TableRow<T1, T2>
{
    public bool IsSelected { get; set; }
    public T1? FirstValue { get; set; }
    public T2? SecondValue { get; set; }
    public string? FirstUnit { get; set; }
    public string? SecondUnit { get; set; }
}
