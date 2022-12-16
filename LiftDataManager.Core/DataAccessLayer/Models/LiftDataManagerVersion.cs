namespace LiftDataManager.Core.DataAccessLayer.Models;

public class LiftDataManagerVersion : BaseEntity
{
    public required string VersionsNumber { get; set; }
    public DateTime VersionsDate { get; set; }
    public required string VersionDescription { get; set; }
    public required string Author { get; set; }
}

