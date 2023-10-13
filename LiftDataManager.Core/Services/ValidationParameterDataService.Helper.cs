using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using LiftDataManager.Core.Messenger.Messages;

namespace LiftDataManager.Core.Services;
public partial class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
{
    private void SetCarDoorData(string zugang)
    {
        if (string.IsNullOrWhiteSpace(zugang))
            return;
        string tuerTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuertyp");
        string tuerBezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Tuerbezeichnung");
        string schwellenprofilSchachttuer = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Schwellenprofil");
        string schwellenprofilKabinentuer = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_SchwellenprofilKabTuere");
        double tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB");
        double tuerHoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TH");
        double tuerGewicht = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Tuergewicht");

        ParamterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue = tuerTyp;
        ParamterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue = tuerBezeichnung;
        ParamterDictionary[$"var_TB_{zugang}"].Value = Convert.ToString(tuerBreite);
        ParamterDictionary[$"var_TH_{zugang}"].Value = Convert.ToString(tuerHoehe);
        ParamterDictionary[$"var_Tuergewicht_{zugang}"].Value = Convert.ToString(tuerGewicht);
        ParamterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue = schwellenprofilSchachttuer;
        ParamterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue = schwellenprofilKabinentuer;
    }

    private void RemoveCarDoorData(string zugang)
    {
        if (string.IsNullOrWhiteSpace(zugang))
            return;

        if (!string.IsNullOrWhiteSpace(ParamterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue))
            ParamterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue = string.Empty;
        if (!string.IsNullOrWhiteSpace(ParamterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue))
            ParamterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue = string.Empty;
        if (!string.IsNullOrWhiteSpace(ParamterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue))
            ParamterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue = string.Empty;
        if (!string.IsNullOrWhiteSpace(ParamterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue))
            ParamterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue = string.Empty;
        if (!string.IsNullOrWhiteSpace(ParamterDictionary[$"var_TB_{zugang}"].Value))
        {
            if (ParamterDictionary[$"var_TB_{zugang}"].Value != "0")
                ParamterDictionary[$"var_TB_{zugang}"].Value = string.Empty;
        }
        if (!string.IsNullOrWhiteSpace(ParamterDictionary[$"var_TH_{zugang}"].Value))
        {
            if (ParamterDictionary[$"var_TH_{zugang}"].Value != "0")
                ParamterDictionary[$"var_TH_{zugang}"].Value = string.Empty;
        }
        if (!string.IsNullOrWhiteSpace(ParamterDictionary[$"var_Tuergewicht_{zugang}"].Value))
        {
            if (ParamterDictionary[$"var_Tuergewicht_{zugang}"].Value != "0")
                ParamterDictionary[$"var_Tuergewicht_{zugang}"].Value = string.Empty;
        }
        if (!string.IsNullOrWhiteSpace(ParamterDictionary[$"var_TuerEinbauB"].Value))
        {
            if (ParamterDictionary[$"var_TuerEinbau{zugang}"].Value != "0")
                ParamterDictionary[$"var_TuerEinbau{zugang}"].Value = string.Empty;
        }
    }

    private void SetCarDesignParameterSill(string zugang, LiftDoorGroup liftDoorGroup)
    {
        if (string.IsNullOrWhiteSpace(zugang) || liftDoorGroup is null || liftDoorGroup.CarDoor is null)
            return;

        if (liftDoorGroup.DoorManufacturer != "Meiller")
        {
            ParamterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
            return;
        }

        double doorWidth = zugang == "A" ? LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, $"var_TB") : LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, $"var_TB_{zugang}");

        double supportPlateExtensionLeft;
        double supportPlateExtensionRight;
        double sillExtensionLeft;
        double sillExtensionRight;
        double sillBracketExtensionLeft;
        double sillBracketExtensionRight;

        if (liftDoorGroup.CarDoor.LiftDoorOpeningDirection!.Name == "zental öffnend")
        {
            supportPlateExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //1
            supportPlateExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //2
            sillExtensionLeft = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //9
            sillExtensionRight = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //10
            sillBracketExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //11
            sillBracketExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //12
        }
        else if (liftDoorGroup.CarDoor.LiftDoorOpeningDirection!.Name == "einseitig öffnend (links)")
        {
            supportPlateExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //1
            supportPlateExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //2
            sillExtensionLeft = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //9
            sillExtensionRight = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //10
            sillBracketExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //11
            sillBracketExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //12
        }
        else if (liftDoorGroup.CarDoor.LiftDoorOpeningDirection!.Name == "einseitig öffnend (rechts)")
        {
            supportPlateExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //1
            supportPlateExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) - 65, 2); //2
            sillExtensionLeft = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //9
            sillExtensionRight = Math.Round((doorWidth / (liftDoorGroup.CarDoor.DoorPanelCount * 2)) + 80, 2); //10
            sillBracketExtensionLeft = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //11
            sillBracketExtensionRight = Math.Round((doorWidth / liftDoorGroup.CarDoor.DoorPanelCount) + 35, 2); //12
        }
        else
        {
            ParamterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = string.Empty;
            return;
        }

        var distanceMudHolesFloorEdge = Math.Round(1.0 /3, 2); //3
        var quantityRowMudHoles = 9999; //4
        var distanceMudHole = 9999; //5
        var distanceSillBracketHoles = 9999; //6
        var quantityRowHolesSillBracket = 9999; //7
        var distanceSillMounting = 9999; //8


        // Sill

        var supportPlateWidth = 999; //13
        var offsetOKAprontoOKFF = 999; //14
        var distanceBetweenSillBracketholes = 999; //15
        var triangularLockingDistance = 999; //16

        ParamterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = $"{supportPlateExtensionLeft};{supportPlateExtensionRight};{distanceMudHolesFloorEdge};{quantityRowMudHoles};{distanceMudHole};{distanceSillBracketHoles};{quantityRowHolesSillBracket};{distanceSillMounting};{sillExtensionLeft};{sillExtensionRight};{sillBracketExtensionLeft};{sillBracketExtensionRight};{supportPlateWidth};{offsetOKAprontoOKFF};{distanceBetweenSillBracketholes};{triangularLockingDistance}";
    }
}