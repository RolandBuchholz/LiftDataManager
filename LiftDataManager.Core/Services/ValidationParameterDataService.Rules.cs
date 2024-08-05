using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using HtmlAgilityPack;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using LiftDataManager.Core.Messenger.Messages;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace LiftDataManager.Core.Services;
public partial class ValidationParameterDataService : ObservableRecipient, IValidationParameterDataService, IRecipient<SpeziPropertiesRequestMessage>
{
    // Standard validationrules

    private void NotEmpty(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, optionalCondition))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} darf nicht leer sein", SetSeverity(severity)));
        }
    }

    private void NotEmptyOr0(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "0") || string.Equals(value, optionalCondition))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} darf nicht leer sein", SetSeverity(severity)));
        }
    }

    private void NotEmptyWhenAnotherTrue(string name, string displayname, string? value, string? severity, string? anotherBoolean)
    {
        if (string.IsNullOrWhiteSpace(anotherBoolean))
            return;
        var anotherParameter = Convert.ToBoolean(ParameterDictionary[anotherBoolean].Value, CultureInfo.CurrentCulture);
        if (string.IsNullOrWhiteSpace(value) && anotherParameter)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} darf nicht leer sein wenn {anotherBoolean} gesetzt (wahr) ist", SetSeverity(severity))
            { DependentParameter = [anotherBoolean] });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = [anotherBoolean] });
        }
    }

    private void NotEmptyOr0WhenAnotherTrue(string name, string displayname, string? value, string? severity, string? anotherBoolean)
    {
        if (string.IsNullOrWhiteSpace(anotherBoolean))
            return;
        var anotherParameter = Convert.ToBoolean(ParameterDictionary[anotherBoolean].Value, CultureInfo.CurrentCulture);
        if ((string.IsNullOrWhiteSpace(value) || value == "0") && anotherParameter)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} darf nicht leer sein wenn {anotherBoolean} gesetzt (wahr) ist", SetSeverity(severity))
            { DependentParameter = [anotherBoolean] });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = [anotherBoolean] });
        }
    }

    private void NotTrueWhenTheOtherIsTrue(string name, string displayname, string? value, string? severity, string? anotherBoolean)
    {
        if (string.IsNullOrWhiteSpace(anotherBoolean))
            return;
        var anotherParameter = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, anotherBoolean!);
        if (!anotherParameter)
            return;

        if (string.Equals(value, "True", StringComparison.CurrentCultureIgnoreCase))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Es is nicht möglich beide Optionen ({displayname} und {ParameterDictionary[anotherBoolean].DisplayName}) auszuwählen!", SetSeverity(severity))
            { DependentParameter = [anotherBoolean] });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = [anotherBoolean] });
        }
    }

    private void MustBeTrueWhenAnotherNotEmty(string name, string displayname, string? value, string? severity, string? anotherString)
    {
        if (string.IsNullOrWhiteSpace(anotherString))
            return;
        var valueToBool = Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        var stringValue = ParameterDictionary[anotherString].Value;

        if (valueToBool && (string.IsNullOrWhiteSpace(stringValue)))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} gesetzt (wahr) ist, darf {anotherString} nicht leer sein", SetSeverity(severity))
            { DependentParameter = [anotherString] });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = [anotherString] });
        }
    }

    private void ListContainsValue(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }
        var dropDownListValue = LiftParameterHelper.GetDropDownListValue(ParameterDictionary[name].dropDownList, value);
        if (dropDownListValue == null || dropDownListValue.Id == -1)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{displayname}: ungültiger Wert | {value} | ist nicht in der Auswahlliste vorhanden.", SetSeverity(severity)));
        }
    }

    // Spezial validationrules

    private void ValidateCreationDate(string name, string displayname, string? value, string? severity, string? odernummerName)
    {
        if (!string.IsNullOrWhiteSpace(value))
            return;
        ParameterDictionary["var_ErstelltAm"].Value = DateTime.Now.ToShortDateString();
    }

    private void ValidateJobNumber(string name, string displayname, string? value, string? severity, string? odernummerName)
    {
        if (string.IsNullOrWhiteSpace(odernummerName))
            return;
        var fabriknummer = ParameterDictionary["var_FabrikNummer"].Value;

        if (string.IsNullOrWhiteSpace(fabriknummer))
            return;
        ParameterDictionary["var_FabrikNummer"].ClearErrors("var_FabrikNummer");

        var auftragsnummer = ParameterDictionary[odernummerName].Value;
        var informationAufzug = ParameterDictionary["var_InformationAufzug"].Value;
        var fabriknummerBestand = ParameterDictionary["var_FabriknummerBestand"].Value;

        switch (informationAufzug)
        {
            case "Neuanlage" or "Ersatzanlage":
                if (auftragsnummer != fabriknummer)
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Bei Neuanlagen und Ersatzanlagen muß die Auftragsnummer und Fabriknummer identisch sein", SetSeverity(severity))
                    { DependentParameter = ["var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand"] });
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = ["var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand"] });
                }
                return;
            case "Umbau":
                if (string.IsNullOrWhiteSpace(fabriknummerBestand) && auftragsnummer != fabriknummer)
                    return;
                if (fabriknummerBestand != fabriknummer)
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Bei Umbauten muß die Fabriknummer der alten Anlage beibehalten werden", SetSeverity(severity))
                    { DependentParameter = ["var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand"] });
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = ["var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand"] });
                }
                return;
            default:
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = ["var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand"] });
                return;
        }
    }

    private void ValidateJungblutOSG(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (value is null)
            return;
        if (value.StartsWith("Jungblut"))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{value} bei diesem Geschwindigkeitsbegrenzer wir gegebenfals eine Sicherheitsschaltung in der Steuerung benötigt.\n" +
                                                              $"Halten sie Rücksprache mit ihrem Steuerungshersteller.\n" +
                                                              $"Alternativ einen Geschwindigkeitsbegrenzer mit elektromagentischer Rückstellung verwenden.", SetSeverity(severity)));
        }
    }

    private void ValidateTravel(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "0"))
            return;
        var foerderhoehe = Math.Round(LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_FH") * 1000);
        var etagenhoehe0 = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Etagenhoehe0");
        var etagenhoehe1 = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Etagenhoehe1");
        var etagenhoehe2 = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Etagenhoehe2");
        var etagenhoehe3 = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Etagenhoehe3");
        var etagenhoehe4 = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Etagenhoehe4");
        var etagenhoehe5 = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Etagenhoehe5");
        var etagenhoehe6 = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Etagenhoehe6");
        var etagenhoehe7 = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Etagenhoehe7");
        var etagenhoehe8 = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Etagenhoehe8");

        if ((etagenhoehe0 + etagenhoehe1 + etagenhoehe2 + etagenhoehe3 + etagenhoehe4 + etagenhoehe5 + etagenhoehe6 + etagenhoehe7 + etagenhoehe8) == 0)
            return;

        var etagenhoeheTotal = etagenhoehe0 + etagenhoehe1 + etagenhoehe2 + etagenhoehe3 + etagenhoehe4 + etagenhoehe5 + etagenhoehe6 + etagenhoehe7 + etagenhoehe8;

        if (etagenhoeheTotal != foerderhoehe)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Die Förderhöhe ({foerderhoehe} mm) stimmt nicht mit Etagenabständen ({etagenhoeheTotal} mm) überein.", SetSeverity(severity))
            { DependentParameter = ["var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2", "var_Etagenhoehe3", "var_Etagenhoehe4", "var_Etagenhoehe5", "var_Etagenhoehe6", "var_Etagenhoehe7", "var_Etagenhoehe8"] });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = ["var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2", "var_Etagenhoehe3", "var_Etagenhoehe4", "var_Etagenhoehe5", "var_Etagenhoehe6", "var_Etagenhoehe7", "var_Etagenhoehe8"] });
        }
    }

    private void ValidateCarFlooring(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        string bodentyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bodentyp");
        string bodenProfil = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_BoPr");
        string bodenBelag = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bodenbelag");
        double bodenBlech = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Bodenblech");
        double bodenBelagHoehe = GetFlooringHeight(bodenBelag);
        double bodenHoehe = -1;

        switch (bodentyp)
        {
            case "standard":
                bodenHoehe = 83;
                ParameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("3");
                ParameterDictionary["var_BoPr"].AutoUpdateParameterValue("80 x 40 x 3");
                break;
            case "verstärkt":
                if (bodenBlech <= 0)
                    SetDefaultReinforcedFloor(name);
                if (string.IsNullOrWhiteSpace(bodenProfil))
                    SetDefaultReinforcedFloor(name);
                if (bodenBlech == 3 && bodenProfil == "80 x 40 x 3")
                    SetDefaultReinforcedFloor(name);
                double bodenProfilHoehe = GetFloorProfilHeight(bodenProfil);
                bodenHoehe = bodenBlech + bodenProfilHoehe;
                break;
            case "standard mit Wanne":
                ParameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("5");
                ParameterDictionary["var_BoPr"].AutoUpdateParameterValue("80 x 40 x 3");
                bodenHoehe = 85;
                break;
            case "sonder":
                if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Bodenblech"].Value))
                    ParameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("(keine Auswahl)");
                if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_BoPr"].Value))
                    ParameterDictionary["var_BoPr"].AutoUpdateParameterValue("(keine Auswahl)");
                bodenHoehe = -1;
                break;
            case "extern":
                if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Bodenblech"].Value))
                    ParameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("(keine Auswahl)");
                if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_BoPr"].Value))
                    ParameterDictionary["var_BoPr"].AutoUpdateParameterValue("(keine Auswahl)");
                bodenHoehe = -1;
                break;
            default:
                if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Bodenblech"].Value))
                    ParameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("(keine Auswahl)");
                if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_BoPr"].Value))
                    ParameterDictionary["var_BoPr"].AutoUpdateParameterValue("(keine Auswahl)");
                break;
        }

        ParameterDictionary["var_Bodenbelagsgewicht"].AutoUpdateParameterValue(GetFloorWeight(bodenBelag));
        if (bodenHoehe != -1)
        {
            ParameterDictionary["var_KU"].AutoUpdateParameterValue(Convert.ToString(bodenHoehe + bodenBelagHoehe));
        }
        ParameterDictionary["var_Bodenbelagsdicke"].AutoUpdateParameterValue(Convert.ToString(bodenBelagHoehe));

        double GetFloorProfilHeight(string bodenProfil)
        {
            if (string.IsNullOrEmpty(bodenProfil))
                return 0;
            var profile = _parametercontext.Set<CarFloorProfile>().FirstOrDefault(x => x.Name == bodenProfil);
            if (profile is null)
                return 0;
            return (double)profile.Height!;
        }

        string GetFloorWeight(string bodenBelag)
        {
            if (string.IsNullOrEmpty(bodenBelag))
                return "0";
            if (string.Equals(bodenBelag, "bauseits lt. Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bodenbelagsgewicht");
            if (string.Equals(bodenBelag, "Nach Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bodenbelagsgewicht");
            var boden = _parametercontext.Set<CarFlooring>().FirstOrDefault(x => x.Name == bodenBelag);
            if (boden is null)
                return "0";
            return boden.WeightPerSquareMeter.ToString()!;
        }

        double GetFlooringHeight(string bodenBelag)
        {
            if (string.IsNullOrEmpty(bodenBelag))
                return 0;
            if (string.Equals(bodenBelag, "bauseits lt. Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Bodenbelagsdicke");
            if (string.Equals(bodenBelag, "Nach Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Bodenbelagsdicke");
            var boden = _parametercontext.Set<CarFlooring>().FirstOrDefault(x => x.Name == bodenBelag);
            if (boden is null)
                return 0;
            return (double)boden.Thickness!;
        }

        void SetDefaultReinforcedFloor(string name)
        {
            ParameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("3");
            ParameterDictionary["var_BoPr"].AutoUpdateParameterValue("80 x 50 x 5");
            if (name == "var_BoPr")
            {
                ParameterDictionary["var_BoPr"].DropDownList.Add(new SelectionValue(-1, "Refresh", "Refresh"));
                ParameterDictionary["var_BoPr"].DropDownList.Remove(new SelectionValue(-1, "Refresh", "Refresh"));
            }
        }
    }

    private void ValidateCarEntranceRightSide(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        double kabinenBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KBI");
        double kabinenTiefe = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KTI");
        bool zugangA = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_A");
        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_D");
        double linkeSeite;
        double tuerBreite;

        switch (name)
        {
            case "var_L1" or "var_TB":
                if (!(kabinenBreite > 0))
                    return;
                if (!zugangA)
                {
                    ParameterDictionary["var_R1"].AutoUpdateParameterValue("0");
                    ParameterDictionary["var_L1"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L1");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB");
                    ParameterDictionary["var_R1"].AutoUpdateParameterValue(Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_L2" or "var_TB_C":
                if (!(kabinenBreite > 0))
                    return;
                if (!zugangC)
                {
                    ParameterDictionary["var_R2"].AutoUpdateParameterValue("0");
                    ParameterDictionary["var_L2"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L2");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB_C");
                    ParameterDictionary["var_R2"].AutoUpdateParameterValue(Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_L3" or "var_TB_B":
                if (!(kabinenTiefe > 0))
                    return;
                if (!zugangB)
                {
                    ParameterDictionary["var_R3"].AutoUpdateParameterValue("0");
                    ParameterDictionary["var_L3"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L3");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB_B");
                    ParameterDictionary["var_R3"].AutoUpdateParameterValue(Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_L4" or "var_TB_D":
                if (!(kabinenTiefe > 0))
                    return;
                if (!zugangD)
                {
                    ParameterDictionary["var_R4"].AutoUpdateParameterValue("0");
                    ParameterDictionary["var_L4"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L4");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB_D");
                    ParameterDictionary["var_R4"].AutoUpdateParameterValue(Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_KBI":
                if (!(kabinenBreite > 0))
                    return;
                if (!zugangA)
                {
                    ParameterDictionary["var_R1"].AutoUpdateParameterValue("0");
                    ParameterDictionary["var_L1"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L1");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB");
                    ParameterDictionary["var_R1"].AutoUpdateParameterValue(Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite)));
                }
                if (!zugangC)
                {
                    ParameterDictionary["var_R2"].AutoUpdateParameterValue("0");
                    ParameterDictionary["var_L2"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L2");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB_C");
                    ParameterDictionary["var_R2"].AutoUpdateParameterValue(Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_KTI":
                if (!(kabinenTiefe > 0))
                    return;
                if (!zugangB)
                {
                    ParameterDictionary["var_R3"].AutoUpdateParameterValue("0");
                    ParameterDictionary["var_L3"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L3");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB_B");
                    ParameterDictionary["var_R3"].AutoUpdateParameterValue(Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite)));
                }
                if (!zugangD)
                {
                    ParameterDictionary["var_R4"].AutoUpdateParameterValue("0");
                    ParameterDictionary["var_L4"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L4");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB_D");
                    ParameterDictionary["var_R4"].AutoUpdateParameterValue(Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite)));
                }
                return;
            default:
                return;
        }
    }

    private void ValidateVariableCarDoors(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        bool variableTuerdaten = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_Variable_Tuerdaten");
        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_D");

        if (variableTuerdaten)
        {
            if (!zugangB)
                RemoveCarDoorData("B");
            if (!zugangC)
                RemoveCarDoorData("C");
            if (!zugangD)
                RemoveCarDoorData("D");
            return;
        }
        if (zugangB)
        {
            SetCarDoorData("B");
        }
        else
        {
            RemoveCarDoorData("B");
        }
        if (zugangC)
        {
            SetCarDoorData("C");
        }
        else
        {
            RemoveCarDoorData("C");
        }
        if (zugangD)
        {
            SetCarDoorData("D");
        }
        else
        {
            RemoveCarDoorData("D");
        }
    }

    private void ValidateCarFrameSelection(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var liftTypes = _parametercontext.Set<LiftType>().ToList();
        var currentLiftType = liftTypes.FirstOrDefault(x => x.Name == value);

        var driveTypeId = currentLiftType is not null ? currentLiftType.DriveTypeId : 1;

        var carframes = _parametercontext.Set<CarFrameType>().ToList();

        var availableCarframes = carframes.Where(x => x.DriveTypeId == driveTypeId).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });

        if (availableCarframes is not null)
        {
            UpdateDropDownList("var_Bausatz", availableCarframes);
        }

        CheckListContainsValue(ParameterDictionary["var_Bausatz"]);
    }

    private void ValidateReducedProtectionSpaces(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (name == "var_Ersatzmassnahmen")
        {
            if (string.IsNullOrWhiteSpace(value) || value == "keine")
            {
                ParameterDictionary["var_ErsatzmassnahmenSK"].AutoUpdateParameterValue("False");
                ParameterDictionary["var_ErsatzmassnahmenSG"].AutoUpdateParameterValue("False");
            }
            else if (value == "Vorausgelöstes Anhaltesystem" || value.StartsWith("Schachtkopf und Schachtgrube"))
            {
                ParameterDictionary["var_ErsatzmassnahmenSK"].AutoUpdateParameterValue("True");
                ParameterDictionary["var_ErsatzmassnahmenSG"].AutoUpdateParameterValue("True");
            }
            else if (value.StartsWith("Schachtkopf"))
            {
                ParameterDictionary["var_ErsatzmassnahmenSK"].AutoUpdateParameterValue("True");
                ParameterDictionary["var_ErsatzmassnahmenSG"].AutoUpdateParameterValue("False");
            }
            else if (value.StartsWith("Schachtgrube"))
            {
                ParameterDictionary["var_ErsatzmassnahmenSK"].AutoUpdateParameterValue("False");
                ParameterDictionary["var_ErsatzmassnahmenSG"].AutoUpdateParameterValue("True");
            }
        }

        var selectedSafetyGear = ParameterDictionary["var_TypFV"].Value;
        var selectedReducedProtectionSpace = ParameterDictionary["var_Ersatzmassnahmen"].Value;

        if (name == "var_TypFV")
        {
            var reducedProtectionSpaces = _parametercontext.Set<ReducedProtectionSpace>().ToList();
            IEnumerable<SelectionValue> availablEReducedProtectionSpaces;

            if (string.IsNullOrWhiteSpace(selectedSafetyGear))
            {
                availablEReducedProtectionSpaces = reducedProtectionSpaces.Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
                ;
            }
            else if (selectedSafetyGear.Contains("BS"))
            {
                availablEReducedProtectionSpaces = reducedProtectionSpaces.Where(x => x.Name.Contains(selectedSafetyGear) || x.Name == "keine").Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
            }
            else if (selectedSafetyGear.Contains("PC 13") || selectedSafetyGear.Contains("PC 24") || selectedSafetyGear.Contains("CSGB01") || selectedSafetyGear.Contains("CSGB02"))
            {
                availablEReducedProtectionSpaces = reducedProtectionSpaces.Where(x => !x.Name.Contains("ESG")).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
            }
            else
            {
                availablEReducedProtectionSpaces = reducedProtectionSpaces.Where(x => !x.Name.Contains("ESG") && !x.Name.Contains("Voraus")).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });
            }

            if (availablEReducedProtectionSpaces is not null)
            {
                UpdateDropDownList("var_Ersatzmassnahmen", availablEReducedProtectionSpaces);
            }
        }

        if (!string.IsNullOrWhiteSpace(selectedSafetyGear) &&
            !string.IsNullOrWhiteSpace(selectedReducedProtectionSpace))
        {
            var selectedReducedProtectionSpaceValue = LiftParameterHelper.GetDropDownListValue(ParameterDictionary["var_Ersatzmassnahmen"].DropDownList, selectedReducedProtectionSpace);
            if (selectedReducedProtectionSpaceValue == null || selectedReducedProtectionSpaceValue.Id == -1)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Ersatzmassnahmen sind mit der Fangvorrichtung {selectedSafetyGear} nicht zulässig!", SetSeverity(severity))
                { DependentParameter = [optional!] });
            }
            else
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = [optional!] });
            }

            //if (!ParameterDictionary["var_Ersatzmassnahmen"].DropDownList.Contains(selectedReducedProtectionSpace))
            //{
            //    ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Ersatzmassnahmen sind mit der Fangvorrichtung {selectedSafetyGear} nicht zulässig!", SetSeverity(severity))
            //    { DependentParameter = [optional!] });
            //}
            //else
            //{
            //    ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = [optional!] });
            //}
        }
    }

    private void ValidateSafetyGear(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var safetyGears = _parametercontext.Set<SafetyGearModelType>().ToList();
        var selectedSafetyGear = ParameterDictionary["var_TypFV"].Value;
        IEnumerable<SelectionValue> availablseafetyGears = value switch
        {
            "keine" => [],
            "Sperrfangvorrichtung" => safetyGears.Where(x => x.SafetyGearTypeId == 1).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified }),
            "Bremsfangvorrichtung" => safetyGears.Where(x => x.SafetyGearTypeId == 2).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified }),
            "Rohrbruchventil" => safetyGears.Where(x => x.SafetyGearTypeId == 3).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified }),
            _ => safetyGears.Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified }),
        };
        if (availablseafetyGears is not null)
        {
            UpdateDropDownList("var_TypFV", availablseafetyGears);
            if (!string.IsNullOrWhiteSpace(selectedSafetyGear) && !availablseafetyGears.Any(x => x.Name == selectedSafetyGear))
            {
                ParameterDictionary["var_TypFV"].AutoUpdateParameterValue(string.Empty);
            }
        }
    }

    private void ValidateSafetyRange(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_Q"].Value) ||
            string.IsNullOrWhiteSpace(ParameterDictionary["var_F"].Value) ||
            ParameterDictionary["var_F"].Value == "0" ||
            string.IsNullOrWhiteSpace(ParameterDictionary["var_Fuehrungsart"].Value) ||
            string.IsNullOrWhiteSpace(ParameterDictionary["var_FuehrungsschieneFahrkorb"].Value) ||
            string.IsNullOrWhiteSpace(ParameterDictionary["var_TypFV"].Value))
            return;

        var safetygearResult = _calculationsModuleService.GetSafetyGearCalculation(ParameterDictionary);
        if (safetygearResult is not null)
        {
            if (safetygearResult.PipeRuptureValve)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
                return;
            }

            if (!safetygearResult.RailHeadAllowed)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählter Schienenkopf ist für diese Fangvorrichtung nicht zulässig.", SetSeverity(severity)));
                return;
            }

            var load = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_Q");
            var carWeight = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_F");

            if (safetygearResult.MinLoad > load + carWeight)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Fangvorrichtung nicht zulässig Minimalgewicht {safetygearResult.MinLoad} kg | Nutzlast+Fahrkorbgewicht: {load + carWeight} kg unterschritten.", SetSeverity(severity)));
                return;
            }
            if (safetygearResult.MaxLoad < load + carWeight)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Fangvorrichtung nicht zulässig Maximalgewicht {safetygearResult.MaxLoad} kg | Nutzlast+Fahrkorbgewicht: {carWeight + load} kg überschritten.", SetSeverity(severity)));
                return;
            }
        }
    }

    private void ValidateDriveSystemTypes(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var driveSystems = _parametercontext.Set<DriveSystem>().Include(i => i.DriveSystemType)
                                                               .ToList();
        var currentdriveSystem = driveSystems.FirstOrDefault(x => x.Name == value);
        ParameterDictionary["var_Getriebe"].AutoUpdateParameterValue(currentdriveSystem is not null ? currentdriveSystem.DriveSystemType!.Name : string.Empty);
        ParameterDictionary["var_Getriebe"].AutoUpdateParameterValue(ParameterDictionary["var_Getriebe"].Value);
    }

    private void ValidateCarweightWithoutFrame(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        int carFrameWeight = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_Rahmengewicht");
        string? fangrahmenTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bausatz");

        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_Rahmengewicht"].Value) && !string.IsNullOrWhiteSpace(fangrahmenTyp))
        {
            var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == fangrahmenTyp);
            if (carFrameType is not null)
            {
                carFrameWeight = carFrameType.CarFrameWeight;
            }
        }

        if (carFrameWeight >= 0)
        {
            int carWeight = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_F");
            if (carWeight > 0)
            {
                ParameterDictionary["var_KabTueF"].AutoUpdateParameterValue(Convert.ToString(carWeight - carFrameWeight));
            }
        }
    }

    private void ValidateCorrectionWeight(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        try
        {
            if (Math.Abs(Convert.ToInt16(value)) > 10)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{displayname} ist größer +-10 kg überprüfen Sie die Eingabe", SetSeverity(severity)));
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private void ValidateCarArea(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (!string.IsNullOrWhiteSpace(value) && !string.Equals(value, "0"))
        {
            if (string.Equals(LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Normen"), "MRL 2006/42/EG"))
                return;
            double load = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Q");
            double reducedLoad = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Q1");
            double area = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_A_Kabine");
            string lift = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Aufzugstyp");
            var cargoTypDB = _parametercontext.Set<LiftType>().Include(i => i.CargoType)
                                                            .ToList()
                                                            .FirstOrDefault(x => x.Name == lift);
            var cargotyp = cargoTypDB is not null ? cargoTypDB.CargoType!.Name! : "Aufzugstyp noch nicht gewählt !";
            string drivesystem = GetDriveSystem(lift);
            if (string.Equals(cargotyp, "Lastenaufzug") && string.Equals(drivesystem, "Hydraulik"))
            {
                var loadTable6 = _calculationsModuleService.GetLoadFromTable(area, "Tabelle6");

                if (reducedLoad < loadTable6)
                    ParameterDictionary["var_Q1"].AutoUpdateParameterValue(Convert.ToString(loadTable6));
            }
            else
            {
                ParameterDictionary["var_Q1"].AutoUpdateParameterValue(Convert.ToString(load));
            }

            if (!_calculationsModuleService.ValdidateLiftLoad(load, area, cargotyp, drivesystem))
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Nennlast enspricht nicht der EN81:20!", SetSeverity(severity)) { DependentParameter = ["var_Aufzugstyp", "var_Q", "var_A_Kabine"] });
            }
            else
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = ["var_Aufzugstyp", "var_Q", "var_A_Kabine"] });
            }
        }
    }

    private void ValidateUCMValues(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_Aggregat"].Value)
            || ParameterDictionary["var_Aggregat"].Value != "Ziehl-Abegg"
            || string.IsNullOrWhiteSpace(ParameterDictionary["var_Steuerungstyp"].Value))
        {
            ParameterDictionary["var_Erkennungsweg"].AutoUpdateParameterValue("0");
            ParameterDictionary["var_Totzeit"].AutoUpdateParameterValue("0");
            ParameterDictionary["var_Vdetektor"].AutoUpdateParameterValue("0");

            if (name == "var_Steuerungstyp" && ParameterDictionary["var_Aggregat"].Value == "Ziehl-Abegg" && string.IsNullOrWhiteSpace(ParameterDictionary["var_Steuerungstyp"].Value))
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"UCM-Daten können ohne Steuerungsauswahl nicht berechnet werden.", SetSeverity("Warning")));
            }
            return;
        }

        var currentLiftControlManufacturers = _parametercontext.Set<LiftControlManufacturer>().FirstOrDefault(x => x.Name == ParameterDictionary["var_Steuerungstyp"].Value);

        if (ParameterDictionary["var_Schachtinformationssystem"].Value == "Limax 33CP"
            || ParameterDictionary["var_Schachtinformationssystem"].Value == "NEW-Lift S1-Box"
            || ParameterDictionary["var_Schachtinformationssystem"].Value == "NEW-Lift S2 (FST-3)")
        {
            ParameterDictionary["var_Erkennungsweg"].AutoUpdateParameterValue(Convert.ToString(currentLiftControlManufacturers?.DetectionDistanceSIL3));
            ParameterDictionary["var_Totzeit"].AutoUpdateParameterValue(Convert.ToString(currentLiftControlManufacturers?.DeadTimeSIL3));
            ParameterDictionary["var_Vdetektor"].AutoUpdateParameterValue(Convert.ToString(currentLiftControlManufacturers?.SpeeddetectorSIL3));
        }
        else
        {
            var oldTotzeit = ParameterDictionary["var_Totzeit"].Value;
            var oldVdetektor = ParameterDictionary["var_Vdetektor"].Value;
            var newTotzeit = Convert.ToBoolean(ParameterDictionary["var_ElektrBremsenansteuerung"].Value) ?
                Convert.ToString(currentLiftControlManufacturers?.DeadTimeZAsbc4) :
                Convert.ToString(currentLiftControlManufacturers?.DeadTime);
            var newVdetektor = Convert.ToString(currentLiftControlManufacturers?.Speeddetector);

            if (oldTotzeit == newTotzeit && oldVdetektor == newVdetektor)
            {
                return;
            }

            ParameterDictionary["var_Erkennungsweg"].AutoUpdateParameterValue(Convert.ToString(currentLiftControlManufacturers?.DetectionDistance));
            ParameterDictionary["var_Totzeit"].AutoUpdateParameterValue(newTotzeit);
            ParameterDictionary["var_Vdetektor"].AutoUpdateParameterValue(newVdetektor);
        }
    }

    private void ValidateZAliftData(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(FullPathXml) || FullPathXml == pathDefaultAutoDeskTransfer)
        {
            return;
        }

        var zaHtmlPath = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html");
        if (!File.Exists(zaHtmlPath))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_Aggregat"].Value))
        {
            ParameterDictionary["var_Aggregat"].AutoUpdateParameterValue("Ziehl-Abegg");
        }

        if (!string.Equals(ParameterDictionary["var_Aggregat"].Value, "Ziehl-Abegg"))
        {
            return;
        }

        var lastWriteTime = File.GetLastWriteTime(zaHtmlPath);

        if (lastWriteTime != ZaHtmlCreationTime)
        {
            var zaliftHtml = new HtmlDocument();
            zaliftHtml.Load(zaHtmlPath);
            var zliData = zaliftHtml.DocumentNode.SelectNodes("//comment()").FirstOrDefault(x => x.InnerHtml.StartsWith("<!-- zli"))?
                                                                            .InnerHtml.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (zliData is null)
            {
                return;
            }

            ZliDataDictionary.Clear();

            foreach (var zlipar in zliData)
            {

                if (!string.IsNullOrWhiteSpace(zlipar) && zlipar.Contains('='))
                {
                    var zliPairValue = zlipar.Split('=');

                    if (!ZliDataDictionary.ContainsKey(zliPairValue[0]))
                    {
                        ZliDataDictionary.Add(zliPairValue[0], zliPairValue[1]);
                    }
                }
            }

            var htmlNodes = zaliftHtml.DocumentNode.SelectNodes("//tr");

            if (htmlNodes is not null)
            {
                ZliDataDictionary.Add("ElektrBremsenansteuerung", htmlNodes.Any(x => x.InnerText.StartsWith("Bremsansteuermodul")).ToString());
            }
            else
            {
                ZliDataDictionary.Add("ElektrBremsenansteuerung", "False");
            }

            var detectionDistanceMeter = htmlNodes?.FirstOrDefault(x => x.InnerText.StartsWith("Erkennungsweg") || x.InnerText.StartsWith("Detection distance"))?.ChildNodes[1].InnerText;
            if (!string.IsNullOrWhiteSpace(detectionDistanceMeter))
            {
                ZliDataDictionary.Add("DetectionDistance", (Convert.ToDouble(detectionDistanceMeter.Replace("m", "").Trim(), CultureInfo.CurrentCulture) * 1000).ToString());
            }

            var deadTime = htmlNodes?.FirstOrDefault(x => x.InnerText.StartsWith("Totzeit") || x.InnerText.StartsWith("Dead time"))?.ChildNodes[1].InnerText.Replace("ms", "").Trim();
            if (!string.IsNullOrWhiteSpace(deadTime))
            {
                ZliDataDictionary.Add("DeadTime", deadTime);
            }

            var vDetector = Convert.ToDouble(htmlNodes?.FirstOrDefault(x => x.InnerText.StartsWith("V Detektor") || x.InnerText.StartsWith("V Detector"))?.ChildNodes[1].InnerText.Replace("m/s", "").Trim(), CultureInfo.CurrentCulture).ToString();
            if (!string.IsNullOrWhiteSpace(vDetector))
            {
                ZliDataDictionary.Add("VDetector", vDetector);
            }

            ZaHtmlCreationTime = lastWriteTime;
        }

        var zaLiftValue = string.Empty;
        var zaLiftValue2 = string.Empty;
        var brakerelease = string.Empty;

        var searchString = name switch
        {
            "var_Q" => "Nennlast_Q",
            "var_F" => "Fahrkorbgewicht_F",
            "var_FH" => "Anlage-FH",
            "var_Fremdbelueftung" => "Motor-Fan",
            "var_ElektrBremsenansteuerung" => "ElektrBremsenansteuerung",
            "var_Treibscheibegehaertet" => "Treibscheibe-RF",
            "var_Handlueftung" => "Bremse-Handlueftung",
            "var_Erkennungsweg" => "DetectionDistance",
            "var_Totzeit" => "DeadTime",
            "var_Vdetektor" => "VDetector",
            _ => string.Empty,
        };

        ZliDataDictionary.TryGetValue(searchString, out zaLiftValue);

        if (string.IsNullOrWhiteSpace(zaLiftValue))
        {
            return;
        }

        if (name == "var_Handlueftung")
        {
            ZliDataDictionary.TryGetValue("Bremse-Lueftueberwachung", out zaLiftValue2);
            if (string.IsNullOrWhiteSpace(zaLiftValue2))
                return;
            if (zaLiftValue == "ohne Handlueftung" && zaLiftValue2 == "Mikroschalter")
                brakerelease = "207 V Bremse. ohne Handl. Mikrosch.";
            if (zaLiftValue == "ohne Handlueftung" && zaLiftValue2 == "Naeherungsschalter")
                brakerelease = "207 V Bremse. ohne Hand. Indukt. NS";
            if (zaLiftValue == "mit Handlueftung" && zaLiftValue2 == "Mikroschalter")
                brakerelease = "207 V Bremse. mit Handl. Mikrosch.";
            if (zaLiftValue == "mit Handlueftung" && zaLiftValue2 == "Naeherungsschalter")
                brakerelease = "207 V Bremse. mit Handl. induktiver NS";
            if (zaLiftValue == "fuer Bowdenzug" && zaLiftValue2 == "Mikroschalter")
                brakerelease = "207 V Bremse. v. für Bowdenz. Handl. Mikrosch.";
            if (zaLiftValue == "fuer Bowdenzug" && zaLiftValue2 == "Naeherungsschalter")
                brakerelease = "207 V Bremse. v. für Bowdenz. Handl. Indukt. NS";
        }

        var isValid = name switch
        {
            "var_Q" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            "var_F" => Math.Abs(Convert.ToInt32(value) - Convert.ToInt32(zaLiftValue)) <= 10,
            "var_FH" => Math.Abs(Convert.ToDouble(value) * 1000 - Convert.ToDouble(zaLiftValue) * 1000) <= 20,
            "var_Fremdbelueftung" => string.Equals(value, Convert.ToString(!zaLiftValue.StartsWith("ohne")), StringComparison.CurrentCultureIgnoreCase),
            "var_ElektrBremsenansteuerung" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            "var_Treibscheibegehaertet" => string.Equals(value, Convert.ToString(zaLiftValue.Contains("gehaertet")), StringComparison.CurrentCultureIgnoreCase),
            "var_Handlueftung" => string.Equals(value, brakerelease, StringComparison.CurrentCultureIgnoreCase),
            "var_Erkennungsweg" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            "var_Totzeit" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            "var_Vdetektor" => string.Equals(value, zaLiftValue, StringComparison.CurrentCultureIgnoreCase),
            _ => true,
        };
        ;

        if (!isValid)
        {
            if (name != "var_Handlueftung")
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Unterschiedliche Werte für >{displayname}<  Wert Spezifikation {value} | Wert ZALiftauslegung {zaLiftValue}", SetSeverity(severity)));
            }
            else
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Unterschiedliche Werte für >{displayname}<  Wert Spezifikation {value} | Wert ZALiftauslegung {zaLiftValue} - {zaLiftValue2} ", SetSeverity(severity)));
            }

        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
        }
    }

    private void ValidateShaftWalls(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (value is null)
            return;

        if (string.Equals(value, "Holz"))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Achtung: Holzschacht - LBO und Türzulassung beachten!\n" +
                $"Schienenbügelbefestigung muß durch bauseitigem Statiker erfolgen!", SetSeverity(severity)));
        }
    }

    private void ValidateGuideModel(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }
        if (!name.StartsWith("var_Fuehrungsart"))
        {
            return;
        }
        var guideModels = _parametercontext.Set<GuideModelType>().ToList();
        var guideTyp = name.Replace("Fuehrungsart", "TypFuehrung");
        var selectedguideModel = ParameterDictionary[guideTyp].Value;
        IEnumerable<SelectionValue> availableguideModels = value switch
        {
            "Gleitführung" => guideModels.Where(x => x.GuideTypeId == 1).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified }),
            "Rollenführung" => guideModels.Where(x => x.GuideTypeId == 2).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified }),
            _ => guideModels.Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified }),
        };
        if (availableguideModels is not null)
        {
            UpdateDropDownList(guideTyp, availableguideModels);
            CheckListContainsValue(ParameterDictionary[guideTyp]);
        }
    }

    private void ValidatePitLadder(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ParameterDictionary["var_SchachtgrubenleiterKontaktgesichert"].AutoUpdateParameterValue("False");
            return;
        }
        ParameterDictionary["var_SchachtgrubenleiterKontaktgesichert"].AutoUpdateParameterValue(value == "Schachtgrubenleiter EN81:20 mit el. Kontakt" ? "True" : "False");
    }

    private void ValidateDoorTyps(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!name.StartsWith("var_Tuertyp"))
            return;

        var liftDoorGroups = name.Replace("var_Tuertyp", "var_Tuerbezeichnung");

        if (string.IsNullOrWhiteSpace(value))
        {
            ParameterDictionary[liftDoorGroups].AutoUpdateParameterValue(string.Empty);
            ParameterDictionary[liftDoorGroups].AutoUpdateParameterValue(null);
            ParameterDictionary[liftDoorGroups].DropDownList.Clear();
        }
        else
        {
            var selectedDoorSytem = value[..1];
            Expression<Func<LiftDoorGroup, bool>> filterDoorSystems;
            if (selectedDoorSytem == "M")
            {
                if (ParameterDictionary[name].DropDownListValue is not null && ParameterDictionary[name].DropDownListValue!.Id == 4)
                {
                    filterDoorSystems = x => x.DoorManufacturer!.StartsWith(selectedDoorSytem) && x.Name.Contains("DT");
                }
                else
                {
                    filterDoorSystems = x => x.DoorManufacturer!.StartsWith(selectedDoorSytem) && !x.Name.Contains("DT");
                }
            }
            else
            {
                filterDoorSystems = x => x.DoorManufacturer!.StartsWith(selectedDoorSytem);
            }

            var availableLiftDoorGroups = _parametercontext.Set<LiftDoorGroup>().Where(filterDoorSystems)
                                                                                .Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName)
                                                                                {
                                                                                    IsFavorite = x.IsFavorite,
                                                                                    SchindlerCertified = x.SchindlerCertified
                                                                                });
            if (availableLiftDoorGroups is not null)
            {
                UpdateDropDownList(liftDoorGroups, availableLiftDoorGroups);
                CheckListContainsValue(ParameterDictionary[liftDoorGroups]);
            }
        }
    }

    private void ValidateDoorData(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!name.StartsWith("var_Tuerbezeichnung"))
            return;

        var liftDoortyp = name.Replace("var_Tuerbezeichnung", "var_Tuertyp");

        var doorOpeningDirection = name.Replace("var_Tuerbezeichnung", "var_Tueroeffnung");
        var doorPanelCount = name.Replace("var_Tuerbezeichnung", "var_AnzahlTuerfluegel");

        if (string.IsNullOrWhiteSpace(value))
        {
            ParameterDictionary[doorOpeningDirection].AutoUpdateParameterValue(string.Empty);
            ParameterDictionary[doorOpeningDirection].AutoUpdateParameterValue(null);
            ParameterDictionary[doorPanelCount].AutoUpdateParameterValue("0");
        }
        else
        {
            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.ShaftDoor)
                                                                      .ThenInclude(t => t!.LiftDoorOpeningDirection)
                                                                      .FirstOrDefault(x => x.Name == value);
            if (liftDoorGroup is not null && liftDoorGroup.ShaftDoor is not null)
            {
                if (liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection is not null)
                {
                    if (string.IsNullOrWhiteSpace(ParameterDictionary[doorOpeningDirection].Value) || !ParameterDictionary[doorOpeningDirection].Value!.StartsWith(liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection.Name))
                    {
                        ParameterDictionary[doorOpeningDirection].AutoUpdateParameterValue(liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection.Name);
                        ParameterDictionary[doorOpeningDirection].AutoUpdateParameterValue(liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection.Name);
                    }
                }
                ParameterDictionary[doorPanelCount].AutoUpdateParameterValue(Convert.ToString(liftDoorGroup.ShaftDoor.DoorPanelCount));
            }
        }
    }

    private void ValidateCarEquipmentPosition(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, name))
            return;

        var zugang = name.Last();
        bool hasSpiegel = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_Spiegel{zugang}");
        bool hasHandlauf = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_Handlauf{zugang}");
        bool hasRammschutz = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_Rammschutz{zugang}");
        bool hasPaneel = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_PaneelPos{zugang}");
        bool hasSchutzgelaender = !string.IsNullOrWhiteSpace(ParameterDictionary[$"var_Schutzgelaender_{zugang}"].Value);
        bool hasTeilungsleiste = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_Teilungsleiste{zugang}");
        bool hasRueckwand = false;
        if (zugang == 'C')
        {
            hasRueckwand = !string.IsNullOrWhiteSpace(ParameterDictionary["var_Rueckwand"].Value);
        }

        if (hasSpiegel || hasHandlauf || hasTeilungsleiste || hasRammschutz || hasPaneel || hasSchutzgelaender || hasRueckwand)
        {
            var errorMessage = $"Bei Zugang {zugang} wurde folgende Ausstattung gewählt:";
            if (hasSpiegel)
                errorMessage += " Spiegel,";
            if (hasHandlauf)
                errorMessage += " Handlauf,";
            if (hasTeilungsleiste)
                errorMessage += " Teilungsleiste,";
            if (hasRammschutz)
                errorMessage += " Rammschutz,";
            if (hasPaneel)
                errorMessage += " Paneel,";
            if (hasSchutzgelaender)
                errorMessage += " Schutzgeländer,";
            if (hasRueckwand)
                errorMessage += " Rückwand,";
            errorMessage += " dies erfordert eine Plausibilitätsprüfung!";

            ValidationResult.Add(new ParameterStateInfo(name, displayname, errorMessage, SetSeverity(severity)));
        }
    }

    private void ValidateLiftPositionSystems(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var controler = ParameterDictionary["var_Steuerungstyp"].Value;
        var liftPositionSystem = ParameterDictionary["var_Schachtinformationssystem"].Value;

        if (string.IsNullOrWhiteSpace(controler) || string.IsNullOrWhiteSpace(liftPositionSystem))
            return;

        var validSystem = liftPositionSystem switch
        {
            "Limax 33CP" => string.Equals(controler, "Kühn MSZ 9E"),
            "NEW-Lift S1-Box" => string.Equals(controler, "New-Lift FST-2 XT") || string.Equals(controler, "New-Lift FST-2 S"),
            "NEW-Lift S2 (FST-3)" => string.Equals(controler, "New-Lift FST-3"),
            _ => true
        };

        if (validSystem)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = [optional!] });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewähltes Schachtinformationssystem: {liftPositionSystem} ist mit der Steuerung: {controler} nicht zulässig!", SetSeverity(severity))
            { DependentParameter = [optional!] });
        }
    }

    private void ValidateFloorColorTyps(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ParameterDictionary["var_BodenbelagsTyp"].AutoUpdateParameterValue(string.Empty);
            ParameterDictionary["var_BodenbelagsTyp"].AutoUpdateParameterValue(string.Empty);
            ParameterDictionary["var_BodenbelagsTyp"].DropDownList.Clear();
            return;
        }

        //var floorColors = _parametercontext.Set<CarFloorColorTyp>().Include(i => i.CarFlooring).ToList();

        //IEnumerable<SelectionValue> availableFloorColors = floorColors.Where(x => x.CarFlooring?.Name == value).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });

        IEnumerable<SelectionValue> availableFloorColors = _parametercontext.Set<CarFloorColorTyp>().Include(i => i.CarFlooring).Where(x => x.CarFlooring!.Name == value).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified });


        if (availableFloorColors is not null)
        {
            if (!availableFloorColors.Any())
            {
                ParameterDictionary["var_BodenbelagsTyp"].AutoUpdateParameterValue(string.Empty);
            }
            UpdateDropDownList("var_BodenbelagsTyp", availableFloorColors);
            CheckListContainsValue(ParameterDictionary["var_BodenbelagsTyp"]);
        }
    }

    private void ValidateEntryDimensions(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_AutogenerateFloorDoorData"))
            return;

        var zugang = name.StartsWith("var_TB") ? string.Equals(name[^1..], "_B") || string.Equals(name[^1..], "_C") || string.Equals(name[^1..], "_D") ? name[^1..] : "A"
                                               : string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";

        if (name.StartsWith("var_TuerEinbau"))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ParameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue(string.Empty);
                return;
            }

            var liftDoor = ParameterDictionary[zugang == "A" ? "var_Tuerbezeichnung" : $"var_Tuerbezeichnung_{zugang}"].Value;
            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor).FirstOrDefault(x => x.Name == liftDoor);
            var doorEntrance = Convert.ToDouble(value, CultureInfo.CurrentCulture);
            if (liftDoorGroup is null || liftDoorGroup.CarDoor is null || liftDoorGroup.CarDoor.SillWidth >= doorEntrance)
            {
                ParameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue(string.Empty);
                ParameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(string.Empty);
                return;
            }
            ParameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue((doorEntrance - liftDoorGroup.CarDoor.SillWidth).ToString());
            ParameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(liftDoorGroup.CarDoor.SillWidth.ToString());
            SetCarDesignParameterSill(zugang, liftDoorGroup);
        }
        else if (name.StartsWith("var_Tuerbezeichnung"))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ParameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue(string.Empty);
                ParameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(string.Empty);
                return;
            }

            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor).FirstOrDefault(x => x.Name == value);
            var doorEntranceString = ParameterDictionary[zugang == "A" ? "var_TuerEinbau" : $"var_TuerEinbau{zugang}"].Value;
            if (!string.IsNullOrWhiteSpace(doorEntranceString))
            {
                var doorEntrance = Convert.ToDouble(doorEntranceString, CultureInfo.CurrentCulture);
                if (liftDoorGroup is null || liftDoorGroup.CarDoor is null || liftDoorGroup.CarDoor.SillWidth >= doorEntrance)
                {
                    ParameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue(string.Empty);
                    ParameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(string.Empty);
                    return;
                }
                ParameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue((doorEntrance - liftDoorGroup.CarDoor.SillWidth).ToString());
                ParameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(liftDoorGroup.CarDoor.SillWidth.ToString());
                SetCarDesignParameterSill(zugang, liftDoorGroup);
            }
        }
        else if (name.StartsWith("var_SchwellenprofilKab") || name.StartsWith("var_TB") || name.StartsWith("var_Tueroeffnung"))
        {
            if (string.IsNullOrWhiteSpace(value) || value == "0")
                return;

            var liftDoor = ParameterDictionary[zugang == "A" ? "var_Tuerbezeichnung" : $"var_Tuerbezeichnung_{zugang}"].Value;
            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor).FirstOrDefault(x => x.Name == liftDoor);
            if (liftDoorGroup is not null)
            {
                SetCarDesignParameterSill(zugang, liftDoorGroup);
            }
        }
    }

    private void ValidateHydrauliclock(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.Equals(value, "False", StringComparison.CurrentCultureIgnoreCase))
            ParameterDictionary["var_AufsetzvorrichtungSystem"].AutoUpdateParameterValue(string.Empty);
    }

    private void ValidateCounterweightMass(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        string lift = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Aufzugstyp");
        string drivesystem = GetDriveSystem(lift);

        double cwtLoad;
        double cwtFillingLoad;
        double cwtFillingHeight;

        if (string.IsNullOrWhiteSpace(drivesystem) || string.Equals(drivesystem, "Hydraulik"))
        {
            cwtLoad = 0;
            cwtFillingLoad = 0;
            cwtFillingHeight = 0;
        }
        else
        {
            double load = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Q");
            double carWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_F");
            double balance = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_GGWNutzlastausgleich");
            double cwtWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Gegengewicht_Einlagenbreite");
            double cwtDepth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Gegengewicht_Einlagentiefe");
            double cwtFrameWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_GGW_Rahmen_Gewicht");
            cwtLoad = Math.Round(load * balance + carWeight);
            cwtFillingLoad = cwtLoad - cwtFrameWeight;
            double fillingDensity = (cwtWidth * cwtDepth * 7.85d * 0.000001);
            cwtFillingHeight = fillingDensity > 0 ? Math.Round(cwtFillingLoad / fillingDensity) : 0;
        }
        ParameterDictionary["var_Gegengewichtsmasse"].AutoUpdateParameterValue(Convert.ToString(cwtLoad));
        ParameterDictionary["var_GGW_Fuellgewicht"].AutoUpdateParameterValue(Convert.ToString(cwtFillingLoad));
        ParameterDictionary["var_GGW_Fuellhoehe"].AutoUpdateParameterValue(Convert.ToString(cwtFillingHeight));
    }

    private void ValidateProtectiveRailingSwitch(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        string[] sides = ["var_Schutzgelaender_A", "var_Schutzgelaender_B", "var_Schutzgelaender_C", "var_Schutzgelaender_D"];
        bool railingSwitch = false;

        foreach (var side in sides)
        {
            if (!string.IsNullOrWhiteSpace(ParameterDictionary[side].Value))
            {
                if (ParameterDictionary[side].Value!.Contains("klappbar") || ParameterDictionary[side].Value!.Contains("steckbar"))
                {
                    railingSwitch = true;
                    break;
                }
            }
        }
        ParameterDictionary["var_SchutzgelaenderKontakt"].AutoUpdateParameterValue(railingSwitch ? "True" : "False");
    }

    private void ValidateMirrorDimensions(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_AutoDimensionsMirror"))
        {
            return;
        }
        List<string> mirrors = [];
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SpiegelA"))
        {
            mirrors.Add("A");
        }
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SpiegelB"))
        {
            mirrors.Add("B");
        }
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SpiegelC"))
        {
            mirrors.Add("C");
        }
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_SpiegelD"))
        {
            mirrors.Add("D");
        }
        var mirrorWidth = string.Empty;
        var mirrorHeight = string.Empty;
        var mirrorWidth2 = string.Empty;
        var mirrorHeight2 = string.Empty;
        var mirrorWidth3 = string.Empty;
        var mirrorHeight3 = string.Empty;
        var mirrorDistanceLeft = string.Empty;
        var mirrorDistanceLeft2 = string.Empty;
        var mirrorDistanceLeft3 = string.Empty;
        var mirrorDistanceCeiling = string.Empty;
        var mirrorDistanceCeiling2 = string.Empty;
        var mirrorDistanceCeiling3 = string.Empty;

        if (mirrors.Count == 1)
        {
            var mirrorWidthSet = _calculationsModuleService.GetMirrorWidth(ParameterDictionary, mirrors[0], 1);
            var mirrorHeightSet = _calculationsModuleService.GetMirrorHeight(ParameterDictionary, mirrors[0], 1);

            mirrorWidth = mirrorWidthSet.Item1.ToString();
            mirrorHeight = mirrorHeightSet.Item1.ToString();
            mirrorDistanceLeft = mirrorWidthSet.Item2.ToString();
            mirrorDistanceCeiling = mirrorHeightSet.Item2.ToString();
        }
        else if (mirrors.Count == 2)
        {
            var mirrorWidthSet = _calculationsModuleService.GetMirrorWidth(ParameterDictionary, mirrors[0], 1);
            var mirrorHeightSet = _calculationsModuleService.GetMirrorHeight(ParameterDictionary, mirrors[0], 1);
            var mirrorWidthSet2 = _calculationsModuleService.GetMirrorWidth(ParameterDictionary, mirrors[1], 2);
            var mirrorHeightSet2 = _calculationsModuleService.GetMirrorHeight(ParameterDictionary, mirrors[1], 2);

            mirrorWidth = mirrorWidthSet.Item1.ToString();
            mirrorHeight = mirrorHeightSet.Item1.ToString();
            mirrorDistanceLeft = mirrorWidthSet.Item2.ToString();
            mirrorDistanceCeiling = mirrorHeightSet.Item2.ToString();

            mirrorWidth2 = mirrorWidthSet2.Item1.ToString();
            mirrorHeight2 = mirrorHeightSet2.Item1.ToString();
            mirrorDistanceLeft2 = mirrorWidthSet2.Item2.ToString();
            mirrorDistanceCeiling2 = mirrorHeightSet2.Item2.ToString();
        }
        else if (mirrors.Count == 3)
        {
            var mirrorWidthSet = _calculationsModuleService.GetMirrorWidth(ParameterDictionary, mirrors[0], 1);
            var mirrorHeightSet = _calculationsModuleService.GetMirrorHeight(ParameterDictionary, mirrors[0], 1);
            var mirrorWidthSet2 = _calculationsModuleService.GetMirrorWidth(ParameterDictionary, mirrors[1], 2);
            var mirrorHeightSet2 = _calculationsModuleService.GetMirrorHeight(ParameterDictionary, mirrors[1], 2);
            var mirrorWidthSet3 = _calculationsModuleService.GetMirrorWidth(ParameterDictionary, mirrors[2], 3);
            var mirrorHeightSet3 = _calculationsModuleService.GetMirrorHeight(ParameterDictionary, mirrors[2], 3);

            mirrorWidth = mirrorWidthSet.Item1.ToString();
            mirrorHeight = mirrorHeightSet.Item1.ToString();
            mirrorDistanceLeft = mirrorWidthSet.Item2.ToString();
            mirrorDistanceCeiling = mirrorHeightSet.Item2.ToString();

            mirrorWidth2 = mirrorWidthSet2.Item1.ToString();
            mirrorHeight2 = mirrorHeightSet2.Item1.ToString();
            mirrorDistanceLeft2 = mirrorWidthSet2.Item2.ToString();
            mirrorDistanceCeiling2 = mirrorHeightSet2.Item2.ToString();

            mirrorWidth3 = mirrorWidthSet3.Item1.ToString();
            mirrorHeight3 = mirrorHeightSet3.Item1.ToString();
            mirrorDistanceLeft3 = mirrorWidthSet3.Item2.ToString();
            mirrorDistanceCeiling3 = mirrorHeightSet3.Item2.ToString();
        }

        ParameterDictionary["var_BreiteSpiegel"].AutoUpdateParameterValue(mirrorWidth);
        ParameterDictionary["var_HoeheSpiegel"].AutoUpdateParameterValue(mirrorHeight);
        ParameterDictionary["var_BreiteSpiegel2"].AutoUpdateParameterValue(mirrorWidth2);
        ParameterDictionary["var_HoeheSpiegel2"].AutoUpdateParameterValue(mirrorHeight2);
        ParameterDictionary["var_BreiteSpiegel3"].AutoUpdateParameterValue(mirrorWidth3);
        ParameterDictionary["var_HoeheSpiegel3"].AutoUpdateParameterValue(mirrorHeight3);
        ParameterDictionary["var_AbstandSpiegelvonLinks"].AutoUpdateParameterValue(mirrorDistanceLeft);
        ParameterDictionary["var_AbstandSpiegelvonLinks2"].AutoUpdateParameterValue(mirrorDistanceLeft2);
        ParameterDictionary["var_AbstandSpiegelvonLinks3"].AutoUpdateParameterValue(mirrorDistanceLeft3);
        ParameterDictionary["var_AbstandSpiegelDecke"].AutoUpdateParameterValue(mirrorDistanceCeiling);
        ParameterDictionary["var_AbstandSpiegelDecke2"].AutoUpdateParameterValue(mirrorDistanceCeiling2);
        ParameterDictionary["var_AbstandSpiegelDecke3"].AutoUpdateParameterValue(mirrorDistanceCeiling3);
    }

    private void ValidateDoorSill(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!(name.StartsWith("var_Tuertyp") || name.StartsWith("var_Tuerbezeichnung") || string.Equals(name, "var_EN8171Cat012")))
        {
            return;
        }
        var zugang = string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";

        var shaftSillParameterName = string.Equals(zugang, "A") ? "var_Schwellenprofil" : $"var_Schwellenprofil{zugang}";
        var carSillParameterName = string.Equals(zugang, "A") ? "var_SchwellenprofilKabTuere" : $"var_SchwellenprofilKabTuere{zugang}";

        IEnumerable<SelectionValue> availableDoorSills;

        if (name.StartsWith("var_Tuertyp"))
        {
            var doorDescription = ParameterDictionary[string.Equals(zugang, "A") ? "var_Tuerbezeichnung" : $"var_Tuerbezeichnung_{zugang}"].Value;
            availableDoorSills = GetAvailableDoorSills(value, doorDescription);
        }
        else if (string.Equals(name, "var_EN8171Cat012"))
        {
            var doorTyp = ParameterDictionary[string.Equals(zugang, "A") ? "var_Tuertyp" : $"var_Tuertyp_{zugang}"].Value;
            var doorDescription = ParameterDictionary[string.Equals(zugang, "A") ? "var_Tuerbezeichnung" : $"var_Tuerbezeichnung_{zugang}"].Value;
            availableDoorSills = GetAvailableDoorSills(doorTyp, doorDescription);
        }
        else
        {
            var doorTyp = ParameterDictionary[string.Equals(zugang, "A") ? "var_Tuertyp" : $"var_Tuertyp_{zugang}"].Value;
            availableDoorSills = GetAvailableDoorSills(doorTyp, value);
        }

        UpdateDropDownList(shaftSillParameterName, availableDoorSills);
        CheckListContainsValue(ParameterDictionary[shaftSillParameterName]);

        UpdateDropDownList(carSillParameterName, availableDoorSills);
        CheckListContainsValue(ParameterDictionary[carSillParameterName]);
    }

    private void ValidateCarDoorHeaders(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var zugang = string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";

        if (name.StartsWith("var_Tuerbezeichnung"))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownList.Clear();
                ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownList.Clear();
                ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].AutoUpdateParameterValue(string.Empty);
                ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].AutoUpdateParameterValue(string.Empty);
            }

            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor).FirstOrDefault(x => x.Name == value);
            var carDoorHeaderDepths = liftDoorGroup?.CarDoor?.CarDoorHeaderDepth.ToImmutableList();
            var carDoorHeaderHeights = liftDoorGroup?.CarDoor?.CarDoorHeaderHeight.ToImmutableList();

            if (carDoorHeaderDepths is not null)
            {
                List<SelectionValue> availablcarDoorHeaderDepths = [];
                for (int i = 0; i < carDoorHeaderDepths.Count; i++)
                {
                    availablcarDoorHeaderDepths.Add(new SelectionValue(i + 1, carDoorHeaderDepths[i].ToString(), carDoorHeaderDepths[i].ToString()) { IsFavorite = false, SchindlerCertified = false });
                }
                UpdateDropDownList($"var_KabTuerKaempferBreite{zugang}", availablcarDoorHeaderDepths, false);

                if (ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownList.Count == 1)
                {
                    ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].AutoUpdateParameterValue(ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownList[0].Name);
                }
                else
                {
                    CheckListContainsValue(ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"]);
                }
            }

            if (carDoorHeaderHeights is not null)
            {
                List<SelectionValue> availablcarDoorHeaderHeights = [];
                for (int i = 0; i < carDoorHeaderHeights.Count; i++)
                {
                    availablcarDoorHeaderHeights.Add(new SelectionValue(i + 1, carDoorHeaderHeights[i].ToString(), carDoorHeaderHeights[i].ToString()) { IsFavorite = false, SchindlerCertified = false });
                }
                UpdateDropDownList($"var_KabTuerKaempferHoehe{zugang}", availablcarDoorHeaderHeights, false);

                if (ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownList.Count == 1)
                {
                    ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].AutoUpdateParameterValue(ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownList[0].Name);
                }
                else
                {
                    CheckListContainsValue(ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"]);
                }
            }
        }
    }

    private void ValidateReducedCarDoorHeaderHeight(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value) && !string.Equals(name, "var_Tuerverriegelung"))
            return;

        var zugang = name.StartsWith("var_TB") ? string.Equals(name[^1..], "_B") || string.Equals(name[^1..], "_C") || string.Equals(name[^1..], "_D") ? name[^1..] : "A"
                                               : string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";

        var carDoorHeaderDepthParameter = ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"];
        var carDoorHeaderHeightParameter = ParameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"];

        if (carDoorHeaderHeightParameter is not null && carDoorHeaderDepthParameter is not null)
        {
            var errorMessage = $"{carDoorHeaderHeightParameter.DisplayName}: reduzierte Kämpferhöhe 350 mm nicht möglich.";

            if (carDoorHeaderHeightParameter.Value == "350")
            {
                if (carDoorHeaderDepthParameter.Value == "97")
                {
                    carDoorHeaderHeightParameter.AddError("Value", new ParameterStateInfo(carDoorHeaderHeightParameter.Name!, carDoorHeaderHeightParameter.DisplayName!, errorMessage, ErrorLevel.Error, false));
                    return;
                }
                if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Tuerverriegelung"].Value) && ParameterDictionary["var_Tuerverriegelung"].Value!.Contains("54"))
                {
                    carDoorHeaderHeightParameter.AddError("Value", new ParameterStateInfo(carDoorHeaderHeightParameter.Name!, carDoorHeaderHeightParameter.DisplayName!, errorMessage, ErrorLevel.Error, false));
                    return;
                }

                int doorWidth = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, string.Equals(zugang, "A") ? "var_TB" : $"var_TB_{zugang}");

                switch (ParameterDictionary[string.Equals(zugang, "A") ? "var_AnzahlTuerfluegel" : $"var_AnzahlTuerfluegel_{zugang}"].Value)
                {
                    case "2":
                        if (doorWidth < 600)
                        {
                            carDoorHeaderHeightParameter.AddError("Value", new ParameterStateInfo(carDoorHeaderHeightParameter.Name!, carDoorHeaderHeightParameter.DisplayName!, errorMessage, ErrorLevel.Error, false));
                            return;
                        }
                        break;
                    case "3":
                        if (doorWidth < 1000)
                        {
                            carDoorHeaderHeightParameter.AddError("Value", new ParameterStateInfo(carDoorHeaderHeightParameter.Name!, carDoorHeaderHeightParameter.DisplayName!, errorMessage, ErrorLevel.Error, false));
                            return;
                        }
                        break;
                    case "4":
                        if (doorWidth < 1200)
                        {
                            carDoorHeaderHeightParameter.AddError("Value", new ParameterStateInfo(carDoorHeaderHeightParameter.Name!, carDoorHeaderHeightParameter.DisplayName!, errorMessage, ErrorLevel.Error, false));
                            return;
                        }
                        break;
                    case "6":
                        if (doorWidth < 2100)
                        {
                            carDoorHeaderHeightParameter.AddError("Value", new ParameterStateInfo(carDoorHeaderHeightParameter.Name!, carDoorHeaderHeightParameter.DisplayName!, errorMessage, ErrorLevel.Error, false));
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }
            carDoorHeaderHeightParameter.RemoveError("Value", errorMessage);
        }
    }

    private void ValidateCarCeilingDetails(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.IsDefaultCarTyp(ParameterDictionary["var_Fahrkorbtyp"].Value))
            return;

        var ruleActivationDate = new DateTime(2024, 01, 11);
        var creationDate = DateTime.MinValue;

        if (DateTime.TryParse(LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_ErstelltAm"), out DateTime parsedDate))
            creationDate = parsedDate;

        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_KD"].Value) && ruleActivationDate.CompareTo(creationDate) > 0)
            return;

        switch (name)
        {
            case "var_KBI" or "var_overrideDefaultCeiling":
                ParameterDictionary["var_GrundDeckenhoehe"].AutoUpdateParameterValue(GetDefaultCeiling().ToString());
                break;
            case "var_abgDecke" or "var_overrideSuspendedCeiling":
                var suspendedCeilingHeight = GetSuspendedCeiling();
                if (suspendedCeilingHeight == 0)
                {
                    ParameterDictionary["var_abgeDeckeHoehe"].AutoUpdateParameterValue(string.Empty);
                    ParameterDictionary["var_overrideSuspendedCeiling"].AutoUpdateParameterValue(string.Empty);
                }
                else
                {
                    ParameterDictionary["var_abgeDeckeHoehe"].AutoUpdateParameterValue(suspendedCeilingHeight.ToString());
                }
                break;
            default:
                break;
        }

        double cRail = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_DeckenCSchienenHoehe");
        double defaultCeiling = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_GrundDeckenhoehe");
        double suspendedCeiling = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_abgeDeckeHoehe");

        ParameterDictionary["var_KD"].AutoUpdateParameterValue(Convert.ToString(cRail + defaultCeiling + suspendedCeiling));
    }

    private void ValidateCarHeight(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        double bodenHoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KU");
        double kabinenHoeheInnen = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KHLicht");
        double kabinenHoeheAussen = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KHA");
        double deckenhoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KD");

        if (bodenHoehe + kabinenHoeheInnen + deckenhoehe == kabinenHoeheAussen)
            return;
        ParameterDictionary["var_KHA"].AutoUpdateParameterValue(Convert.ToString(bodenHoehe + kabinenHoeheInnen + deckenhoehe));
    }

    private void ValidateCarHeightExcludingSuspendedCeiling(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value) && string.Equals(name, "var_KHLicht"))
        {
            ParameterDictionary["var_KHRoh"].AutoUpdateParameterValue(string.Empty);
            return;
        }

        var suspendedCeilingHeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_abgeDeckeHoehe");
        var carHeight = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KHLicht");
        ParameterDictionary["var_KHRoh"].AutoUpdateParameterValue(Convert.ToString(carHeight + suspendedCeilingHeight, CultureInfo.CurrentCulture));
    }

    private void ValidateGlassPanelColor(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        switch (name)
        {
            case "var_Paneelmaterial":
                if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("ESG"))
                {
                    ParameterDictionary["var_PaneelmaterialGlas"].AutoUpdateParameterValue(string.Empty);
                    ParameterDictionary["var_PaneelGlasRAL"].AutoUpdateParameterValue(string.Empty);
                }
                break;
            case "var_PaneelmaterialGlas":
                if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("Euro"))
                {
                    ParameterDictionary["var_PaneelGlasRAL"].AutoUpdateParameterValue(string.Empty);
                }
                break;
            default:
                break;
        }
    }

    private void ValidateCarFramePosition(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        Dictionary<string, bool> entrances = new () 
        {
            {"A", false },
            {"B", false },
            {"C", false },
            {"D", false },
        };
        var availableCarFramePositions = _parametercontext.Set<CarFramePosition>().Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified }).ToList();
        foreach (var entrance in entrances)
        {
            entrances[entrance.Key] = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_ZUGANSSTELLEN_{entrance.Key}");
            if (entrances[entrance.Key])
            {
                var selectedEntrance = availableCarFramePositions.FirstOrDefault(x => x.Name == entrance.Key);
                if (selectedEntrance != null)
                {
                    availableCarFramePositions.Remove(selectedEntrance);
                }
            }
        }
        ParameterDictionary["var_Durchladung"].AutoUpdateParameterValue(entrances["A"] && entrances["C"] || entrances["B"] && entrances["D"] ? "True" : "False");

        if (!entrances["C"])
        {
            var carFrame = _calculationsModuleService.GetCarFrameTyp(ParameterDictionary);
            if (carFrame != null)
            {
                if (carFrame.CarFrameBaseTypeId == 3 || carFrame.CarFrameBaseTypeId == 5 || carFrame.CarFrameBaseTypeId == 6)
                {
                    var selectedEntrance = availableCarFramePositions.FirstOrDefault(x => x.Name == "C");
                    if (selectedEntrance != null)
                    {
                        availableCarFramePositions.Remove(selectedEntrance);
                    }
                }
            }
        }
        
        UpdateDropDownList("var_Bausatzlage", availableCarFramePositions);

        var carFramePosition = ParameterDictionary["var_Bausatzlage"].Value;
        if (!string.IsNullOrWhiteSpace(carFramePosition))
        {
            switch (carFramePosition)
            {
                case "A":
                    ParameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("False");
                    ParameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("False");
                    ParameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("False");
                    break;
                case "B":
                    ParameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("False");
                    ParameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("True");
                    ParameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("False");
                    break;
                case "C":
                    ParameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("False");
                    ParameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("False");
                    ParameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("True");
                    break;
                case "D":
                    ParameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("True");
                    ParameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("False");
                    ParameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("False");
                    break;
                default:
                    ParameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("False");
                    ParameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("False");
                    ParameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("False");
                    break;
            }
        }
    }

    private void ValidateLayOutDrawingLoads(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        // Rule only invoked by var_CFPdefiniert
        if (value == "False")
        {
            return;
        }

        string[] loadNames = ["var_Belastung_pro_Schiene_auf_Grundelement",
                              "var_Belastung_pro_Schiene_auf_Grundelement_GGW",
                              "var_Belastung_Pufferstuetze_auf_Grundelement",
                              "var_Belastung_Pufferstuetze_auf_Grundelement_GGW",
                              "var_FxF",
                              "var_FyF",
                              "var_FxFA_GGW",
                              "var_FyFA_GGW"];

        foreach (var loadName in loadNames)
        {
            double load = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, loadName);
            ParameterDictionary[$"{loadName}_AZ"].Value = LiftParameterHelper.GetLayoutDrawingLoad(load).ToString();
        }
    }

    private void ValidateCarFrameProgramData(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(FullPathXml) || FullPathXml == pathDefaultAutoDeskTransfer)
            return;

        var cFPPath = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".dat");
        if (!File.Exists(cFPPath))
            return;
        var lastWriteTime = File.GetLastWriteTime(cFPPath);
        if (lastWriteTime != CFPCreationTime)
        {
            string cFPDataFile = string.Empty;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding win1252 = Encoding.GetEncoding(1252);
            using (var sr = new StreamReader(cFPPath, win1252, true))
            {
                cFPDataFile = sr.ReadToEnd();
            }

            var cFPDataFileLines = cFPDataFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            if (cFPDataFileLines.Length > 0)
            {
                CFPDataDictionary.Clear();
                foreach (var line in cFPDataFileLines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        break;
                    }

                    var keyCFPParameter = line[1..line.IndexOf(']')];
                    string valueCFPParameter;
                    if (line.Contains('¦'))
                    {
                        valueCFPParameter = line[(line.IndexOf(']') + 2)..line.IndexOf('¦')];
                    }
                    else
                    {
                        valueCFPParameter = line[(line.IndexOf(']') + 2)..];
                    }
                    CFPDataDictionary.TryAdd(keyCFPParameter, valueCFPParameter);
                }
            }
            CFPCreationTime = lastWriteTime;
        }

        var searchString = name switch
        {
            "var_Q" => "Nutzmasse",
            "var_F" => "Kabinenmasse",
            "var_FH" => "Foerderhoehe",
            "var_SG" => "Schachtgrube",
            "var_SK" => "Schachtkopf",
            "var_SB" => "Schachtbreite_innen",
            "var_ST" => "Schachttiefe_innen",
            "var_KBI" => "Kabinenbreite_innen",
            "var_KTI" => "Kabinentiefe_innen",
            "var_KHLicht" => "Kabinenhoehe_innen",
            "var_KHA" => "Kabinenhoehe_aussen",
            "var_v" => "Sollgeschwindigkeit abw",
            "var_TypFV" => "Fangvorrichtung",
            "var_FuehrungsschieneFahrkorb" => "Schienentyp",
            "var_FuehrungsschieneGegengewicht" => "Hilfsschienentyp",
            "var_Geschwindigkeitsbegrenzer" => "GB_ID",
            "var_TypFuehrung" => "Fuehrungsart",
            _ => string.Empty,
        };

        if (CFPDataDictionary.TryGetValue(searchString, out string? cFPValue))
        {
            if (cFPValue is not null)
            {
                var isValid = name switch
                {
                    "var_Q" => string.Equals(value, cFPValue, StringComparison.CurrentCultureIgnoreCase),
                    "var_F" => Math.Abs(Convert.ToInt32(value) - Convert.ToInt32(cFPValue)) <= 10,
                    "var_FH" => Math.Abs(Convert.ToDouble(value) * 1000 - Convert.ToDouble(cFPValue) * 1000) <= 20,
                    "var_SG" => Convert.ToDouble(value) == Math.Round(Convert.ToDouble(cFPValue) * 1000),
                    "var_SK" => Convert.ToDouble(value) == Math.Round(Convert.ToDouble(cFPValue) * 1000),
                    "var_SB" => Convert.ToDouble(value) == Math.Round(Convert.ToDouble(cFPValue) * 1000),
                    "var_ST" => Convert.ToDouble(value) == Math.Round(Convert.ToDouble(cFPValue) * 1000),
                    "var_KBI" => Convert.ToDouble(value) == Math.Round(Convert.ToDouble(cFPValue) * 1000),
                    "var_KTI" => Convert.ToDouble(value) == Math.Round(Convert.ToDouble(cFPValue) * 1000),
                    "var_KHLicht" => Convert.ToDouble(value) == Math.Round(Convert.ToDouble(cFPValue) * 1000),
                    "var_KHA" => Convert.ToDouble(value) == Math.Round(Convert.ToDouble(cFPValue) * 1000),
                    "var_v" => Convert.ToDouble(value) == Convert.ToDouble(cFPValue),
                    "var_TypFV" => value switch
                    {
                        "Bucher RSG55" => true,
                        "Bucher RSG70" => true,
                        "Bucher RSG90" => true,
                        _ => string.Equals(value, cFPValue, StringComparison.CurrentCultureIgnoreCase),
                    },
                    "var_FuehrungsschieneFahrkorb" => string.Equals(value, cFPValue, StringComparison.CurrentCultureIgnoreCase),
                    "var_FuehrungsschieneGegengewicht" => string.Equals(value, cFPValue, StringComparison.CurrentCultureIgnoreCase),
                    "var_Geschwindigkeitsbegrenzer" => cFPValue switch
                    {
                        "1" => string.Equals(value, "kein GB", StringComparison.CurrentCultureIgnoreCase) || string.Equals(value, "Schlaffseilauslösung", StringComparison.CurrentCultureIgnoreCase),
                        "2" => string.Equals(value, "GB durch Kunde", StringComparison.CurrentCultureIgnoreCase),
                        "3" => string.Equals(value, "Jungblut HJ 200", StringComparison.CurrentCultureIgnoreCase),
                        "4" => string.Equals(value, "Bode Typ 5 mit FA 12V", StringComparison.CurrentCultureIgnoreCase),
                        "5" => string.Equals(value, "Jungblut HJ200 mit FA 24V", StringComparison.CurrentCultureIgnoreCase),
                        "6" => string.Equals(value, "Jungblut HJ200 mit FA 230V", StringComparison.CurrentCultureIgnoreCase),
                        "7" => string.Equals(value, "Jungblut HJ200 mit AS 12V", StringComparison.CurrentCultureIgnoreCase),
                        "8" => string.Equals(value, "Jungblut HJ200 mit AS 24V", StringComparison.CurrentCultureIgnoreCase),
                        "9" => string.Equals(value, "Jungblut HJ200 mit ASV 12V und Elektronikpaket Ausführung A1", StringComparison.CurrentCultureIgnoreCase),
                        "10" => string.Equals(value, "Jungblut HJ200 mit ASV 12V und Elektronikpaket Ausführung A2", StringComparison.CurrentCultureIgnoreCase),
                        "11" => string.Equals(value, "Jungblut HJ200 mit ASV 24V und Elektronikpaket Ausführung A1", StringComparison.CurrentCultureIgnoreCase),
                        "12" => string.Equals(value, "Jungblut HJ200 mit ASV 24V und Elektronikpaket Ausführung A2", StringComparison.CurrentCultureIgnoreCase),
                        "13" => string.Equals(value, "PFB LK 200, FA, el. Vorab. 230V (elektrom. Rückst.)", StringComparison.CurrentCultureIgnoreCase),
                        "14" => string.Equals(value, "kein GB", StringComparison.CurrentCultureIgnoreCase),
                        "15" => string.Equals(value, "HJ200, FA u. el. Vorab. 230V (elektrom. Rückst.)", StringComparison.CurrentCultureIgnoreCase),
                        "16" => string.Equals(value, "HJ200, AS 24V, el. Vorab. 230V (elektrom. Rückst.)", StringComparison.CurrentCultureIgnoreCase),
                        _ => true
                    },
                    "var_TypFuehrung" => cFPValue switch
                    {
                        "1" => string.Equals(value, "HSM 140", StringComparison.CurrentCultureIgnoreCase),
                        "2" => string.Equals(value, "HSML 180", StringComparison.CurrentCultureIgnoreCase),
                        "3" => string.Equals(value, "HSMEL 300", StringComparison.CurrentCultureIgnoreCase),
                        "4" => string.Equals(value, "Gleitfuehrung", StringComparison.CurrentCultureIgnoreCase),
                        "5" => string.Equals(value, "Rollenfuehrung BR", StringComparison.CurrentCultureIgnoreCase),
                        "6" => string.Equals(value, "RF FK 3", StringComparison.CurrentCultureIgnoreCase),
                        "7" => string.Equals(value, "Gleitfuehrung 903940", StringComparison.CurrentCultureIgnoreCase),
                        "8" => string.Equals(value, "Gleitfuehrung 903809", StringComparison.CurrentCultureIgnoreCase),
                        "9" => string.Equals(value, "Rollenfuehrung 903800", StringComparison.CurrentCultureIgnoreCase),
                        "10" => string.Equals(value, "Rollenfuehrung 903935", StringComparison.CurrentCultureIgnoreCase),
                        "11" => string.Equals(value, "HSM 140 gedämpft", StringComparison.CurrentCultureIgnoreCase),
                        "12" => string.Equals(value, "RF FK 1", StringComparison.CurrentCultureIgnoreCase),
                        _ => true
                    },
                    _ => true
                };

                if (!isValid)
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Unterschiedliche Werte für >{displayname}<  Wert Spezifikation: {value} | Wert CarFrameProgram: {cFPValue}", SetSeverity(severity)));
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
                }
            }
        }
    }

    private void ValidateCarDoorMountingDimensions(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.Equals(ParameterDictionary["var_Fahrkorbtyp"].Value, "Fremdkabine", StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }
        var zugang = string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";
        string doorTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, zugang == "A" ? "var_Tuerbezeichnung" : $"var_Tuerbezeichnung_{zugang}");

        if (string.IsNullOrWhiteSpace(doorTyp))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
        }
        else
        {
            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor).FirstOrDefault(x => x.Name == doorTyp);
            if (liftDoorGroup == null || liftDoorGroup.CarDoor == null)
            {
                return;
            }
            double minMountingSpace = liftDoorGroup.CarDoor.MinimalMountingSpace;
            double minMountingSpaceReduced = 0d;
            if (ParameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].Value == "97")
            {
                minMountingSpaceReduced = liftDoorGroup.CarDoor.ReducedMinimalMountingSpace;
            }
            string doorMounting = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, zugang == "A" ? "var_TuerEinbau" : $"var_TuerEinbau{zugang}");

            if (double.TryParse(doorMounting, out double selectedMountingSpace))
            {
                if (selectedMountingSpace <= 0)
                {
                    return;
                }

                string[] dependentParameter = name switch
                {
                    var n when n.StartsWith("var_Tuerbezeichnung") => [zugang == "A" ? "var_TuerEinbau" : $"var_TuerEinbau{zugang}", $"var_KabTuerKaempferBreite{zugang}"],
                    var n when n.StartsWith("var_TuerEinbau") => [zugang == "A" ? "var_Tuerbezeichnung" : $"var_Tuerbezeichnung_{zugang}", $"var_KabTuerKaempferBreite{zugang}"],
                    var n when n.StartsWith("var_KabTuerKaempferBreite") => [zugang == "A" ? "var_TuerEinbau" : $"var_TuerEinbau{zugang}", zugang == "A" ? "var_Tuerbezeichnung" : $"var_Tuerbezeichnung_{zugang}"],
                    _ => []
                };

                if (minMountingSpace != selectedMountingSpace && minMountingSpaceReduced != selectedMountingSpace)
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Türanbau: {selectedMountingSpace} mm enspricht nicht unserem standard Türanbau: {minMountingSpace} mm bzw. reduziertem Türanbau: {minMountingSpaceReduced} mm.", SetSeverity(severity)) { DependentParameter = dependentParameter });
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = dependentParameter });
                }
            }
        }
    }

    private void ValidateSchindlerCertifiedComponents(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (!string.Equals(name, "var_Schachtinformationssystem"))
        {
            return;
        }
        var msz9eSchindlerCertified = ParameterDictionary[name].DropDownListValue?.Id == 2;
        var msz9e = ParameterDictionary["var_Steuerungstyp"].DropDownList.FirstOrDefault(x => x.Id == 6);
        if (msz9e is not null)
        {
            msz9e.SchindlerCertified = msz9eSchindlerCertified;
        }
    }
}