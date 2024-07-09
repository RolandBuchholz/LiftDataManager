using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class FirstSetupNewSelectionParameter3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "WallMaterials",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "WallMaterials",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "WallMaterials",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "WallMaterials",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "WallMaterials",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "VandalResistants",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "VandalResistants",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "VandalResistants",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "VandalResistants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "VandalResistants",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "StylusPlateMaterials",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "StylusPlateMaterials",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "StylusPlateMaterials",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "StylusPlateMaterials",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "StylusPlateMaterials",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SmokeExtractionShafts",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "SmokeExtractionShafts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "SmokeExtractionShafts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "SmokeExtractionShafts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "SmokeExtractionShafts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SkirtingBoards",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "SkirtingBoards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "SkirtingBoards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "SkirtingBoards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "SkirtingBoards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ShaftTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ShaftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ShaftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ShaftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ShaftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ShaftFrameFieldFillings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ShaftFrameFieldFillings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ShaftFrameFieldFillings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ShaftFrameFieldFillings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ShaftFrameFieldFillings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ShaftDoors",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ShaftDoors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ShaftDoors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ShaftDoors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ShaftDoors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SafetyGearTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "SafetyGearTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "SafetyGearTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "SafetyGearTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "SafetyGearTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SafetyGearModelTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ReducedProtectionSpaces",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ReducedProtectionSpaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ReducedProtectionSpaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ReducedProtectionSpaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ReducedProtectionSpaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "RammingProtections",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "RammingProtections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "RammingProtections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "RammingProtections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "RammingProtections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "RailBracketFixings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "RailBracketFixings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "RailBracketFixings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "RailBracketFixings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "RailBracketFixings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ProtectiveRailings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ProtectiveRailings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ProtectiveRailings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ProtectiveRailings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ProtectiveRailings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "PowerSupplys",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "PowerSupplys",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "PowerSupplys",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "PowerSupplys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "PowerSupplys",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "PitLadders",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "PitLadders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "PitLadders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "PitLadders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "PitLadders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "OverspeedGovernors",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "OverspeedGovernors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "OverspeedGovernors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "OverspeedGovernors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "OverspeedGovernors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "OutdoorPanelFastenings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "OutdoorPanelFastenings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "OutdoorPanelFastenings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "OutdoorPanelFastenings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "OutdoorPanelFastenings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "MirrorStyles",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "MirrorStyles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "MirrorStyles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "MirrorStyles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "MirrorStyles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Mirrors",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Mirrors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "Mirrors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "Mirrors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "Mirrors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "MaterialThicknesss",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "MaterialThicknesss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "MaterialThicknesss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "MaterialThicknesss",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "MaterialThicknesss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "MaterialSurfaces",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "MaterialSurfaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "MaterialSurfaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "MaterialSurfaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "MaterialSurfaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "MachineRoomPositions",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "MachineRoomPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "MachineRoomPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "MachineRoomPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "MachineRoomPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LoadWeighingDevices",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LoadWeighingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LoadWeighingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LoadWeighingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LoadWeighingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LoadingDevices",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LoadingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LoadingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LoadingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LoadingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LightFieldSizes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LightFieldSizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LightFieldSizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LightFieldSizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LightFieldSizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftPositionSystems",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftPositionSystems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftPositionSystems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftPositionSystems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftPositionSystems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftDoorTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftDoorTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftDoorTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftDoorTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftDoorTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftDoorStandards",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftDoorStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftDoorStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftDoorStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftDoorStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftDoorSills",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftDoorSills",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftDoorSills",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftDoorSills",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftDoorSills",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftDoorOpeningDirections",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftDoorOpeningDirections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftDoorOpeningDirections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftDoorOpeningDirections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftDoorOpeningDirections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftDoorLockingDevices",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftDoorLockingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftDoorLockingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftDoorLockingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftDoorLockingDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftDoorGuards",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftDoorGuards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftDoorGuards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftDoorGuards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftDoorGuards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftDoorGroups",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftDoorControls",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftDoorControls",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftDoorControls",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftDoorControls",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftDoorControls",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftControlManufacturers",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftCarTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftCarTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftCarTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftCarTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftCarTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftCarRoofs",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftCarRoofs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftCarRoofs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftCarRoofs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftCarRoofs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftBuffers",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftBuffers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftBuffers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftBuffers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftBuffers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "InstallationInfos",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "InstallationInfos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "InstallationInfos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "InstallationInfos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "InstallationInfos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "HydraulicLocks",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "HydraulicLocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "HydraulicLocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "HydraulicLocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "HydraulicLocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Handrails",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Handrails",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "Handrails",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "Handrails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "Handrails",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "GuideTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "GuideTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "GuideTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "GuideTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "GuideTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "GuideRailsStatuss",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "GuideRailsStatuss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "GuideRailsStatuss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "GuideRailsStatuss",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "GuideRailsStatuss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "GuideRailss",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "GuideRailss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "GuideRailss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "GuideRailss",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "GuideRailss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "GuideRailLengths",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "GuideRailLengths",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "GuideRailLengths",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "GuideRailLengths",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "GuideRailLengths",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "GuideModelTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "GuideModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "GuideModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "GuideModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "GuideModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "GoodsLiftStandards",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "GoodsLiftStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "GoodsLiftStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "GoodsLiftStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "GoodsLiftStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "FireClosures",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "FireClosures",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "FireClosures",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "FireClosures",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "FireClosures",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "FireClosureBys",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "FireClosureBys",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "FireClosureBys",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "FireClosureBys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "FireClosureBys",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "EmergencyHotlines",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "EmergencyHotlines",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "EmergencyHotlines",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "EmergencyHotlines",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "EmergencyHotlines",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "EmergencyDevices",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "EmergencyDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "EmergencyDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "EmergencyDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "EmergencyDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "EmergencyConnections",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "EmergencyConnections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "EmergencyConnections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "EmergencyConnections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "EmergencyConnections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "EmergencyCallButtons",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "EmergencyCallButtons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "EmergencyCallButtons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "EmergencyCallButtons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "EmergencyCallButtons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ElevatorStandards",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ElevatorStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ElevatorStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ElevatorStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ElevatorStandards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DriveTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "DriveTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "DriveTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "DriveTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "DriveTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DriveSystemTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "DriveSystemTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "DriveSystemTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "DriveSystemTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "DriveSystemTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DriveSystems",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "DriveSystems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "DriveSystems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "DriveSystems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "DriveSystems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DriveBrakeDesigns",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "DriveBrakeDesigns",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "DriveBrakeDesigns",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "DriveBrakeDesigns",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "DriveBrakeDesigns",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DivisionBars",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "DivisionBars",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "DivisionBars",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "DivisionBars",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "DivisionBars",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Displays",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Displays",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "Displays",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "Displays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "Displays",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DirectionIndicatorss",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "DirectionIndicatorss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "DirectionIndicatorss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "DirectionIndicatorss",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "DirectionIndicatorss",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DeliveryTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "DeliveryTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "DeliveryTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "DeliveryTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "DeliveryTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ControlTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ControlTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ControlTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ControlTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ControlTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ControlCabinetSizes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ControlCabinetSizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ControlCabinetSizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ControlCabinetSizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ControlCabinetSizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ControlCabinetPositions",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ControlCabinetPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ControlCabinetPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ControlCabinetPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ControlCabinetPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ColorLEDButtons",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ColorLEDButtons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ColorLEDButtons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ColorLEDButtons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ColorLEDButtons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ColorDisplays",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ColorDisplays",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ColorDisplays",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ColorDisplays",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ColorDisplays",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Coatings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Coatings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "Coatings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "Coatings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "Coatings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarPanels",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarPanelFittings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarPanelFittings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarPanelFittings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarPanelFittings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarPanelFittings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarLightings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarLightings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarLightings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarLightings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarLightings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CargoTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CargoTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CargoTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CargoTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CargoTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarFrameTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarFramePositions",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarFramePositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarFramePositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarFramePositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarFramePositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarFrameBaseTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarFrameBaseTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarFrameBaseTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarFrameBaseTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarFrameBaseTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarFloorTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarFloorTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarFloorTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarFloorTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarFloorTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarFloorSurfaces",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarFloorSurfaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarFloorSurfaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarFloorSurfaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarFloorSurfaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarFloorSheets",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarFloorSheets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarFloorSheets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarFloorSheets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarFloorSheets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarFloorProfiles",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarFloorProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarFloorProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarFloorProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarFloorProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarFloorings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarFloorings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarFloorings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarFloorings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarFloorings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarFloorColorTyps",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarFloorColorTyps",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarFloorColorTyps",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarFloorColorTyps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarFloorColorTyps",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarDoors",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarDoors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarDoors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarDoors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarDoors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarCoverPanels",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarCoverPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarCoverPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarCoverPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarCoverPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CarCoverGlassPanels",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CarCoverGlassPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CarCoverGlassPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CarCoverGlassPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CarCoverGlassPanels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ButtonStylusPlates",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ButtonStylusPlates",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ButtonStylusPlates",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ButtonStylusPlates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ButtonStylusPlates",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Buttons",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Buttons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "Buttons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "Buttons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "Buttons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "BuildingTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "BuildingTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "BuildingTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "BuildingTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "BuildingTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "BufferPropProfiles",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "BufferPropProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "BufferPropProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "BufferPropProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "BufferPropProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "BellPositions",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "BellPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "BellPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "BellPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "BellPositions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AntiDrums",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "AntiDrums",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "AntiDrums",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "AntiDrums",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "AntiDrums",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "WallMaterials");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "WallMaterials");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "WallMaterials");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "WallMaterials");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "WallMaterials");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "VandalResistants");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "VandalResistants");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "VandalResistants");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "VandalResistants");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "VandalResistants");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "StylusPlateMaterials");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "StylusPlateMaterials");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "StylusPlateMaterials");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "StylusPlateMaterials");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "StylusPlateMaterials");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SmokeExtractionShafts");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "SmokeExtractionShafts");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "SmokeExtractionShafts");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "SmokeExtractionShafts");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "SmokeExtractionShafts");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SkirtingBoards");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "SkirtingBoards");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "SkirtingBoards");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "SkirtingBoards");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "SkirtingBoards");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ShaftTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ShaftTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ShaftTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ShaftTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ShaftTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ShaftFrameFieldFillings");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ShaftFrameFieldFillings");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ShaftFrameFieldFillings");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ShaftFrameFieldFillings");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ShaftFrameFieldFillings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SafetyGearTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "SafetyGearTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "SafetyGearTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "SafetyGearTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "SafetyGearTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ReducedProtectionSpaces");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ReducedProtectionSpaces");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ReducedProtectionSpaces");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ReducedProtectionSpaces");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ReducedProtectionSpaces");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "RammingProtections");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "RammingProtections");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "RammingProtections");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "RammingProtections");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "RammingProtections");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "RailBracketFixings");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "RailBracketFixings");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "RailBracketFixings");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "RailBracketFixings");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "RailBracketFixings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ProtectiveRailings");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ProtectiveRailings");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ProtectiveRailings");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ProtectiveRailings");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ProtectiveRailings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "PowerSupplys");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "PowerSupplys");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "PowerSupplys");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "PowerSupplys");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "PowerSupplys");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "PitLadders");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "PitLadders");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "PitLadders");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "PitLadders");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "PitLadders");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "OverspeedGovernors");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "OverspeedGovernors");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "OverspeedGovernors");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "OverspeedGovernors");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "OverspeedGovernors");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "OutdoorPanelFastenings");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "OutdoorPanelFastenings");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "OutdoorPanelFastenings");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "OutdoorPanelFastenings");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "OutdoorPanelFastenings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "MirrorStyles");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "MirrorStyles");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "MirrorStyles");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "MirrorStyles");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "MirrorStyles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Mirrors");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Mirrors");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "Mirrors");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "Mirrors");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "Mirrors");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "MaterialThicknesss");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "MaterialThicknesss");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "MaterialThicknesss");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "MaterialThicknesss");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "MaterialThicknesss");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "MaterialSurfaces");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "MaterialSurfaces");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "MaterialSurfaces");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "MaterialSurfaces");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "MaterialSurfaces");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "MachineRoomPositions");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "MachineRoomPositions");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "MachineRoomPositions");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "MachineRoomPositions");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "MachineRoomPositions");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LoadWeighingDevices");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LoadWeighingDevices");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LoadWeighingDevices");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LoadWeighingDevices");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LoadWeighingDevices");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LoadingDevices");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LoadingDevices");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LoadingDevices");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LoadingDevices");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LoadingDevices");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LightFieldSizes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LightFieldSizes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LightFieldSizes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LightFieldSizes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LightFieldSizes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftPositionSystems");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftPositionSystems");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftPositionSystems");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftPositionSystems");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftPositionSystems");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftDoorTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftDoorTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftDoorTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftDoorTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftDoorTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftDoorStandards");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftDoorStandards");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftDoorStandards");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftDoorStandards");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftDoorStandards");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftDoorSills");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftDoorSills");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftDoorSills");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftDoorSills");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftDoorSills");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftDoorOpeningDirections");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftDoorOpeningDirections");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftDoorOpeningDirections");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftDoorOpeningDirections");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftDoorOpeningDirections");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftDoorLockingDevices");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftDoorLockingDevices");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftDoorLockingDevices");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftDoorLockingDevices");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftDoorLockingDevices");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftDoorGuards");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftDoorGuards");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftDoorGuards");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftDoorGuards");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftDoorGuards");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftDoorControls");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftDoorControls");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftDoorControls");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftDoorControls");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftDoorControls");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftCarTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftCarTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftCarTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftCarTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftCarTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftCarRoofs");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftCarRoofs");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftCarRoofs");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftCarRoofs");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftCarRoofs");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftBuffers");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftBuffers");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftBuffers");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftBuffers");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftBuffers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "InstallationInfos");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "InstallationInfos");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "InstallationInfos");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "InstallationInfos");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "InstallationInfos");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "HydraulicLocks");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "HydraulicLocks");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "HydraulicLocks");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "HydraulicLocks");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "HydraulicLocks");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Handrails");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Handrails");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "Handrails");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "Handrails");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "Handrails");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "GuideTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "GuideTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "GuideTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "GuideTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "GuideTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "GuideRailsStatuss");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "GuideRailsStatuss");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "GuideRailsStatuss");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "GuideRailsStatuss");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "GuideRailsStatuss");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "GuideRailLengths");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "GuideRailLengths");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "GuideRailLengths");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "GuideRailLengths");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "GuideRailLengths");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "GuideModelTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "GuideModelTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "GuideModelTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "GuideModelTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "GuideModelTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "GoodsLiftStandards");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "GoodsLiftStandards");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "GoodsLiftStandards");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "GoodsLiftStandards");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "GoodsLiftStandards");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "FireClosures");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "FireClosures");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "FireClosures");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "FireClosures");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "FireClosures");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "FireClosureBys");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "FireClosureBys");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "FireClosureBys");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "FireClosureBys");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "FireClosureBys");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "EmergencyHotlines");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "EmergencyHotlines");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "EmergencyHotlines");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "EmergencyHotlines");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "EmergencyHotlines");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "EmergencyDevices");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "EmergencyDevices");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "EmergencyDevices");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "EmergencyDevices");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "EmergencyDevices");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "EmergencyConnections");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "EmergencyConnections");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "EmergencyConnections");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "EmergencyConnections");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "EmergencyConnections");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "EmergencyCallButtons");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "EmergencyCallButtons");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "EmergencyCallButtons");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "EmergencyCallButtons");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "EmergencyCallButtons");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ElevatorStandards");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ElevatorStandards");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ElevatorStandards");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ElevatorStandards");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ElevatorStandards");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DriveTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "DriveTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "DriveTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "DriveTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "DriveTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DriveSystemTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "DriveSystemTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "DriveSystemTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "DriveSystemTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "DriveSystemTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DriveSystems");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "DriveSystems");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "DriveSystems");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "DriveSystems");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "DriveSystems");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DriveBrakeDesigns");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "DriveBrakeDesigns");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "DriveBrakeDesigns");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "DriveBrakeDesigns");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "DriveBrakeDesigns");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DivisionBars");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "DivisionBars");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "DivisionBars");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "DivisionBars");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "DivisionBars");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Displays");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Displays");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "Displays");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "Displays");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "Displays");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DirectionIndicatorss");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "DirectionIndicatorss");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "DirectionIndicatorss");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "DirectionIndicatorss");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "DirectionIndicatorss");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DeliveryTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "DeliveryTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "DeliveryTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "DeliveryTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "DeliveryTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ControlTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ControlTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ControlTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ControlTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ControlTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ControlCabinetSizes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ControlCabinetSizes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ControlCabinetSizes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ControlCabinetSizes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ControlCabinetSizes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ControlCabinetPositions");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ControlCabinetPositions");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ControlCabinetPositions");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ControlCabinetPositions");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ControlCabinetPositions");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ColorLEDButtons");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ColorLEDButtons");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ColorLEDButtons");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ColorLEDButtons");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ColorLEDButtons");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ColorDisplays");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ColorDisplays");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ColorDisplays");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ColorDisplays");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ColorDisplays");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Coatings");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Coatings");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "Coatings");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "Coatings");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "Coatings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarPanels");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarPanels");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarPanels");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarPanels");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarPanels");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarPanelFittings");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarPanelFittings");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarPanelFittings");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarPanelFittings");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarPanelFittings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarLightings");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarLightings");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarLightings");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarLightings");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarLightings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CargoTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CargoTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CargoTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CargoTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CargoTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarFramePositions");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarFramePositions");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarFramePositions");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarFramePositions");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarFramePositions");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarFrameBaseTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarFrameBaseTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarFrameBaseTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarFrameBaseTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarFrameBaseTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarFloorTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarFloorTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarFloorTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarFloorTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarFloorTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarFloorSurfaces");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarFloorSurfaces");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarFloorSurfaces");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarFloorSurfaces");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarFloorSurfaces");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarFloorSheets");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarFloorSheets");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarFloorSheets");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarFloorSheets");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarFloorSheets");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarFloorProfiles");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarFloorProfiles");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarFloorProfiles");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarFloorProfiles");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarFloorProfiles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarFloorings");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarFloorings");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarFloorings");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarFloorings");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarFloorings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarFloorColorTyps");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarFloorColorTyps");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarFloorColorTyps");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarFloorColorTyps");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarFloorColorTyps");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarDoors");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarDoors");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarDoors");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarDoors");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarDoors");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarCoverPanels");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarCoverPanels");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarCoverPanels");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarCoverPanels");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarCoverPanels");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CarCoverGlassPanels");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CarCoverGlassPanels");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CarCoverGlassPanels");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CarCoverGlassPanels");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CarCoverGlassPanels");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ButtonStylusPlates");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ButtonStylusPlates");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ButtonStylusPlates");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ButtonStylusPlates");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ButtonStylusPlates");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Buttons");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Buttons");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "Buttons");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "Buttons");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "Buttons");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "BuildingTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "BuildingTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "BuildingTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "BuildingTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "BuildingTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "BufferPropProfiles");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "BufferPropProfiles");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "BufferPropProfiles");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "BufferPropProfiles");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "BufferPropProfiles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "BellPositions");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "BellPositions");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "BellPositions");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "BellPositions");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "BellPositions");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AntiDrums");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "AntiDrums");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "AntiDrums");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "AntiDrums");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "AntiDrums");
        }
    }
}
