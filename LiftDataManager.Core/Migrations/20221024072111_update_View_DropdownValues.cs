using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class update_View_DropdownValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW IF NOT EXISTS DropdownValues(Base , Name) AS 
                    SELECT 'AntiDrums',
                       Name
                  FROM AntiDrums
                UNION ALL
                SELECT 'BellPositions' AS Base,
                       Name
                  FROM BellPositions
                UNION ALL
                SELECT 'BuildingTypes' AS Base,
                       Name
                  FROM BuildingTypes
                UNION ALL
                SELECT 'Buttons' AS Base,
                       Name
                  FROM Buttons
                UNION ALL
                SELECT 'ButtonStylusPlates' AS Base,
                       Name
                  FROM ButtonStylusPlates
                UNION ALL
                SELECT 'CarCoverPanels' AS Base,
                       Name
                  FROM CarCoverPanels
                UNION ALL
                SELECT 'CarDoors' AS Base,
                       Name
                  FROM CarDoors
                UNION ALL
                SELECT 'CarFloorings' AS Base,
                       Name
                  FROM CarFloorings
                UNION ALL
                SELECT 'CarFlooringsSpecialSheet' AS Base,
       				   Name
  				  FROM CarFloorings
  				  WHERE SpecialSheet = 1
				UNION ALL
                SELECT 'CarFloorProfiles' AS Base,
                       Name
                  FROM CarFloorProfiles
                UNION ALL
                SELECT 'CarFloorSheets' AS Base,
                       Name
                  FROM CarFloorSheets
                UNION ALL
                SELECT 'CarFloorTypes' AS Base,
                       Name
                  FROM CarFloorTypes
                UNION ALL
                SELECT 'CarFrameBaseTypes' AS Base,
                       Name
                  FROM CarFrameBaseTypes
                UNION ALL
                SELECT 'CarFrameTypes' AS Base,
                       Name
                  FROM CarFrameTypes
                UNION ALL
                SELECT 'CargoTypes' AS Base,
                       Name
                  FROM CargoTypes
                UNION ALL
                SELECT 'CarLightings' AS Base,
                       Name
                  FROM CarLightings
                UNION ALL
                SELECT 'CarPanelFittings' AS Base,
                       Name
                  FROM CarPanelFittings
                UNION ALL
                SELECT 'CarPanels' AS Base,
                       Name
                  FROM CarPanels
                UNION ALL
                SELECT 'CENumbers' AS Base,
                       Name
                  FROM CENumbers
                UNION ALL
                SELECT 'Coatings' AS Base,
                       Name
                  FROM Coatings
                UNION ALL
                SELECT 'ColorDisplays' AS Base,
                       Name
                  FROM ColorDisplays
                UNION ALL
                SELECT 'ColorLEDButtons' AS Base,
                       Name
                  FROM ColorLEDButtons
                UNION ALL
                SELECT 'ControlCabinetPositions' AS Base,
                       Name
                  FROM ControlCabinetPositions
                UNION ALL
                SELECT 'ControlCabinetSizes' AS Base,
                       Name
                  FROM ControlCabinetSizes
                UNION ALL
                SELECT 'ControlTypes' AS Base,
                       Name
                  FROM ControlTypes
                UNION ALL
                SELECT 'DeliveryTypes' AS Base,
                       Name
                  FROM DeliveryTypes
                UNION ALL
                SELECT 'DirectionIndicatorss' AS Base,
                       Name
                  FROM DirectionIndicatorss
                UNION ALL
                SELECT 'Displays' AS Base,
                       Name
                  FROM Displays
                UNION ALL
               	SELECT 'DriveSystems' AS Base,
                       Name
                  FROM DriveSystems
                UNION ALL
               SELECT 'DriveSystemTypes' AS Base,
                           Name
                  FROM DriveSystemTypes
                UNION ALL
                SELECT 'DriveTypes' AS Base,
                       Name
                  FROM DriveTypes
                UNION ALL
                SELECT 'ElevatorStandards' AS Base,
                       Name
                  FROM ElevatorStandards
                UNION ALL
                SELECT 'EmergencyCallButtons' AS Base,
                       Name
                  FROM EmergencyCallButtons
                UNION ALL
                SELECT 'EmergencyConnections' AS Base,
                       Name
                  FROM EmergencyConnections
                UNION ALL
                SELECT 'EmergencyDevices' AS Base,
                       Name
                  FROM EmergencyDevices
                UNION ALL
                SELECT 'EmergencyHotlines' AS Base,
                       Name
                  FROM EmergencyHotlines
                UNION ALL
                SELECT 'FireClosureBys' AS Base,
                       Name
                  FROM FireClosureBys
                UNION ALL
                SELECT 'FireClosures' AS Base,
                       Name
                  FROM FireClosures
                UNION ALL
                SELECT 'GoodsLiftStandards' AS Base,
                       Name
                  FROM GoodsLiftStandards
                UNION ALL
                SELECT 'GuideModelTypes' AS Base,
                       Name
                  FROM GuideModelTypes
                UNION ALL
                SELECT 'GuideRailsCarRail' AS Base,
                       Name
                  FROM GuideRailss
                  WHERE UsageAsCarRail = 1
                UNION ALL
                SELECT 'GuideRailsCwtRail' AS Base,
                       Name
                  FROM GuideRailss
                  WHERE UsageAsCwtRail = 1
                UNION ALL
                SELECT 'GuideRailsStatuss' AS Base,
                       Name
                  FROM GuideRailsStatuss
                UNION ALL
                SELECT 'GuideTypes' AS Base,
                       Name
                  FROM GuideTypes
                UNION ALL
                SELECT 'Handrails' AS Base,
                       Name
                  FROM Handrails
                UNION ALL
                SELECT 'InstallationInfos' AS Base,
                       Name
                  FROM InstallationInfos
                UNION ALL
                SELECT 'LiftCarRoofs' AS Base,
                       Name
                  FROM LiftCarRoofs
                UNION ALL
                SELECT 'LiftCarTypes' AS Base,
                       Name
                  FROM LiftCarTypes
                UNION ALL
                SELECT 'LiftControlManufacturers' AS Base,
                       Name
                  FROM LiftControlManufacturers
                UNION ALL
                SELECT 'LiftDoorControls' AS Base,
                       Name
                  FROM LiftDoorControls
                UNION ALL
                SELECT 'LiftDoorGroups' AS Base,
                       Name
                  FROM LiftDoorGroups
                UNION ALL
                SELECT 'LiftDoorGuards' AS Base,
                       Name
                  FROM LiftDoorGuards
                UNION ALL
                SELECT 'LiftDoorLockingDevices' AS Base,
                       Name
                  FROM LiftDoorLockingDevices
                UNION ALL
                SELECT 'LiftDoorOpeningDirections' AS Base,
                       Name
                  FROM LiftDoorOpeningDirections
                UNION ALL
                SELECT 'LiftDoorSills' AS Base,
                       Name
                  FROM LiftDoorSills
                UNION ALL
                SELECT 'LiftDoorStandards' AS Base,
                       Name
                  FROM LiftDoorStandards
                UNION ALL
                SELECT 'LiftDoorTypes' AS Base,
                       Name
                  FROM LiftDoorTypes
                UNION ALL
                SELECT 'LiftPositionSystems' AS Base,
                       Name
                  FROM LiftPositionSystems
                UNION ALL
                SELECT 'LiftTypes' AS Base,
                       Name
                  FROM LiftTypes
                UNION ALL
                SELECT 'LightFieldSizes' AS Base,
                       Name
                  FROM LightFieldSizes
                UNION ALL
                SELECT 'LoadingDevices' AS Base,
                       Name
                  FROM LoadingDevices
                UNION ALL
                SELECT 'LoadWeighingDevices' AS Base,
                       Name
                  FROM LoadWeighingDevices
                UNION ALL
                SELECT 'MachineRoomPositions' AS Base,
                       Name
                  FROM MachineRoomPositions
                UNION ALL
                SELECT 'MaterialSurfacesCarMaterialFrontBackWalls' AS Base,
                       Name
                  FROM MaterialSurfaces
                 WHERE CarMaterialFrontBackWalls = 1
                UNION ALL
                SELECT 'MaterialSurfacesCarMaterialSideWalls' AS Base,
                       Name
                  FROM MaterialSurfaces
                 WHERE CarMaterialSideWalls = 1
                UNION ALL
                SELECT 'MaterialSurfacesCarPanelMaterial' AS Base,
                       Name
                  FROM MaterialSurfaces
                 WHERE CarPanelMaterial = 1
                UNION ALL
                SELECT 'MaterialSurfacesLiftDoorMaterial' AS Base,
                       Name
                  FROM MaterialSurfaces
                 WHERE LiftDoorMaterial = 1
                UNION ALL
                SELECT 'MaterialSurfacesControlCabinetMaterial' AS Base,
                       Name
                  FROM MaterialSurfaces
                 WHERE ControlCabinetMaterial = 1
                UNION ALL
                SELECT 'MaterialThicknesss' AS Base,
                       Name
                  FROM MaterialThicknesss
                UNION ALL
                SELECT 'Mirrors' AS Base,
                       Name
                  FROM Mirrors
                UNION ALL
                SELECT 'OutdoorPanelFastenings' AS Base,
                       Name
                  FROM OutdoorPanelFastenings
                UNION ALL
                SELECT 'OverspeedGovernors' AS Base,
                       Name
                  FROM OverspeedGovernors
                UNION ALL
                SELECT 'PowerSupplys' AS Base,
                       Name
                  FROM PowerSupplys
                UNION ALL
                SELECT 'RailBracketFixings' AS Base,
                       Name
                  FROM RailBracketFixings
                UNION ALL
                SELECT 'RammingProtections' AS Base,
                       Name
                  FROM RammingProtections
                UNION ALL
                SELECT 'ReducedProtectionSpaces' AS Base,
                       Name
                  FROM ReducedProtectionSpaces
                UNION ALL
                SELECT 'SafetyGearModelTypes' AS Base,
                       Name
                  FROM SafetyGearModelTypes
                UNION ALL
                SELECT 'SafetyGearTypes' AS Base,
                       Name
                  FROM SafetyGearTypes
                UNION ALL
                SELECT 'ShaftDoors' AS Base,
                       Name
                  FROM ShaftDoors
                UNION ALL
                SELECT 'ShaftFrameFieldFillings' AS Base,
                       Name
                  FROM ShaftFrameFieldFillings
                UNION ALL
                SELECT 'ShaftTypes' AS Base,
                       Name
                  FROM ShaftTypes
                UNION ALL
                SELECT 'SkirtingBoards' AS Base,
                       Name
                  FROM SkirtingBoards
                UNION ALL
                SELECT 'SmokeExtractionShafts' AS Base,
                       Name
                  FROM SmokeExtractionShafts
                UNION ALL
                SELECT 'StylusPlateMaterials' AS Base,
                       Name
                  FROM StylusPlateMaterials
                UNION ALL
                SELECT 'VandalResistants' AS Base,
                       Name
                  FROM VandalResistants
                UNION ALL
                SELECT 'WallMaterials' AS Base,
                       Name
                  FROM WallMaterials;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            drop view DropdownValues;
            ");
        }
    }
}
