namespace LiftDataManager.Core.DataAccessLayer.Models;

public class DatabaseTableValueModification : BaseEntity
{
    public required string Timestamp { get; set; }
    public required string TableName { get; set; }
    public required string Operation { get; set; }
    public int EntityId { get; set; } 
    public required string EntityName { get; set; }
    public string? NewEntityValue { get; set;}
}
