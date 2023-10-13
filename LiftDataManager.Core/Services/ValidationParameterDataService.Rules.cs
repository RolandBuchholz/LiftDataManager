using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using HtmlAgilityPack;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using LiftDataManager.Core.Messenger.Messages;
using System.Globalization;

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
        var anotherParameter = Convert.ToBoolean(ParamterDictionary[anotherBoolean].Value, CultureInfo.CurrentCulture);
        if (string.IsNullOrWhiteSpace(value) && anotherParameter)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} darf nicht leer sein wenn {anotherBoolean} gesetzt (wahr) ist", SetSeverity(severity))
            { DependentParameter = new string[] { anotherBoolean } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { anotherBoolean } });
        }
    }

    private void NotEmptyOr0WhenAnotherTrue(string name, string displayname, string? value, string? severity, string? anotherBoolean)
    {
        if (string.IsNullOrWhiteSpace(anotherBoolean))
            return;
        var anotherParameter = Convert.ToBoolean(ParamterDictionary[anotherBoolean].Value, CultureInfo.CurrentCulture);
        if ((string.IsNullOrWhiteSpace(value) || value == "0") && anotherParameter)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} darf nicht leer sein wenn {anotherBoolean} gesetzt (wahr) ist", SetSeverity(severity))
            { DependentParameter = new string[] { anotherBoolean } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { anotherBoolean } });
        }
    }

    private void MustBeTrueWhenAnotherNotEmty(string name, string displayname, string? value, string? severity, string? anotherString)
    {
        if (string.IsNullOrWhiteSpace(anotherString))
            return;
        var valueToBool = Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        var stringValue = ParamterDictionary[anotherString].Value;

        if (valueToBool && (string.IsNullOrWhiteSpace(stringValue)))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{name} gesetzt (wahr) ist, darf {anotherString} nicht leer sein", SetSeverity(severity))
            { DependentParameter = new string[] { anotherString } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { anotherString } });
        }
    }

    private void ListContainsValue(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        if (!ParamterDictionary[name].dropDownList.Contains(value))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{displayname}: ungültiger Wert | {value} | ist nicht in der Auswahlliste vorhanden.", SetSeverity(severity)));
        }
    }

    // Spezial validationrules

    private void ValidateJobNumber(string name, string displayname, string? value, string? severity, string? odernummerName)
    {
        if (string.IsNullOrWhiteSpace(odernummerName))
            return;
        var fabriknummer = ParamterDictionary["var_FabrikNummer"].Value;

        if (string.IsNullOrWhiteSpace(fabriknummer))
            return;
        ParamterDictionary["var_FabrikNummer"].ClearErrors("var_FabrikNummer");

        var auftragsnummer = ParamterDictionary[odernummerName].Value;
        var informationAufzug = ParamterDictionary["var_InformationAufzug"].Value;
        var fabriknummerBestand = ParamterDictionary["var_FabriknummerBestand"].Value;

        switch (informationAufzug)
        {
            case "Neuanlage" or "Ersatzanlage":
                if (auftragsnummer != fabriknummer)
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Bei Neuanlagen und Ersatzanlagen muß die Auftragsnummer und Fabriknummer identisch sein", SetSeverity(severity))
                    { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                return;
            case "Umbau":
                if (string.IsNullOrWhiteSpace(fabriknummerBestand) && auftragsnummer != fabriknummer)
                    return;
                if (fabriknummerBestand != fabriknummer)
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Bei Umbauten muß die Fabriknummer der alten Anlage beibehalten werden", SetSeverity(severity))
                    { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                else
                {
                    ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
                }
                return;
            default:
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_FabrikNummer", "var_InformationAufzug", "var_FabriknummerBestand" } });
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

    private void ValidateRammingProtections(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (value is null)
            return;
        if (value == "Rammschutz siehe Beschreibung")
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Bei Rammschutz |{value}| muss das Gewicht über das Kabinenkorrekturgewicht mitgegeben werden!", SetSeverity(severity)));
        }
    }

    private void ValidateTravel(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "0"))
            return;
        var foerderhoehe = Math.Round(LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_FH") * 1000);
        var etagenhoehe0 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe0");
        var etagenhoehe1 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe1");
        var etagenhoehe2 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe2");
        var etagenhoehe3 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe3");
        var etagenhoehe4 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe4");
        var etagenhoehe5 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe5");
        var etagenhoehe6 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe6");
        var etagenhoehe7 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe7");
        var etagenhoehe8 = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Etagenhoehe8");

        if ((etagenhoehe0 + etagenhoehe1 + etagenhoehe2 + etagenhoehe3 + etagenhoehe4 + etagenhoehe5 + etagenhoehe6 + etagenhoehe7 + etagenhoehe8) == 0)
            return;

        var etagenhoeheTotal = etagenhoehe0 + etagenhoehe1 + etagenhoehe2 + etagenhoehe3 + etagenhoehe4 + etagenhoehe5 + etagenhoehe6 + etagenhoehe7 + etagenhoehe8;

        if (etagenhoeheTotal != foerderhoehe)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Die Förderhöhe ({foerderhoehe} mm) stimmt nicht mit Etagenabständen ({etagenhoeheTotal} mm) überein.", SetSeverity(severity))
            { DependentParameter = new string[] { "var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2", "var_Etagenhoehe3", "var_Etagenhoehe4", "var_Etagenhoehe5", "var_Etagenhoehe6", "var_Etagenhoehe7", "var_Etagenhoehe8" } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_FH", "var_Etagenhoehe0", "var_Etagenhoehe1", "var_Etagenhoehe2", "var_Etagenhoehe3", "var_Etagenhoehe4", "var_Etagenhoehe5", "var_Etagenhoehe6", "var_Etagenhoehe7", "var_Etagenhoehe8" } });
        }
    }

    private void ValidateCarFlooring(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        string bodentyp = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodentyp");
        string bodenProfil = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_BoPr");
        string bodenBelag = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelag");
        double bodenBlech = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenblech");
        double bodenBelagHoehe = GetFlooringHeight(bodenBelag);
        double bodenHoehe = -1;

        switch (bodentyp)
        {
            case "standard":
                bodenHoehe = 83;
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "3";
                ParamterDictionary["var_BoPr"].DropDownListValue = "80 x 40 x 3";
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
                ParamterDictionary["var_Bodenblech"].DropDownListValue = "5";
                ParamterDictionary["var_BoPr"].DropDownListValue = "80 x 40 x 3";
                bodenHoehe = 85;
                break;
            case "sonder":
                if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Bodenblech"].Value))
                    ParamterDictionary["var_Bodenblech"].DropDownListValue = "(keine Auswahl)";
                if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_BoPr"].Value))
                    ParamterDictionary["var_BoPr"].DropDownListValue = "(keine Auswahl)";
                bodenHoehe = -1;
                break;
            case "extern":
                if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Bodenblech"].Value))
                    ParamterDictionary["var_Bodenblech"].DropDownListValue = "(keine Auswahl)";
                if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_BoPr"].Value))
                    ParamterDictionary["var_BoPr"].DropDownListValue = "(keine Auswahl)";
                bodenHoehe = -1;
                break;
            default:
                if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_Bodenblech"].Value))
                    ParamterDictionary["var_Bodenblech"].DropDownListValue = "(keine Auswahl)";
                if (!string.IsNullOrWhiteSpace(ParamterDictionary["var_BoPr"].Value))
                    ParamterDictionary["var_BoPr"].DropDownListValue = "(keine Auswahl)";
                break;
        }

        ParamterDictionary["var_Bodenbelagsgewicht"].Value = GetFloorWeight(bodenBelag);
        if (bodenHoehe != -1)
        {
            ParamterDictionary["var_KU"].Value = Convert.ToString(bodenHoehe + bodenBelagHoehe);
        }
        ParamterDictionary["var_Bodenbelagsdicke"].Value = Convert.ToString(bodenBelagHoehe);

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
                return LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelagsgewicht");
            if (string.Equals(bodenBelag, "Nach Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bodenbelagsgewicht");
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
                return LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenbelagsdicke");
            if (string.Equals(bodenBelag, "Nach Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Bodenbelagsdicke");
            var boden = _parametercontext.Set<CarFlooring>().FirstOrDefault(x => x.Name == bodenBelag);
            if (boden is null)
                return 0;
            return (double)boden.Thickness!;
        }

        void SetDefaultReinforcedFloor(string name)
        {
            ParamterDictionary["var_Bodenblech"].DropDownListValue = "3";
            ParamterDictionary["var_BoPr"].DropDownListValue = "80 x 50 x 5";
            if (name == "var_BoPr")
            {
                ParamterDictionary["var_BoPr"].DropDownList.Add("Refresh");
                ParamterDictionary["var_BoPr"].DropDownList.Remove("Refresh");
            }
        }
    }

    private void ValidateCarHeight(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        double bodenHoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KU");
        double kabinenHoeheInnen = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KHLicht");
        double kabinenHoeheAussen = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KHA");
        double deckenhoehe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KD");

        if (bodenHoehe + kabinenHoeheInnen + deckenhoehe == kabinenHoeheAussen)
            return;

        switch (name)
        {
            case "var_KU" or "var_KHLicht" or "var_KD":
                ParamterDictionary["var_KHA"].Value = Convert.ToString(bodenHoehe + kabinenHoeheInnen + deckenhoehe);
                return;
            case "var_KHA":
                ParamterDictionary["var_KD"].Value = Convert.ToString(kabinenHoeheAussen - (bodenHoehe + kabinenHoeheInnen));
                return;
            default:
                return;
        }
    }

    private void ValidateCarEntranceRightSide(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        double kabinenBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KBI");
        double kabinenTiefe = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_KTI");
        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_D");
        double linkeSeite;
        double tuerBreite;

        switch (name)
        {
            case "var_L1" or "var_TB" or "var_KBI":
                if (!(kabinenBreite > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L1");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB");
                ParamterDictionary["var_R1"].Value = Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite));
                return;
            case "var_L2" or "var_TB_C" or "var_KBI":
                if (!(kabinenBreite > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L2");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_C");
                double r2 = zugangC ? kabinenBreite - (linkeSeite + tuerBreite) : 0;
                ParamterDictionary["var_R2"].Value = Convert.ToString(r2);
                return;
            case "var_L3" or "var_TB_B" or "var_KTI":
                if (!(kabinenTiefe > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L3");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_B");
                double r3 = zugangB ? kabinenTiefe - (linkeSeite + tuerBreite) : 0;
                ParamterDictionary["var_R3"].Value = Convert.ToString(r3);
                return;
            case "var_L4" or "var_TB_D" or "var_KTI":
                if (!(kabinenTiefe > 0))
                    return;
                linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_L4");
                tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_TB_D");
                double r4 = zugangD ? kabinenTiefe - (linkeSeite + tuerBreite) : 0;
                ParamterDictionary["var_R4"].Value = Convert.ToString(r4);
                return;
            default:
                return;
        }
    }

    private void ValidateVariableCarDoors(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        bool variableTuerdaten = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_Variable_Tuerdaten");
        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_ZUGANSSTELLEN_D");
        
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

        var availableCarframes = carframes.Where(x => x.DriveTypeId == driveTypeId).Select(s => s.Name);

        if (availableCarframes is not null)
        {
            ParamterDictionary["var_Bausatz"].DropDownList.Clear();
            foreach (var item in availableCarframes)
            {
                ParamterDictionary["var_Bausatz"].DropDownList.Add(item!);
            }
        }
    }

    private void ValidateReducedProtectionSpaces(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (name == "var_Ersatzmassnahmen")
        {
            if (string.IsNullOrWhiteSpace(value) || value == "keine")
            {
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "False";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "False";
            }
            else if (value == "Vorausgelöstes Anhaltesystem" || value.StartsWith("Schachtkopf und Schachtgrube"))
            {
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "True";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "True";
            }
            else if (value.StartsWith("Schachtkopf"))
            {
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "True";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "False";
            }
            else if (value.StartsWith("Schachtgrube"))
            {
                ParamterDictionary["var_ErsatzmassnahmenSK"].Value = "False";
                ParamterDictionary["var_ErsatzmassnahmenSG"].Value = "True";
            }
        }

        var selectedSafetyGear = ParamterDictionary["var_TypFV"].Value;
        var selectedReducedProtectionSpace = ParamterDictionary["var_Ersatzmassnahmen"].Value;

        if (name == "var_TypFV")
        {
            var reducedProtectionSpaces = _parametercontext.Set<ReducedProtectionSpace>().ToList();
            IEnumerable<string?> availablEReducedProtectionSpaces;

            if (string.IsNullOrWhiteSpace(selectedSafetyGear))
            {
                availablEReducedProtectionSpaces = reducedProtectionSpaces.Select(s => s.Name);
            }
            else if (selectedSafetyGear.Contains("BS"))
            {
                availablEReducedProtectionSpaces = reducedProtectionSpaces.Where(x => x.Name.Contains(selectedSafetyGear) || x.Name == "keine").Select(s => s.Name);
            }
            else if (selectedSafetyGear.Contains("PC 13") || selectedSafetyGear.Contains("PC 24") || selectedSafetyGear.Contains("CSGB01") || selectedSafetyGear.Contains("CSGB02"))
            {
                availablEReducedProtectionSpaces = reducedProtectionSpaces.Where(x => !x.Name.Contains("ESG")).Select(s => s.Name);
            }
            else
            {
                availablEReducedProtectionSpaces = reducedProtectionSpaces.Where(x => !x.Name.Contains("ESG") && !x.Name.Contains("Voraus")).Select(s => s.Name);
            }

            if (availablEReducedProtectionSpaces is not null)
            {
                ParamterDictionary["var_Ersatzmassnahmen"].DropDownList.Clear();
                foreach (var item in availablEReducedProtectionSpaces)
                {
                    ParamterDictionary["var_Ersatzmassnahmen"].DropDownList.Add(item!);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(selectedSafetyGear) &&
            !string.IsNullOrWhiteSpace(selectedReducedProtectionSpace))
        {
            if (!ParamterDictionary["var_Ersatzmassnahmen"].DropDownList.Contains(selectedReducedProtectionSpace))
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Ersatzmassnahmen sind mit der Fangvorrichtung {selectedSafetyGear} nicht zulässig!", SetSeverity(severity))
                { DependentParameter = new string[] { optional! } });
            }
            else
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { optional! } });
            }
        }
    }

    private void ValidateSafetyGear(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var safetyGears = _parametercontext.Set<SafetyGearModelType>().ToList();
        var selectedSafetyGear = ParamterDictionary["var_TypFV"].Value;
        IEnumerable<string?> availablseafetyGears = value switch
        {
            "keine" => Enumerable.Empty<string?>(),
            "Sperrfangvorrichtung" => safetyGears.Where(x => x.SafetyGearTypeId == 1).Select(s => s.Name),
            "Bremsfangvorrichtung" => safetyGears.Where(x => x.SafetyGearTypeId == 2).Select(s => s.Name),
            "Rohrbruchventil" => safetyGears.Where(x => x.SafetyGearTypeId == 3).Select(s => s.Name),
            _ => safetyGears.Select(s => s.Name),
        };
        if (availablseafetyGears is not null)
        {
            ParamterDictionary["var_TypFV"].DropDownList.Clear();
            foreach (var item in availablseafetyGears)
            {
                ParamterDictionary["var_TypFV"].DropDownList.Add(item!);
            }

            if (!string.IsNullOrWhiteSpace(selectedSafetyGear) && !availablseafetyGears.Contains(selectedSafetyGear))
            {
                ParamterDictionary["var_TypFV"].Value = string.Empty;
                ParamterDictionary["var_TypFV"].DropDownListValue = null;
            }
        }
    }

    private void ValidateSafetyRange(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(ParamterDictionary["var_Q"].Value) ||
            string.IsNullOrWhiteSpace(ParamterDictionary["var_F"].Value) ||
            ParamterDictionary["var_F"].Value == "0" ||
            string.IsNullOrWhiteSpace(ParamterDictionary["var_Fuehrungsart"].Value) ||
            string.IsNullOrWhiteSpace(ParamterDictionary["var_FuehrungsschieneFahrkorb"].Value) ||
            string.IsNullOrWhiteSpace(ParamterDictionary["var_TypFV"].Value))
            return;

        var safetygearResult = _calculationsModuleService.GetSafetyGearCalculation(ParamterDictionary);
        if (safetygearResult is not null)
        {
            if (!safetygearResult.RailHeadAllowed)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählter Schienenkopf ist für diese Fangvorrichtung nicht zulässig.", SetSeverity(severity)));
                return;
            }

            var load = LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_Q");
            var carWeight = LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_F");

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
        ParamterDictionary["var_Getriebe"].Value = currentdriveSystem is not null ? currentdriveSystem.DriveSystemType!.Name : string.Empty;
        ParamterDictionary["var_Getriebe"].DropDownListValue = ParamterDictionary["var_Getriebe"].Value;
    }

    private void ValidateCarweightWithoutFrame(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        int carFrameWeight = LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_Rahmengewicht");
        string? fangrahmenTyp = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Bausatz");

        if (carFrameWeight == 0 && !string.IsNullOrWhiteSpace(fangrahmenTyp))
        {
            var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == fangrahmenTyp);
            if (carFrameType is not null)
            {
                carFrameWeight = carFrameType.CarFrameWeight;
            }
        }

        if (carFrameWeight > 0)
        {
            int carWeight = LiftParameterHelper.GetLiftParameterValue<int>(ParamterDictionary, "var_F");
            if (carWeight > 0)
            {
                ParamterDictionary["var_KabTueF"].Value = Convert.ToString(carWeight - carFrameWeight);
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
            if (string.Equals(LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Normen"), "MRL 2006/42/EG"))
                return;
            var load = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q");
            var reducedLoad = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q1");
            var area = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_A_Kabine");
            var lift = LiftParameterHelper.GetLiftParameterValue<string>(ParamterDictionary, "var_Aufzugstyp");
            var cargoTypDB = _parametercontext.Set<LiftType>().Include(i => i.CargoType)
                                                            .ToList()
                                                            .FirstOrDefault(x => x.Name == lift);
            var cargotyp = cargoTypDB is not null ? cargoTypDB.CargoType!.Name! : "Aufzugstyp noch nicht gewählt !";
            var driveSystemDB = _parametercontext.Set<LiftType>().Include(i => i.DriveType)
                                                               .ToList()
                                                               .FirstOrDefault(x => x.Name == lift);
            var drivesystem = driveSystemDB is not null ? driveSystemDB.DriveType!.Name! : "";

            if (string.Equals(cargotyp, "Lastenaufzug") && string.Equals(drivesystem, "Hydraulik"))
            {
                var loadTable6 = _calculationsModuleService.GetLoadFromTable(area, "Tabelle6");

                if (reducedLoad < loadTable6)
                    ParamterDictionary["var_Q1"].Value = Convert.ToString(loadTable6);
            }
            else
            {
                ParamterDictionary["var_Q1"].Value = Convert.ToString(load);
            }

            if (!_calculationsModuleService.ValdidateLiftLoad(load, area, cargotyp, drivesystem))
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Nennlast enspricht nicht der EN81:20!", SetSeverity(severity)) { DependentParameter = new string[] { "var_Aufzugstyp", "var_Q", "var_A_Kabine" } });
            }
            else
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { "var_Aufzugstyp", "var_Q", "var_A_Kabine" } });
            }
        }
    }

    private void ValidateUCMValues(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(ParamterDictionary["var_Aggregat"].Value)
            || ParamterDictionary["var_Aggregat"].Value != "Ziehl-Abegg"
            || string.IsNullOrWhiteSpace(ParamterDictionary["var_Steuerungstyp"].Value))
        {
            ParamterDictionary["var_Erkennungsweg"].Value = "0";
            ParamterDictionary["var_Totzeit"].Value = "0";
            ParamterDictionary["var_Vdetektor"].Value = "0";

            if (name == "var_Steuerungstyp" && ParamterDictionary["var_Aggregat"].Value == "Ziehl-Abegg" && string.IsNullOrWhiteSpace(ParamterDictionary["var_Steuerungstyp"].Value))
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"UCM-Daten können nicht berechnet werden es ist kein Steuerung gewählt ist!", SetSeverity("Warning")));
            }
            return;
        }

        var currentLiftControlManufacturers = _parametercontext.Set<LiftControlManufacturer>().FirstOrDefault(x => x.Name == ParamterDictionary["var_Steuerungstyp"].Value);

        if (ParamterDictionary["var_Schachtinformationssystem"].Value == "Limax 33CP"
            || ParamterDictionary["var_Schachtinformationssystem"].Value == "NEW-Lift S1-Box"
            || ParamterDictionary["var_Schachtinformationssystem"].Value == "NEW-Lift S2 (FST-3)")
        {
            ParamterDictionary["var_Erkennungsweg"].Value = Convert.ToString(currentLiftControlManufacturers?.DetectionDistanceSIL3);
            ParamterDictionary["var_Totzeit"].Value = Convert.ToString(currentLiftControlManufacturers?.DeadTimeSIL3);
            ParamterDictionary["var_Vdetektor"].Value = Convert.ToString(currentLiftControlManufacturers?.SpeeddetectorSIL3);
        }
        else
        {
            var oldTotzeit = ParamterDictionary["var_Totzeit"].Value;
            var oldVdetektor = ParamterDictionary["var_Vdetektor"].Value;
            var newTotzeit = Convert.ToBoolean(ParamterDictionary["var_ElektrBremsenansteuerung"].Value) ?
                Convert.ToString(currentLiftControlManufacturers?.DeadTimeZAsbc4) :
                Convert.ToString(currentLiftControlManufacturers?.DeadTime);
            var newVdetektor = Convert.ToString(currentLiftControlManufacturers?.Speeddetector);

            if (oldTotzeit == newTotzeit && oldVdetektor == newVdetektor)
            {
                return;
            }

            ParamterDictionary["var_Erkennungsweg"].Value = Convert.ToString(currentLiftControlManufacturers?.DetectionDistance);
            ParamterDictionary["var_Totzeit"].Value = newTotzeit;
            ParamterDictionary["var_Vdetektor"].Value = newVdetektor;
        }
    }

    private void ValidateZAliftData(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(FullPathXml) || FullPathXml == pathDefaultAutoDeskTransfer)
            return;

        var zaHtmlPath = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html");
        if (!File.Exists(zaHtmlPath))
            return;

        if (string.IsNullOrWhiteSpace(ParamterDictionary["var_Aggregat"].Value))
        {
            ParamterDictionary["var_Aggregat"].Value = "Ziehl-Abegg";
            ParamterDictionary["var_Aggregat"].DropDownListValue = "Ziehl-Abegg";
        }

        var lastWriteTime = File.GetLastWriteTime(zaHtmlPath);

        if (lastWriteTime != ZaHtmlCreationTime)
        {
            var zaliftHtml = new HtmlDocument();
            zaliftHtml.Load(zaHtmlPath);
            var zliData = zaliftHtml.DocumentNode.SelectNodes("//comment()").FirstOrDefault(x => x.InnerHtml.StartsWith("<!-- zli"))?
                                                                            .InnerHtml.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (zliData is null)
                return;

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
            return;

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
            return;
        if (!name.StartsWith("var_Fuehrungsart"))
            return;

        var guideModels = _parametercontext.Set<GuideModelType>().ToList();
        var guideTyp = name.Replace("Fuehrungsart", "TypFuehrung");
        var selectedguideModel = ParamterDictionary[guideTyp].Value;

        IEnumerable<string?> availableguideModels = value switch
        {
            "Gleitführung" => guideModels.Where(x => x.GuideTypeId == 1).Select(s => s.Name),
            "Rollenführung" => guideModels.Where(x => x.GuideTypeId == 2).Select(s => s.Name),
            _ => guideModels.Select(s => s.Name),
        };

        if (availableguideModels is not null)
        {
            ParamterDictionary[guideTyp].DropDownList.Clear();
            foreach (var item in availableguideModels)
            {
                ParamterDictionary[guideTyp].DropDownList.Add(item!);
            }

            if (!string.IsNullOrWhiteSpace(selectedguideModel) && !availableguideModels.Contains(selectedguideModel))
            {
                ParamterDictionary[guideTyp].Value = string.Empty;
                ParamterDictionary[guideTyp].DropDownListValue = null;
            }
        }
    }

    private void ValidatePitLadder(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ParamterDictionary["var_SchachtgrubenleiterKontaktgesichert"].Value = "False";
            return;
        }
        if (value == "Schachtgrubenleiter EN81:20 mit el. Kontakt")
        {
            ParamterDictionary["var_SchachtgrubenleiterKontaktgesichert"].Value = "True";
        }
        else
        {
            ParamterDictionary["var_SchachtgrubenleiterKontaktgesichert"].Value = "False";
        }
    }

    private void ValidateDoorTyps(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!name.StartsWith("var_Tuertyp"))
            return;

        var liftDoorGroups = name.Replace("var_Tuertyp", "var_Tuerbezeichnung");

        if (string.IsNullOrWhiteSpace(value))
        {
            ParamterDictionary[liftDoorGroups].Value = string.Empty;
            ParamterDictionary[liftDoorGroups].DropDownListValue = null;
            ParamterDictionary[liftDoorGroups].DropDownList.Clear();
        }
        else
        {
            var selectedDoorSytem = ParamterDictionary[name].Value![..1];

            var availableLiftDoorGroups = _parametercontext.Set<LiftDoorGroup>().Where(x => x.DoorManufacturer!.StartsWith(selectedDoorSytem)).ToList();

            if (availableLiftDoorGroups is not null)
            {
                ParamterDictionary[liftDoorGroups].DropDownList.Clear();
                foreach (var item in availableLiftDoorGroups)
                {
                    ParamterDictionary[liftDoorGroups].DropDownList.Add(item.Name);
                }
            }
        }
        _ = ParamterDictionary[liftDoorGroups].ValidateParameterAsync().Result;
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
            ParamterDictionary[doorOpeningDirection].Value = string.Empty;
            ParamterDictionary[doorOpeningDirection].DropDownListValue = null;
            ParamterDictionary[doorPanelCount].Value = "0";
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
                    if (string.IsNullOrWhiteSpace(ParamterDictionary[doorOpeningDirection].Value) || !ParamterDictionary[doorOpeningDirection].Value!.StartsWith(liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection.Name))
                    {
                        ParamterDictionary[doorOpeningDirection].Value = liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection.Name;
                        ParamterDictionary[doorOpeningDirection].DropDownListValue = liftDoorGroup.ShaftDoor.LiftDoorOpeningDirection.Name;
                    }
                }
                ParamterDictionary[doorPanelCount].Value = Convert.ToString(liftDoorGroup.ShaftDoor.DoorPanelCount);
            }
        }
        if (ParamterDictionary[liftDoortyp].HasErrors)
            ParamterDictionary[liftDoortyp].ClearErrors("Value");
    }

    private void ValidateCarEquipmentPosition(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, name))
            return;

        var zugang = name.Last();
        bool hasSpiegel = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_Spiegel{zugang}");
        bool hasHandlauf = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_Handlauf{zugang}");
        bool hasSockelleiste = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_Sockelleiste{zugang}");
        bool hasRammschutz = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_Rammschutz{zugang}");
        bool hasPaneel = LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, $"var_PaneelPos{zugang}");
        bool hasSchutzgelaender = !string.IsNullOrWhiteSpace(ParamterDictionary[$"var_Schutzgelaender_{zugang}"].Value);
        bool hasRueckwand = false;
        if (zugang == 'C')
        {
            hasRueckwand = !string.IsNullOrWhiteSpace(ParamterDictionary["var_Rueckwand"].Value);
        }
        
        if (hasSpiegel || hasHandlauf || hasSockelleiste || hasRammschutz || hasPaneel || hasSchutzgelaender || hasRueckwand)
        {
            var errorMessage = $"Bei Zugang {zugang} wurde folgende Ausstattung gewählt:";
            if (hasSpiegel)
                errorMessage += " Spiegel,";
            if (hasHandlauf)
                errorMessage += " Handlauf,";
            if (hasSockelleiste)
                errorMessage += " Sockelleiste,";
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
        var controler = ParamterDictionary["var_Steuerungstyp"].Value;
        var liftPositionSystem = ParamterDictionary["var_Schachtinformationssystem"].Value;

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
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = new string[] { optional! } });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewähltes Schachtinformationssystem: {liftPositionSystem} ist mit der Steuerung: {controler} nicht zulässig!", SetSeverity(severity))
            { DependentParameter = new string[] { optional! } });
        }
    }

    private void ValidateFloorColorTyps(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ParamterDictionary["var_BodenbelagsTyp"].Value = string.Empty;
            ParamterDictionary["var_BodenbelagsTyp"].DropDownListValue = string.Empty;
            ParamterDictionary["var_BodenbelagsTyp"].DropDownList.Clear();
            return;
        }

        var floorColors = _parametercontext.Set<CarFloorColorTyp>().Include(i => i.CarFlooring).ToList();

        IEnumerable<string> availableFloorColors = floorColors.Where(x => x.CarFlooring?.Name == value).Select(s => s.Name);

        if (availableFloorColors is not null)
        {
            ParamterDictionary["var_BodenbelagsTyp"].DropDownList.Clear();
            foreach (var item in availableFloorColors)
            {
                ParamterDictionary["var_BodenbelagsTyp"].DropDownList.Add(item!);
            }

            if (!availableFloorColors.Contains(ParamterDictionary["var_BodenbelagsTyp"].Value))
            {
                ParamterDictionary["var_BodenbelagsTyp"].Value = string.Empty;
                ParamterDictionary["var_BodenbelagsTyp"].DropDownListValue = null;
            }
        }
    }

    private void ValidateEntryDimensions(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParamterDictionary, "var_AutogenerateFloorDoorData"))
            return;
        
        var zugang = string.Equals(name[^1..],"B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D")? name[^1..] : "A";


        if (name.StartsWith("var_TuerEinbau"))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ParamterDictionary[$"var_EinzugN_{zugang}"].Value = string.Empty;
                return;
            }

            var liftDoor = ParamterDictionary[zugang == "A" ?"var_Tuerbezeichnung": $"var_Tuerbezeichnung_{zugang}"].Value;
            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor).FirstOrDefault(x => x.Name == liftDoor);
            var doorEntrance = Convert.ToDouble(value, CultureInfo.CurrentCulture);
            if (liftDoorGroup is null || liftDoorGroup.CarDoor is null || liftDoorGroup.CarDoor.SillWidth >= doorEntrance)
            {
                ParamterDictionary[$"var_EinzugN_{zugang}"].Value = string.Empty;
                ParamterDictionary[$"var_Schwellenbreite{zugang}"].Value = string.Empty;
                return;
            }
            ParamterDictionary[$"var_EinzugN_{zugang}"].Value = (doorEntrance - liftDoorGroup.CarDoor.SillWidth).ToString();
            ParamterDictionary[$"var_Schwellenbreite{zugang}"].Value = liftDoorGroup.CarDoor.SillWidth.ToString();
            SetCarDesignParameterSill(zugang, liftDoorGroup);
        }
        else if (name.StartsWith("var_Tuerbezeichnung"))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ParamterDictionary[$"var_EinzugN_{zugang}"].Value = string.Empty;
                ParamterDictionary[$"var_Schwellenbreite{zugang}"].Value = string.Empty;
                return;
            }

            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor).FirstOrDefault(x => x.Name == value);
            var doorEntranceString = ParamterDictionary[zugang == "A" ? "var_TuerEinbau" : $"var_TuerEinbau{zugang}"].Value;
            if (!string.IsNullOrWhiteSpace (doorEntranceString))
            {
                var doorEntrance = Convert.ToDouble(doorEntranceString, CultureInfo.CurrentCulture);
                if (liftDoorGroup is null || liftDoorGroup.CarDoor is null || liftDoorGroup.CarDoor.SillWidth >= doorEntrance)
                {
                    ParamterDictionary[$"var_EinzugN_{zugang}"].Value = string.Empty;
                    ParamterDictionary[$"var_Schwellenbreite{zugang}"].Value = string.Empty;
                    return;
                }
                ParamterDictionary[$"var_EinzugN_{zugang}"].Value = (doorEntrance - liftDoorGroup.CarDoor.SillWidth).ToString();
                ParamterDictionary[$"var_Schwellenbreite{zugang}"].Value = liftDoorGroup.CarDoor.SillWidth.ToString();
                SetCarDesignParameterSill(zugang, liftDoorGroup);
            }
        }
        else if (name.StartsWith("var_SchwellenprofilKab") || name.StartsWith("var_TB"))
        {
            if (string.IsNullOrWhiteSpace(value) || value == "0")
                return;

            var liftDoor = ParamterDictionary[zugang == "A" ? "var_Tuerbezeichnung" : $"var_Tuerbezeichnung_{zugang}"].Value;
            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor).FirstOrDefault(x => x.Name == liftDoor);
            if (liftDoorGroup is not null)
            {
                SetCarDesignParameterSill(zugang, liftDoorGroup);
            }
        }
    }

    private void ValidateHydrauliclock(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.Equals(value,"False",StringComparison.CurrentCultureIgnoreCase))
        {
            ParamterDictionary["var_AufsetzvorrichtungSystem"].Value = string.Empty;
            ParamterDictionary["var_AufsetzvorrichtungSystem"].DropDownListValue = null;
        }
    }

    private void ValidateCounterweightMass(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        double load = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_Q");
        double carWeight = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_F");
        double balance = LiftParameterHelper.GetLiftParameterValue<double>(ParamterDictionary, "var_GGWNutzlastausgleich");

        ParamterDictionary["var_Gegengewichtsmasse"].Value = Convert.ToString(Math.Round(load * balance + carWeight));
    }

    private void ValidateProtectiveRailingSwitch(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        string[] sides = { "var_Schutzgelaender_A", "var_Schutzgelaender_B", "var_Schutzgelaender_C", "var_Schutzgelaender_D" };
        bool railingSwitch = false;

        foreach (var side in sides)
        {
           if (!string.IsNullOrWhiteSpace(ParamterDictionary[side].Value))
           {
                if (ParamterDictionary[side].Value!.Contains("klappbar") || ParamterDictionary[side].Value!.Contains("steckbar"))
                {
                    railingSwitch = true;
                    break;
                }
           }
        }
        ParamterDictionary["var_SchutzgelaenderKontakt"].Value = railingSwitch ? "True" : "False";
    }
}