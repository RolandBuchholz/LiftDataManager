﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        string tuerTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tuertyp");
        string tuerBezeichnung = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tuerbezeichnung");
        string schwellenprofilSchachttuer = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Schwellenprofil");
        string schwellenprofilKabinentuer = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_SchwellenprofilKabTuere");
        double tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB");
        double tuerHoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TH");
        double tuerGewicht = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Tuergewicht");

        ParameterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue = tuerTyp;
        ParameterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue = tuerBezeichnung;
        ParameterDictionary[$"var_TB_{zugang}"].Value = Convert.ToString(tuerBreite);
        ParameterDictionary[$"var_TH_{zugang}"].Value = Convert.ToString(tuerHoehe);
        ParameterDictionary[$"var_Tuergewicht_{zugang}"].Value = Convert.ToString(tuerGewicht);
        ParameterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue = schwellenprofilSchachttuer;
        ParameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue = schwellenprofilKabinentuer;
    }

    private void RemoveCarDoorData(string zugang)
    {
        if (string.IsNullOrWhiteSpace(zugang))
            return;

        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue))
            ParameterDictionary[$"var_Tuertyp_{zugang}"].DropDownListValue = string.Empty;
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue))
            ParameterDictionary[$"var_Tuerbezeichnung_{zugang}"].DropDownListValue = string.Empty;
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue))
            ParameterDictionary[$"var_Schwellenprofil{zugang}"].DropDownListValue = string.Empty;
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue))
            ParameterDictionary[$"var_SchwellenprofilKabTuere{zugang}"].DropDownListValue = string.Empty;
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_TB_{zugang}"].Value))
        {
            if (ParameterDictionary[$"var_TB_{zugang}"].Value != "0")
                ParameterDictionary[$"var_TB_{zugang}"].Value = string.Empty;
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_TH_{zugang}"].Value))
        {
            if (ParameterDictionary[$"var_TH_{zugang}"].Value != "0")
                ParameterDictionary[$"var_TH_{zugang}"].Value = string.Empty;
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_Tuergewicht_{zugang}"].Value))
        {
            if (ParameterDictionary[$"var_Tuergewicht_{zugang}"].Value != "0")
                ParameterDictionary[$"var_Tuergewicht_{zugang}"].Value = string.Empty;
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary[$"var_TuerEinbauB"].Value))
        {
            if (ParameterDictionary[$"var_TuerEinbau{zugang}"].Value != "0")
                ParameterDictionary[$"var_TuerEinbau{zugang}"].Value = string.Empty;
        }
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
        ParameterDictionary[$"var_SchwellenUnterbau{zugang}"].Value = $"{supportPlateExtensionLeft};{supportPlateExtensionRight};{distanceMudHolesFloorEdge};{quantityRowMudHoles};{distanceMudHole};{distanceSillBracketHoles};{quantityRowHolesSillBracket};{distanceSillMounting};{sillExtensionLeft};{sillExtensionRight};{sillBracketExtensionLeft};{sillBracketExtensionRight};{supportPlateWidth};{offsetOKAprontoOKFF};{distanceBetweenSillBracketholes};{triangularLockingDistance}";
    }
}