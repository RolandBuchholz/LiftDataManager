namespace LiftDataManager.Core.DataAccessLayer.Models.Kabine;

public class CarMaterial : BaseEntity
{
    public string? Name { get; set; }
    public bool FrontBackWalls { get; set; }
    public bool SideWalls { get; set; }
}
