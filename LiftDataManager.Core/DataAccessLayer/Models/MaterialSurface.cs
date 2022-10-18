namespace LiftDataManager.Core.DataAccessLayer.Models;

public class MaterialSurface : BaseEntity
{
    public string? Name { get; set; }
    public bool CarMaterialFrontBackWalls { get; set; }
    public bool CarMaterialSideWalls { get; set; }
    public bool CarPanelMaterial { get; set; }
    public bool ControlCabinetMaterial { get; set; }
    public bool LiftDoorMaterial { get; set; }

}
