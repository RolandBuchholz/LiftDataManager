using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using System.Collections.Specialized;
using System.Globalization;

namespace LiftDataManager.Core.Services;
public partial class ValidationParameterDataService : IValidationParameterDataService
{
    private static void CheckListContainsValue(Parameter? parameter)
    {
        if (parameter is null)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(parameter.Value))
        {
            parameter.RemoveError("Value", $"{parameter.DisplayName}: ungültiger Wert | {parameter.Value} | ist nicht in der Auswahlliste vorhanden.");
            return;
        }

        var dropDownListValue = LiftParameterHelper.GetDropDownListValue(parameter.DropDownList, parameter.Value);
        if (dropDownListValue == null || dropDownListValue.Id == -1)
        {
            parameter.AddError("Value", new ParameterStateInfo(parameter.Name!, parameter.DisplayName!, $"{parameter.DisplayName}: ungültiger Wert | {parameter.Value} | ist nicht in der Auswahlliste vorhanden.", ErrorLevel.Error, false));
        }
        else
        {
            parameter.RemoveError("Value", $"{parameter.DisplayName}: ungültiger Wert | {parameter.Value} | ist nicht in der Auswahlliste vorhanden.");
        }
    }

    private void SetShaftDoorDoorData(string zugang, bool advancedDoorSelection)
    {
        if (string.IsNullOrWhiteSpace(zugang))
        {
            return;
        }

        if (!advancedDoorSelection)
        {
            string tuerTyp = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Tuertyp");
            string tuerBezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Tuerbezeichnung");
            LiftParameterHelper.UpdateParameterDropDownListValue(_parameterDictionary[$"var_Tuertyp_{zugang}"], tuerTyp);
            LiftParameterHelper.UpdateParameterDropDownListValue(_parameterDictionary[$"var_Tuerbezeichnung_{zugang}"], tuerBezeichnung);
        }

        string zulassungTuere = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_ZulassungTuere");
        string schwellenprofilSchachttuer = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Schwellenprofil");
        string schwellenprofilKabinentuer = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_SchwellenprofilKabTuere");
        double tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TB");
        double tuerHoehe = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TH");
        double tuerGewicht = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Tuergewicht");
        string kaempferBreiteKabinentuer = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_KabTuerKaempferBreiteA");
        string kaempferHoeheKabinentuer = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_KabTuerKaempferHoeheA");
        string carDoorDescription = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_CarDoorDescriptionA");
        string shaftDoorDescription = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_ShaftDoorDescriptionA");

        _parameterDictionary[$"var_TB_{zugang}"].Value = Convert.ToString(tuerBreite);
        _parameterDictionary[$"var_TH_{zugang}"].Value = Convert.ToString(tuerHoehe);
        _parameterDictionary[$"var_Tuergewicht_{zugang}"].Value = Convert.ToString(tuerGewicht);
        LiftParameterHelper.UpdateParameterDropDownListValue(_parameterDictionary[$"var_Schwellenprofil{zugang}"], schwellenprofilSchachttuer);
        LiftParameterHelper.UpdateParameterDropDownListValue(_parameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"], schwellenprofilKabinentuer);
        LiftParameterHelper.UpdateParameterDropDownListValue(_parameterDictionary[$"var_ZulassungTuere_{zugang}"], zulassungTuere);
        LiftParameterHelper.UpdateParameterDropDownListValue(_parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"], kaempferBreiteKabinentuer);
        LiftParameterHelper.UpdateParameterDropDownListValue(_parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"], kaempferHoeheKabinentuer);
        LiftParameterHelper.UpdateParameterDropDownListValue(_parameterDictionary[$"var_CarDoorDescription{zugang}"], carDoorDescription);
        LiftParameterHelper.UpdateParameterDropDownListValue(_parameterDictionary[$"var_ShaftDoorDescription{zugang}"], shaftDoorDescription);
    }

    private void RemoveShaftDoorDoorData(string zugang)
    {
        if (string.IsNullOrWhiteSpace(zugang))
        {
            return;
        }
        if (_parameterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue is not null ||
            _parameterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue?.Id != 0)
        {
            _parameterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (_parameterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue is not null ||
            _parameterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue?.Id != 0)
        {
            _parameterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (_parameterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue is not null ||
            _parameterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue?.Id != 0)
        {
            _parameterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (_parameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue is not null ||
            _parameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue?.Id != 0)
        {
            _parameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (_parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownListValue is not null ||
            _parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownListValue?.Id != 0)
        {
            _parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (_parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownListValue is not null ||
            _parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownListValue?.Id != 0)
        {
            _parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (_parameterDictionary[$"var_ZulassungTuere_{zugang}"].DropDownListValue is not null ||
            _parameterDictionary[$"var_ZulassungTuere_{zugang}"].DropDownListValue?.Id != 0)
        {
            _parameterDictionary[$"var_ZulassungTuere_{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (_parameterDictionary[$"var_CarDoorDescription{zugang}"].DropDownListValue is not null ||
            _parameterDictionary[$"var_CarDoorDescription{zugang}"].DropDownListValue?.Id != 0)
        {
            _parameterDictionary[$"var_CarDoorDescription{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (_parameterDictionary[$"var_ShaftDoorDescription{zugang}"].DropDownListValue is not null ||
            _parameterDictionary[$"var_ShaftDoorDescription{zugang}"].DropDownListValue?.Id != 0)
        {
            _parameterDictionary[$"var_ShaftDoorDescription{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (!string.IsNullOrWhiteSpace(_parameterDictionary[$"var_TB_{zugang}"].Value))
        {
            if (_parameterDictionary[$"var_TB_{zugang}"].Value != "0")
            {
                _parameterDictionary[$"var_TB_{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(_parameterDictionary[$"var_TH_{zugang}"].Value))
        {
            if (_parameterDictionary[$"var_TH_{zugang}"].Value != "0")
            {
                _parameterDictionary[$"var_TH_{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(_parameterDictionary[$"var_Tuergewicht_{zugang}"].Value))
        {
            if (_parameterDictionary[$"var_Tuergewicht_{zugang}"].Value != "0")
            {
                _parameterDictionary[$"var_Tuergewicht_{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(_parameterDictionary[$"var_TuerEinbau{zugang}"].Value))
        {
            if (_parameterDictionary[$"var_TuerEinbau{zugang}"].Value != "0")
            {
                _parameterDictionary[$"var_TuerEinbau{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(_parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].Value))
        {
            if (_parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].Value != "0")
            {
                _parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(_parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].Value))
        {
            if (_parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].Value != "0")
            {
                _parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].Value = string.Empty;
            }
        }
    }

    private string GetDriveSystem(string? lift)
    {
        string driveSystem = string.Empty;
        if (!string.IsNullOrWhiteSpace(lift))
        {
            var driveSystemDB = _parametercontext.Set<LiftType>().Include(i => i.DriveType).FirstOrDefault(x => x.Name == lift)?.DriveType?.Name;
            if (!string.IsNullOrWhiteSpace(driveSystemDB))
                return driveSystemDB;
        }
        return driveSystem;
    }

    private void SetCarDesignParameterSill(string? zugang, CarDoor? carDoor)
    {
        if (string.IsNullOrWhiteSpace(zugang))
        {
            return;
        }

        var openingDirection = zugang == "A" ? _parameterDictionary["var_Tueroeffnung"].Value :
                                               _parameterDictionary[$"var_Tueroeffnung_{zugang}"].Value;

        var sillProfil = zugang == "A" ? _parameterDictionary["var_SchwellenprofilKabTuere"].Value :
                                         _parameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].Value;

        if (carDoor is null || carDoor.Manufacturer != "Meiller" || string.IsNullOrWhiteSpace(openingDirection) || string.IsNullOrWhiteSpace(sillProfil))
        {
            _parameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
            return;
        }

        var carFloorSill = _parametercontext.Set<LiftDoorSill>().FirstOrDefault(x => x.Name == sillProfil);

        if (carFloorSill == null)
        {
            _parameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
            return;
        }

        double doorWidth = zugang == "A" ? LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, $"var_TB") : LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, $"var_TB_{zugang}");

        double supportPlateExtensionLeft;
        double supportPlateExtensionRight;
        double sillExtensionLeft;
        double sillExtensionRight;
        double sillBracketExtensionLeft;
        double sillBracketExtensionRight;

        if (openingDirection == "zentral öffnend")
        {
            supportPlateExtensionLeft = Math.Round((doorWidth / carDoor.DoorPanelCount) - 65, 2); //1
            supportPlateExtensionRight = Math.Round((doorWidth / carDoor.DoorPanelCount) - 65, 2); //2
            sillExtensionLeft = Math.Round((doorWidth / (carDoor.DoorPanelCount * 2)) + 80, 2); //9
            sillExtensionRight = Math.Round((doorWidth / (carDoor.DoorPanelCount * 2)) + 80, 2); //10
            sillBracketExtensionLeft = Math.Round((doorWidth / carDoor.DoorPanelCount) + 35, 2); //11
            sillBracketExtensionRight = Math.Round((doorWidth / carDoor.DoorPanelCount) + 35, 2); //12
        }
        else if (openingDirection == "einseitig öffnend (links)")
        {
            supportPlateExtensionLeft = Math.Round((doorWidth / carDoor.DoorPanelCount) - 65, 2); //1
            supportPlateExtensionRight = 0; //2
            sillExtensionLeft = Math.Round((doorWidth / (carDoor.DoorPanelCount * 2)) + 80, 2); //9
            sillExtensionRight = 0; //10
            sillBracketExtensionLeft = Math.Round((doorWidth / carDoor.DoorPanelCount) + 35, 2); //11
            sillBracketExtensionRight = 50; //12
        }
        else if (openingDirection == "einseitig öffnend (rechts)")
        {
            supportPlateExtensionLeft = 0; //1
            supportPlateExtensionRight = Math.Round((doorWidth / carDoor.DoorPanelCount) - 65, 2); //2
            sillExtensionLeft = 0; //9
            sillExtensionRight = Math.Round((doorWidth / (carDoor.DoorPanelCount * 2)) + 80, 2); //10
            sillBracketExtensionLeft = 50; //11
            sillBracketExtensionRight = Math.Round((doorWidth / carDoor.DoorPanelCount) + 35, 2); //12
        }
        else
        {
            _parameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
            return;
        }

        var distanceMudHolesFloorEdge = carDoor.Name == "STK26" ? 51.5 : 25.5; //3
        var quantityRowMudHoles = carDoor.LiftDoorOpeningDirectionId == 3 ? carDoor.DoorPanelCount / 2 : carDoor.DoorPanelCount; //4
        var distanceMudHole = carDoor.Name == "STK26" ? 0 : 42; //5
        var distanceSillBracketHoles = carDoor.Name == "STK26" ? 26 : 46.5; //6
        var quantityRowHolesSillBracket = (carDoor.DoorPanelCount == 3 || carDoor.DoorPanelCount == 6) ? 2 : 1; //7
        var distanceSillMounting = (carDoor.DoorPanelCount == 3 || carDoor.DoorPanelCount == 6) ? 42 : 0; //8

        double supportPlateWidth; //13
        double offsetOKAprontoOKFF; //14
        double distanceBetweenSillBracketholes; //15
        double triangularLockingDistance; //16

        switch (carFloorSill.SillMountTyp)
        {
            case 0:
                _parameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
                return;
            case 1:
                supportPlateWidth = carDoor.SillWidth - 13; //13
                offsetOKAprontoOKFF = 4; //14
                distanceBetweenSillBracketholes = 42; //15
                triangularLockingDistance = 100; //16
                break;
            case 2:
                supportPlateWidth = carDoor.SillWidth - 13; //13
                offsetOKAprontoOKFF = 29; //14
                distanceBetweenSillBracketholes = 43; //15
                triangularLockingDistance = 100; //16
                break;
            case 3:
                supportPlateWidth = carDoor.SillWidth - 25; //13
                offsetOKAprontoOKFF = 88; //14
                distanceBetweenSillBracketholes = 105; //15
                triangularLockingDistance = 168; //16
                break;
            case 4:
                supportPlateWidth = carDoor.SillWidth - 13; //13
                offsetOKAprontoOKFF = 31; //14
                distanceBetweenSillBracketholes = 45; //15
                triangularLockingDistance = 100; //16
                break;
            default:
                _parameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
                return;
        }
        _parameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = $"{supportPlateExtensionLeft};{supportPlateExtensionRight};{distanceMudHolesFloorEdge};{quantityRowMudHoles};{distanceMudHole};{distanceSillBracketHoles};{quantityRowHolesSillBracket};{distanceSillMounting};{sillExtensionLeft};{sillExtensionRight};{sillBracketExtensionLeft};{sillBracketExtensionRight};{supportPlateWidth};{offsetOKAprontoOKFF};{distanceBetweenSillBracketholes};{triangularLockingDistance};{carFloorSill.SillMountTyp}";
    }

    private IEnumerable<SelectionValue> GetAvailableDoorSills(string carDoorName)
    {
        var sills = _parametercontext.Set<LiftDoorSill>();
        var availableDoorSills = Enumerable.Empty<SelectionValue>();

        var carDoor = _parametercontext.Set<CarDoor>().FirstOrDefault(x => x.Name == carDoorName);
        if (carDoor is not null)
        {
            if (carDoor.Manufacturer == "Meiller")
            {
                string cat = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_EN8171Cat012");
                var doorNumber = carDoor.Name[3..5];

                if (string.IsNullOrWhiteSpace(cat) || string.Equals(cat, "EN81-71 Cat 0"))
                {
                    availableDoorSills = sills.Where(x => x.Manufacturer == carDoor.Manufacturer && (x.SillFilterTyp == "0" || x.SillFilterTyp!.Contains(doorNumber))).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });
                }
                else
                {
                    availableDoorSills = sills.Where(x => x.Manufacturer == carDoor.Manufacturer && x.IsVandalResistant).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });
                }
            }
            else
            {
                availableDoorSills = sills.Where(x => x.Manufacturer == carDoor.Manufacturer).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });
            }
        }
        else
        {
            availableDoorSills = sills.Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });
        }
        return availableDoorSills;
    }

    private void UpdateDropDownList(string? parameterName, IEnumerable<SelectionValue> newList, bool defaultSelection = true)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return;
        }
        if (!newList.Any())
        {
            _parameterDictionary[parameterName].DropDownList.Clear();
            return;
        }
        var orderedList = newList.OrderBy(x => x.OrderSelection);
        var updateList = defaultSelection ? orderedList.Prepend(new SelectionValue()) : orderedList;
        if (_parameterDictionary[parameterName].DropDownList.SequenceEqual(updateList))
        {
            return;
        }
        _parameterDictionary[parameterName].DropDownList.Clear();
        _parameterDictionary[parameterName].DropDownList.AddRange(updateList, NotifyCollectionChangedAction.Reset);
        _parameterDictionary[parameterName].RefreshDropDownListValue();
    }

    private double GetDefaultCeiling()
    {
        double carWidth = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_KBI");

        if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_overrideDefaultCeiling"].Value))
        {
            return Convert.ToDouble(_parameterDictionary["var_overrideDefaultCeiling"].Value, CultureInfo.CurrentCulture);
        }
        return carWidth switch
        {
            > 2000 => 120,
            > 1400 => 85,
            _ => 50,
        };
    }

    private double GetSuspendedCeiling()
    {
        if (LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_abgDecke"))
        {
            if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_overrideSuspendedCeiling"].Value))
            {
                return Convert.ToDouble(_parameterDictionary["var_overrideSuspendedCeiling"].Value, CultureInfo.CurrentCulture);
            }
            return 35;
        }
        return 0;
    }

    private IEnumerable<SelectionValue> FilterSafetyGears(string? safetyGear)
    {
        var safetyGears = _parametercontext.Set<SafetyGearModelType>().ToList();
        return safetyGear switch
        {
            "keine" => [],
            "Sperrfangvorrichtung" => safetyGears.Where(x => x.SafetyGearTypeId == 1).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection }),
            "Bremsfangvorrichtung" => safetyGears.Where(x => x.SafetyGearTypeId == 2).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection }),
            "Rohrbruchventil" => safetyGears.Where(x => x.SafetyGearTypeId == 3).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection }),
            _ => safetyGears.Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection }),
        };
    }

    private void ResetReducedProtectionSpacesbuffer(SelectionValue? protectionSpace)
    {
        switch (protectionSpace?.Id)
        {
            //Headroom
            case 2:
                _parameterDictionary["var_Anzahl_Puffer_EM_SG"].AutoUpdateParameterValue(string.Empty);
                _parameterDictionary["var_Puffertyp_EM_SG"].DropDownListValue = new SelectionValue();
                break;
            //Shaftpit
            case 3:
                _parameterDictionary["var_Anzahl_Puffer_EM_SK"].AutoUpdateParameterValue(string.Empty);
                _parameterDictionary["var_Puffertyp_EM_SK"].DropDownListValue = new SelectionValue();
                break;
            //Headroom and Shaftpit
            case 4:
                break;
            //None or ESG or pre-triggered stopping system
            default:
                _parameterDictionary["var_Anzahl_Puffer_EM_SK"].AutoUpdateParameterValue(string.Empty);
                _parameterDictionary["var_Anzahl_Puffer_EM_SG"].AutoUpdateParameterValue(string.Empty);
                _parameterDictionary["var_Puffertyp_EM_SK"].DropDownListValue = new SelectionValue();
                _parameterDictionary["var_Puffertyp_EM_SG"].DropDownListValue = new SelectionValue();
                break;
        }
    }
}