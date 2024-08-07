﻿namespace LiftDataManager.Core.DataAccessLayer.Models;

public class MaterialSurface : SelectionEntity
{
    public bool CarMaterialFrontBackWalls { get; set; }
    public bool CarMaterialSideWalls { get; set; }
    public bool CarPanelMaterial { get; set; }
    public bool ControlCabinetMaterial { get; set; }
    public bool LiftDoorMaterial { get; set; }
    public bool SkirtingBoardMaterial { get; set; }
    public bool BufferPropMaterial { get; set; }
    public bool DivisionBarMaterial { get; set; }
}
