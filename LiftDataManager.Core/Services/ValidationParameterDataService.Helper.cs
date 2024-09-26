using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using LiftDataManager.Core.Messenger.Messages;
using System.Collections.Specialized;
using System.Globalization;

namespace LiftDataManager.Core.Services;
public partial class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
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

        var dropDownListValue = LiftParameterHelper.GetDropDownListValue(parameter.dropDownList, parameter.Value);
        if (dropDownListValue == null || dropDownListValue.Id == -1)
        {
            parameter.AddError("Value", new ParameterStateInfo(parameter.Name!, parameter.DisplayName!, $"{parameter.DisplayName}: ungültiger Wert | {parameter.Value} | ist nicht in der Auswahlliste vorhanden.", ErrorLevel.Error, false));
        }
        else
        {
            parameter.RemoveError("Value", $"{parameter.DisplayName}: ungültiger Wert | {parameter.Value} | ist nicht in der Auswahlliste vorhanden.");
        }
    }

    private void SetCarDoorData(string zugang)
    {
        if (string.IsNullOrWhiteSpace(zugang))
        {
            return;
        }

        string tuerTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tuertyp");
        string tuerBezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tuerbezeichnung");
        string schwellenprofilSchachttuer = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Schwellenprofil");
        string schwellenprofilKabinentuer = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_SchwellenprofilKabTuere");
        double tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB");
        double tuerHoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TH");
        double tuerGewicht = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Tuergewicht");
        string kaempferBreiteKabinentuer = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_KabTuerKaempferBreiteA");
        string kaempferHoeheKabinentuer = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_KabTuerKaempferHoeheA");

        LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary[$"var_Tuertyp_{zugang}"], tuerTyp);
        LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary[$"var_Tuerbezeichnung_{zugang}"], tuerBezeichnung);
        ParameterDictionary[$"var_TB_{zugang}"].Value = Convert.ToString(tuerBreite);
        ParameterDictionary[$"var_TH_{zugang}"].Value = Convert.ToString(tuerHoehe);
        ParameterDictionary[$"var_Tuergewicht_{zugang}"].Value = Convert.ToString(tuerGewicht);
        LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary[$"var_Schwellenprofil{zugang}"], schwellenprofilSchachttuer);
        LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"], schwellenprofilKabinentuer);
        LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"], kaempferBreiteKabinentuer);
        LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"], kaempferHoeheKabinentuer);
    }

    private void RemoveCarDoorData(string zugang)
    {
        if (string.IsNullOrWhiteSpace(zugang))
        {
            return;
        }
        if (ParameterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue is not null ||
            ParameterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue?.Id != 0)
        {
            ParameterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (ParameterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue is not null ||
            ParameterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue?.Id != 0)
        {
            ParameterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (ParameterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue is not null ||
            ParameterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue?.Id != 0)
        {
            ParameterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (ParameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue is not null ||
            ParameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue?.Id != 0)
        {
            ParameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownListValue is not null ||
            ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownListValue?.Id != 0)
        {
            ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownListValue = new SelectionValue();
        }
        if (ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownListValue is not null ||
            ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownListValue?.Id != 0)
        {
            ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownListValue = new SelectionValue();
        }

        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_TB_{zugang}"].Value))
        {
            if (ParameterDictionary[$"var_TB_{zugang}"].Value != "0")
            {
                ParameterDictionary[$"var_TB_{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_TH_{zugang}"].Value))
        {
            if (ParameterDictionary[$"var_TH_{zugang}"].Value != "0")
            {
                ParameterDictionary[$"var_TH_{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_Tuergewicht_{zugang}"].Value))
        {
            if (ParameterDictionary[$"var_Tuergewicht_{zugang}"].Value != "0")
            {
                ParameterDictionary[$"var_Tuergewicht_{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_TuerEinbau{zugang}"].Value))
        {
            if (ParameterDictionary[$"var_TuerEinbau{zugang}"].Value != "0")
            {
                ParameterDictionary[$"var_TuerEinbau{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].Value))
        {
            if (ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].Value != "0")
            {
                ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].Value = string.Empty;
            }
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].Value))
        {
            if (ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].Value != "0")
            {
                ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].Value = string.Empty;
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

    private void SetCarDesignParameterSill(string zugang, LiftDoorGroup liftDoorGroup)
    {
        if (string.IsNullOrWhiteSpace(zugang) || liftDoorGroup is null || liftDoorGroup.CarDoor is null)
            return;

        var openingDirection = zugang == "A" ? ParameterDictionary["var_Tueroeffnung"].Value :
                                 ParameterDictionary[$"var_Tueroeffnung_{zugang}"].Value;

        var sillProfil = zugang == "A" ? ParameterDictionary["var_SchwellenprofilKabTuere"].Value :
                                         ParameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].Value;

        if (liftDoorGroup.DoorManufacturer != "Meiller" || string.IsNullOrWhiteSpace(openingDirection) || string.IsNullOrWhiteSpace(sillProfil))
        {
            ParameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
            return;
        }

        var carFloorSill = _parametercontext.Set<LiftDoorSill>().FirstOrDefault(x => x.Name == sillProfil);

        if (carFloorSill == null)
        {
            ParameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
            return;
        }

        double doorWidth = zugang == "A" ? LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, $"var_TB") : LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, $"var_TB_{zugang}");

        double supportPlateExtensionLeft;
        double supportPlateExtensionRight;
        double sillExtensionLeft;
        double sillExtensionRight;
        double sillBracketExtensionLeft;
        double sillBracketExtensionRight;

        if (openingDirection == "zentral öffnend")
        {
            supportPlateExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //1
            supportPlateExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //2
            sillExtensionLeft = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //9
            sillExtensionRight = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //10
            sillBracketExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //11
            sillBracketExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //12
        }
        else if (openingDirection == "einseitig öffnend (links)")
        {
            supportPlateExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //1
            supportPlateExtensionRight = 0; //2
            sillExtensionLeft = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //9
            sillExtensionRight = 0; //10
            sillBracketExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //11
            sillBracketExtensionRight = 50; //12
        }
        else if (openingDirection == "einseitig öffnend (rechts)")
        {
            supportPlateExtensionLeft = 0; //1
            supportPlateExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //2
            sillExtensionLeft = 0; //9
            sillExtensionRight = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //10
            sillBracketExtensionLeft = 50; //11
            sillBracketExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //12
        }
        else
        {
            ParameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
            return;
        }

        var distanceMudHolesFloorEdge = liftDoorGroup.CarDoor.Name == "STK26" ? 51.5 : 25.5; //3
        var quantityRowMudHoles = liftDoorGroup.CarDoor.LiftDoorOpeningDirectionId == 3 ? liftDoorGroup.CarDoor.DoorPanelCount / 2 : liftDoorGroup.CarDoor.DoorPanelCount; //4
        var distanceMudHole = liftDoorGroup.CarDoor.Name == "STK26" ? 0 : 42; //5
        var distanceSillBracketHoles = liftDoorGroup.CarDoor.Name == "STK26" ? 26 : 46.5; //6
        var quantityRowHolesSillBracket = (liftDoorGroup.CarDoor.DoorPanelCount == 3 || liftDoorGroup.CarDoor.DoorPanelCount == 6) ? 2 : 1; //7
        var distanceSillMounting = (liftDoorGroup.CarDoor.DoorPanelCount == 3 || liftDoorGroup.CarDoor.DoorPanelCount == 6) ? 42 : 0; //8

        double supportPlateWidth; //13
        double offsetOKAprontoOKFF; //14
        double distanceBetweenSillBracketholes; //15
        double triangularLockingDistance; //16

        switch (carFloorSill.SillMountTyp)
        {
            case 0:
                ParameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
                return;
            case 1:
                supportPlateWidth = liftDoorGroup.CarDoor.SillWidth - 13; //13
                offsetOKAprontoOKFF = 4; //14
                distanceBetweenSillBracketholes = 42; //15
                triangularLockingDistance = 100; //16
                break;
            case 2:
                supportPlateWidth = liftDoorGroup.CarDoor.SillWidth - 13; //13
                offsetOKAprontoOKFF = 29; //14
                distanceBetweenSillBracketholes = 42; //15
                triangularLockingDistance = 100; //16
                break;
            case 3:
                supportPlateWidth = liftDoorGroup.CarDoor.SillWidth - 25; //13
                offsetOKAprontoOKFF = 88; //14
                distanceBetweenSillBracketholes = 105; //15
                triangularLockingDistance = 168; //16
                break;
            default:
                ParameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
                return;
        }
        ParameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = $"{supportPlateExtensionLeft};{supportPlateExtensionRight};{distanceMudHolesFloorEdge};{quantityRowMudHoles};{distanceMudHole};{distanceSillBracketHoles};{quantityRowHolesSillBracket};{distanceSillMounting};{sillExtensionLeft};{sillExtensionRight};{sillBracketExtensionLeft};{sillBracketExtensionRight};{supportPlateWidth};{offsetOKAprontoOKFF};{distanceBetweenSillBracketholes};{triangularLockingDistance};{carFloorSill.SillMountTyp}";
    }

    private IEnumerable<SelectionValue> GetAvailableDoorSills(string? doorTyp, string? doorDescription)
    {
        var sills = _parametercontext.Set<LiftDoorSill>();

        if (!string.IsNullOrWhiteSpace(doorTyp))
        {
            if (doorTyp.StartsWith("Wittur"))
            {
                return sills.Where(x => x.Manufacturer == "Wittur").Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
            }
            if (doorTyp.StartsWith("Riedl"))
            {
                return sills.Where(x => x.Manufacturer == "Riedl").Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
            }
            //Meiller Filter Optionen
            var cat = ParameterDictionary["var_EN8171Cat012"].Value;
            if (string.IsNullOrWhiteSpace(doorDescription))
            {
                if (string.IsNullOrWhiteSpace(cat) || string.Equals(cat, "EN81-71 Cat 0"))
                {
                    return sills.Where(x => x.Manufacturer == "Meiller").Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
                }
                return sills.Where(x => x.Manufacturer == "Meiller" && x.IsVandalResistant).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
            }
            else if (doorDescription.StartsWith("DT"))
            {
                return sills.Where(x => x.Manufacturer == "Meiller" && x.SillFilterTyp == "0").Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
            }
            else
            {
                var doorNumber = doorDescription.Replace("HD", "")[^3..].Trim();

                if (string.IsNullOrWhiteSpace(cat) || string.Equals(cat, "EN81-71 Cat 0"))
                {
                    return sills.Where(x => x.Manufacturer == "Meiller" && (x.SillFilterTyp == "0" || x.SillFilterTyp!.Contains(doorNumber))).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
                }
                return sills.Where(x => x.Manufacturer == "Meiller" && x.IsVandalResistant).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
            }
        }

        return sills.Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
    }

    private void UpdateDropDownList(string? parameterName, IEnumerable<SelectionValue> newList, bool defaultSelection = true)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            return;
        if (!newList.Any())
        {
            ParameterDictionary[parameterName].DropDownList.Clear();
            return;
        }
        var updateList = defaultSelection ? newList.Prepend(new SelectionValue()) : newList;

        if (ParameterDictionary[parameterName].DropDownList.SequenceEqual(updateList))
        {
            return;
        }
        ParameterDictionary[parameterName].DropDownList.Clear();
        ParameterDictionary[parameterName].DropDownList.AddRange(updateList, NotifyCollectionChangedAction.Reset);
        ParameterDictionary[parameterName].RefreshDropDownListValue();
    }

    private double GetDefaultCeiling()
    {
        double carWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KBI");

        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_overrideDefaultCeiling"].Value))
        {
            return Convert.ToDouble(ParameterDictionary["var_overrideDefaultCeiling"].Value, CultureInfo.CurrentCulture);
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
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_abgDecke"))
        {
            if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_overrideSuspendedCeiling"].Value))
            {
                return Convert.ToDouble(ParameterDictionary["var_overrideSuspendedCeiling"].Value, CultureInfo.CurrentCulture);
            }
            return 35;
        }
        return 0;
    }
}