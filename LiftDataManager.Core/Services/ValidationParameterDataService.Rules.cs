using HtmlAgilityPack;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Kabine;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using Microsoft.Extensions.Primitives;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq.Expressions;

namespace LiftDataManager.Core.Services;
public partial class ValidationParameterDataService : IValidationParameterDataService
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
        {
            return;
        }
        var anotherParameter = Convert.ToBoolean(_parameterDictionary[anotherBoolean].Value, CultureInfo.CurrentCulture);
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
        var anotherParameter = Convert.ToBoolean(_parameterDictionary[anotherBoolean].Value, CultureInfo.CurrentCulture);
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
        {
            return;
        }
        var anotherParameter = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, anotherBoolean!);
        if (!anotherParameter)
        {
            return;
        }

        if (string.Equals(value, "True", StringComparison.CurrentCultureIgnoreCase))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Es is nicht möglich beide Optionen ({displayname} und {_parameterDictionary[anotherBoolean].DisplayName}) auszuwählen!", SetSeverity(severity))
            { DependentParameter = [anotherBoolean] });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = [anotherBoolean] });
        }
    }

    private void NotTrueWhenTheOtherIsTrueForce(string name, string displayname, string? value, string? severity, string? anotherBoolean)
    {
        if (string.IsNullOrWhiteSpace(anotherBoolean))
        {
            return;
        }
        bool anotherParameter = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, anotherBoolean);
        if (!anotherParameter)
        {
            return;
        }
        if (string.Equals(value, "True", StringComparison.CurrentCultureIgnoreCase))
        {
            _parameterDictionary[anotherBoolean].AutoUpdateParameterValue("False");
        }
    }

    private void MustBeTrueWhenAnotherNotEmty(string name, string displayname, string? value, string? severity, string? anotherString)
    {
        if (string.IsNullOrWhiteSpace(anotherString))
            return;
        var valueToBool = Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        var stringValue = _parameterDictionary[anotherString].Value;

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
        var dropDownListValue = LiftParameterHelper.GetDropDownListValue(_parameterDictionary[name].DropDownList, value);
        if (dropDownListValue == null || dropDownListValue.Id == -1)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{displayname}: ungültiger Wert | {value} | ist nicht in der Auswahlliste vorhanden.", SetSeverity(severity)));
        }
    }

    // Spezial validationrules

    private void ValidateCreationDate(string name, string displayname, string? value, string? severity, string? odernummerName)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return;
        }
        _parameterDictionary["var_ErstelltAm"].Value = DateTime.Now.ToShortDateString();
    }

    private void ValidateJobNumber(string name, string displayname, string? value, string? severity, string? odernummerName)
    {
        if (string.IsNullOrWhiteSpace(odernummerName))
        {
            return;
        }
        var fabriknummer = _parameterDictionary["var_FabrikNummer"].Value;

        if (string.IsNullOrWhiteSpace(fabriknummer))
        {
            return;
        }
        _parameterDictionary["var_FabrikNummer"].ClearErrors("var_FabrikNummer");

        var auftragsnummer = _parameterDictionary[odernummerName].Value;
        var informationAufzug = _parameterDictionary["var_InformationAufzug"].Value;
        var fabriknummerBestand = _parameterDictionary["var_FabriknummerBestand"].Value;

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

    private void ValidateEntrancePosition(string name, string displayname, string? value, string? severity, string? odernummerName)
    {
        if (string.IsNullOrWhiteSpace(value) || 
            string.Equals(value,"NV") ||
            value.StartsWith("ZG_A"))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{value} : Haupthaltestelle sollte immer auf der Zugangsseite A liegen. Wenn dies nicht möglich ist halten Sie Rücksprache mit der Konstruktion.", SetSeverity(severity)));
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
        {
            return;
        }
        var foerderhoehe = Math.Round(LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_FH") * 1000);
        var etagenhoehe0 = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Etagenhoehe0");
        var etagenhoehe1 = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Etagenhoehe1");
        var etagenhoehe2 = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Etagenhoehe2");
        var etagenhoehe3 = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Etagenhoehe3");
        var etagenhoehe4 = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Etagenhoehe4");
        var etagenhoehe5 = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Etagenhoehe5");
        var etagenhoehe6 = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Etagenhoehe6");
        var etagenhoehe7 = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Etagenhoehe7");
        var etagenhoehe8 = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Etagenhoehe8");

        if ((etagenhoehe0 + etagenhoehe1 + etagenhoehe2 + etagenhoehe3 + etagenhoehe4 + etagenhoehe5 + etagenhoehe6 + etagenhoehe7 + etagenhoehe8) == 0)
        {
            return;
        }
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
        string bodentyp = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Bodentyp");
        string bodenProfil = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_BoPr");
        string bodenBelag = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Bodenbelag");
        double bodenBlech = string.IsNullOrWhiteSpace(_parameterDictionary["var_Bodenblech"].Value) ? -1 :LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Bodenblech");
        double bodenBelagHoehe = GetFlooringHeight(bodenBelag);
        double bodenHoehe = -1;

        switch (bodentyp)
        {
            case "standard":
                bodenHoehe = 83;
                _parameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("3");
                _parameterDictionary["var_BoPr"].AutoUpdateParameterValue("80 x 40 x 3");
                break;
            case "verstärkt":
                if (bodenBlech < 0)
                {
                    SetDefaultReinforcedFloor(name);
                }
                if (string.IsNullOrWhiteSpace(bodenProfil))
                {
                    SetDefaultReinforcedFloor(name);
                }
                if (bodenBlech == 3 && bodenProfil == "80 x 40 x 3")
                {
                    SetDefaultReinforcedFloor(name);
                }
                double bodenProfilHoehe = GetFloorProfilHeight(bodenProfil);
                bodenHoehe = bodenBlech + bodenProfilHoehe;
                break;
            case "standard mit Wanne":
                _parameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("5");
                _parameterDictionary["var_BoPr"].AutoUpdateParameterValue("80 x 40 x 3");
                bodenHoehe = 85;
                break;
            case "sonder":
                if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_Bodenblech"].Value))
                {
                    _parameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("(keine Auswahl)");
                }
                if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_BoPr"].Value))
                {
                    _parameterDictionary["var_BoPr"].AutoUpdateParameterValue("(keine Auswahl)");
                }
                bodenHoehe = -1;
                break;
            case "extern":
                if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_Bodenblech"].Value))
                {
                    _parameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("(keine Auswahl)");
                }
                if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_BoPr"].Value))
                {
                    _parameterDictionary["var_BoPr"].AutoUpdateParameterValue("(keine Auswahl)");
                }
                bodenHoehe = -1;
                break;
            default:
                if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_Bodenblech"].Value))
                {
                    _parameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("(keine Auswahl)");
                }
                if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_BoPr"].Value))
                {
                    _parameterDictionary["var_BoPr"].AutoUpdateParameterValue("(keine Auswahl)");
                }
                break;
        }

        _parameterDictionary["var_Bodenbelagsgewicht"].AutoUpdateParameterValue(GetFloorWeight(bodenBelag));
        if (bodenHoehe != -1)
        {
            _parameterDictionary["var_KU"].AutoUpdateParameterValue(Convert.ToString(bodenHoehe + bodenBelagHoehe));
        }
        _parameterDictionary["var_Bodenbelagsdicke"].AutoUpdateParameterValue(Convert.ToString(bodenBelagHoehe));

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
                return LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Bodenbelagsgewicht");
            if (string.Equals(bodenBelag, "Nach Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Bodenbelagsgewicht");
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
                return LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Bodenbelagsdicke");
            if (string.Equals(bodenBelag, "Nach Beschreibung"))
                return LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Bodenbelagsdicke");
            var boden = _parametercontext.Set<CarFlooring>().FirstOrDefault(x => x.Name == bodenBelag);
            if (boden is null)
                return 0;
            return (double)boden.Thickness!;
        }

        void SetDefaultReinforcedFloor(string name)
        {
            _parameterDictionary["var_Bodenblech"].AutoUpdateParameterValue("3");
            _parameterDictionary["var_BoPr"].AutoUpdateParameterValue("80 x 50 x 5");
            if (name == "var_BoPr")
            {
                _parameterDictionary["var_BoPr"].DropDownList.Add(new SelectionValue(-1, "Refresh", "Refresh"));
                _parameterDictionary["var_BoPr"].DropDownList.Remove(new SelectionValue(-1, "Refresh", "Refresh"));
            }
        }
    }

    private void ValidateCarEntranceRightSide(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        double kabinenBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_KBI");
        double kabinenTiefe = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_KTI");
        bool zugangA = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_ZUGANSSTELLEN_A");
        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_ZUGANSSTELLEN_D");
        double linkeSeite;
        double tuerBreite;

        switch (name)
        {
            case "var_L1" or "var_TB":
                if (!(kabinenBreite > 0))
                    return;
                if (!zugangA)
                {
                    _parameterDictionary["var_R1"].AutoUpdateParameterValue("0");
                    _parameterDictionary["var_L1"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_L1");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TB");
                    _parameterDictionary["var_R1"].AutoUpdateParameterValue(Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_L2" or "var_TB_C":
                if (!(kabinenBreite > 0))
                    return;
                if (!zugangC)
                {
                    _parameterDictionary["var_R2"].AutoUpdateParameterValue("0");
                    _parameterDictionary["var_L2"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_L2");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TB_C");
                    _parameterDictionary["var_R2"].AutoUpdateParameterValue(Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_L3" or "var_TB_B":
                if (!(kabinenTiefe > 0))
                    return;
                if (!zugangB)
                {
                    _parameterDictionary["var_R3"].AutoUpdateParameterValue("0");
                    _parameterDictionary["var_L3"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_L3");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TB_B");
                    _parameterDictionary["var_R3"].AutoUpdateParameterValue(Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_L4" or "var_TB_D":
                if (!(kabinenTiefe > 0))
                    return;
                if (!zugangD)
                {
                    _parameterDictionary["var_R4"].AutoUpdateParameterValue("0");
                    _parameterDictionary["var_L4"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_L4");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TB_D");
                    _parameterDictionary["var_R4"].AutoUpdateParameterValue(Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_KBI":
                if (!(kabinenBreite > 0))
                    return;
                if (!zugangA)
                {
                    _parameterDictionary["var_R1"].AutoUpdateParameterValue("0");
                    _parameterDictionary["var_L1"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_L1");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TB");
                    _parameterDictionary["var_R1"].AutoUpdateParameterValue(Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite)));
                }
                if (!zugangC)
                {
                    _parameterDictionary["var_R2"].AutoUpdateParameterValue("0");
                    _parameterDictionary["var_L2"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_L2");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TB_C");
                    _parameterDictionary["var_R2"].AutoUpdateParameterValue(Convert.ToString(kabinenBreite - (linkeSeite + tuerBreite)));
                }
                return;
            case "var_KTI":
                if (!(kabinenTiefe > 0))
                    return;
                if (!zugangB)
                {
                    _parameterDictionary["var_R3"].AutoUpdateParameterValue("0");
                    _parameterDictionary["var_L3"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_L3");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TB_B");
                    _parameterDictionary["var_R3"].AutoUpdateParameterValue(Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite)));
                }
                if (!zugangD)
                {
                    _parameterDictionary["var_R4"].AutoUpdateParameterValue("0");
                    _parameterDictionary["var_L4"].AutoUpdateParameterValue("0");
                }
                else
                {
                    linkeSeite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_L4");
                    tuerBreite = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_TB_D");
                    _parameterDictionary["var_R4"].AutoUpdateParameterValue(Convert.ToString(kabinenTiefe - (linkeSeite + tuerBreite)));
                }
                return;
            default:
                return;
        }
    }

    private void ValidateVariableCarDoors(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        bool variableTuerdaten = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_Variable_Tuerdaten");
        bool advancedDoorSelection = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_AdvancedDoorSelection");
        bool zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_ZUGANSSTELLEN_B");
        bool zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_ZUGANSSTELLEN_C");
        bool zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_ZUGANSSTELLEN_D");

        if (variableTuerdaten)
        {
            if (!zugangB)
            {
                RemoveShaftDoorDoorData("B");
            }
            if (!zugangC)
            {
                RemoveShaftDoorDoorData("C");
            }
            if (!zugangD)
            {
                RemoveShaftDoorDoorData("D");
            }
            return;
        }
        if (zugangB)
        {
            SetShaftDoorDoorData("B", advancedDoorSelection);
        }
        else
        {
            RemoveShaftDoorDoorData("B");
        }
        if (zugangC)
        {
            SetShaftDoorDoorData("C", advancedDoorSelection);
        }
        else
        {
            RemoveShaftDoorDoorData("C");
        }
        if (zugangD)
        {
            SetShaftDoorDoorData("D", advancedDoorSelection);
        }
        else
        {
            RemoveShaftDoorDoorData("D");
        }
    }

    private void ValidateCarFrameSelection(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var liftTypes = _parametercontext.Set<LiftType>().ToList();
        var currentLiftType = liftTypes.FirstOrDefault(x => x.Name == value);

        var driveTypeId = currentLiftType is not null ? currentLiftType.DriveTypeId : 1;

        var carframes = _parametercontext.Set<CarFrameType>().ToList();

        var availableCarframes = carframes.Where(x => x.DriveTypeId == driveTypeId).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });

        if (availableCarframes is not null)
        {
            UpdateDropDownList("var_Bausatz", availableCarframes);
        }

        CheckListContainsValue(_parameterDictionary["var_Bausatz"]);
    }

    private void ValidateReducedProtectionSpaces(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (name == "var_Ersatzmassnahmen")
        {
            if (string.IsNullOrWhiteSpace(value) || value == "keine")
            {
                _parameterDictionary["var_ErsatzmassnahmenSK"].AutoUpdateParameterValue("False");
                _parameterDictionary["var_ErsatzmassnahmenSG"].AutoUpdateParameterValue("False");
                _parameterDictionary["var_KontaktSchutzraumueberwachung"].AutoUpdateParameterValue("False");
                ResetReducedProtectionSpacesbuffer(_parameterDictionary["var_Ersatzmassnahmen"].DropDownListValue);
            }
            else if (value == "Vorausgelöstes Anhaltesystem" || value.StartsWith("Schachtkopf und Schachtgrube"))
            {
                _parameterDictionary["var_ErsatzmassnahmenSK"].AutoUpdateParameterValue("True");
                _parameterDictionary["var_ErsatzmassnahmenSG"].AutoUpdateParameterValue("True");
                _parameterDictionary["var_KontaktSchutzraumueberwachung"].AutoUpdateParameterValue("True");
                ResetReducedProtectionSpacesbuffer(_parameterDictionary["var_Ersatzmassnahmen"].DropDownListValue);
            }
            else if (value.StartsWith("Schachtkopf"))
            {
                _parameterDictionary["var_ErsatzmassnahmenSK"].AutoUpdateParameterValue("True");
                _parameterDictionary["var_ErsatzmassnahmenSG"].AutoUpdateParameterValue("False");
                _parameterDictionary["var_KontaktSchutzraumueberwachung"].AutoUpdateParameterValue("True");
                ResetReducedProtectionSpacesbuffer(_parameterDictionary["var_Ersatzmassnahmen"].DropDownListValue);
            }
            else if (value.StartsWith("Schachtgrube"))
            {
                _parameterDictionary["var_ErsatzmassnahmenSK"].AutoUpdateParameterValue("False");
                _parameterDictionary["var_ErsatzmassnahmenSG"].AutoUpdateParameterValue("True");
                _parameterDictionary["var_KontaktSchutzraumueberwachung"].AutoUpdateParameterValue("True");
                ResetReducedProtectionSpacesbuffer(_parameterDictionary["var_Ersatzmassnahmen"].DropDownListValue);
            }
        }

        var selectedSafetyGear = _parameterDictionary["var_TypFV"].Value;
        var selectedReducedProtectionSpace = _parameterDictionary["var_Ersatzmassnahmen"].Value;

        if (name == "var_TypFV")
        {
            var reducedProtectionSpaces = _parametercontext.Set<ReducedProtectionSpace>().ToList();
            IEnumerable<SelectionValue> availableReducedProtectionSpaces;

            if (string.IsNullOrWhiteSpace(selectedSafetyGear))
            {
                availableReducedProtectionSpaces = reducedProtectionSpaces.Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });
            }
            else if (selectedSafetyGear.Contains("BS"))
            {
                availableReducedProtectionSpaces = reducedProtectionSpaces.Where(x => x.Name.Contains(selectedSafetyGear) || 
                                                                           x.Name == "Schachtkopf" ||
                                                                           x.Name == "Schachtgrube" ||
                                                                           x.Name == "Schachtkopf und Schachtgrube" ||
                                                                           x.Name == "keine").Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });
            }
            else if (selectedSafetyGear.Contains("PC 13") || selectedSafetyGear.Contains("PC 24") || selectedSafetyGear.Contains("CSGB01") || selectedSafetyGear.Contains("CSGB02"))
            {
                availableReducedProtectionSpaces = reducedProtectionSpaces.Where(x => !x.Name.Contains("ESG")).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });
            }
            else
            {
                availableReducedProtectionSpaces = reducedProtectionSpaces.Where(x => !x.Name.Contains("ESG") && !x.Name.Contains("Voraus")).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });
            }

            if (availableReducedProtectionSpaces is not null)
            {
                UpdateDropDownList("var_Ersatzmassnahmen", availableReducedProtectionSpaces);
            }
        }

        if (!string.IsNullOrWhiteSpace(selectedSafetyGear) &&
            !string.IsNullOrWhiteSpace(selectedReducedProtectionSpace))
        {
            var selectedReducedProtectionSpaceValue = LiftParameterHelper.GetDropDownListValue(_parameterDictionary["var_Ersatzmassnahmen"].DropDownList, selectedReducedProtectionSpace);
            if (selectedReducedProtectionSpaceValue == null || selectedReducedProtectionSpaceValue.Id == -1)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Ersatzmassnahmen sind mit der Fangvorrichtung {selectedSafetyGear} nicht zulässig!", SetSeverity(severity))
                { DependentParameter = [optional!] });
            }
            else
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = [optional!] });
            }
        }
    }

    private void ValidateSafetyGear(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var selectedSafetyGear = _parameterDictionary["var_TypFV"].Value;
        var availablseafetyGears = FilterSafetyGears(value);
        if (availablseafetyGears is not null)
        {
            UpdateDropDownList("var_TypFV", availablseafetyGears);
            if (!string.IsNullOrWhiteSpace(selectedSafetyGear) && !availablseafetyGears.Any(x => x.Name == selectedSafetyGear))
            {
                _parameterDictionary["var_TypFV"].AutoUpdateParameterValue(string.Empty);
            }
        }
    }

    private void ValidateCwtSafetyGear(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_HasCwtSafetyGear")) 
        {
            if (!string.IsNullOrWhiteSpace(LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_TypFV_GGW")))
            {
                _parameterDictionary["var_TypFV_GGW"].AutoUpdateParameterValue(string.Empty);
            }
            return;
        }
        var selectedSafetyGear = _parameterDictionary["var_TypFV_GGW"].Value;
        var availablseafetyGears = FilterSafetyGears(value);
        if (availablseafetyGears is not null)
        {
            UpdateDropDownList("var_TypFV_GGW", availablseafetyGears);
            if (!string.IsNullOrWhiteSpace(selectedSafetyGear) && !availablseafetyGears.Any(x => x.Name == selectedSafetyGear))
            {
                _parameterDictionary["var_TypFV_GGW"].AutoUpdateParameterValue(string.Empty);
            }
        }
    }

    private void ValidateSafetyRange(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(_parameterDictionary["var_Q"].Value) ||
            string.IsNullOrWhiteSpace(_parameterDictionary["var_F"].Value) ||
            _parameterDictionary["var_F"].Value == "0" ||
            string.IsNullOrWhiteSpace(_parameterDictionary["var_Fuehrungsart"].Value) ||
            string.IsNullOrWhiteSpace(_parameterDictionary["var_FuehrungsschieneFahrkorb"].Value) ||
            string.IsNullOrWhiteSpace(_parameterDictionary["var_TypFV"].Value))
        {
            return;
        }

        var safetygearResult = _calculationsModuleService.GetSafetyGearCalculation(_parameterDictionary, false);
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

            var load = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, "var_Q");
            var carWeight = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, "var_F");

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

    private void ValidateCwtSafetyRange(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_HasCwtSafetyGear") ||
            string.IsNullOrWhiteSpace(_parameterDictionary["var_Gegengewichtsmasse"].Value) ||
            _parameterDictionary["var_Gegengewichtsmasse"].Value == "0" ||
            string.IsNullOrWhiteSpace(_parameterDictionary["var_Fuehrungsart_GGW"].Value) ||
            string.IsNullOrWhiteSpace(_parameterDictionary["var_FuehrungsschieneGegengewicht"].Value) ||
            string.IsNullOrWhiteSpace(_parameterDictionary["var_TypFV_GGW"].Value))
        {
            return;
        }

        var cwtSafetygearResult = _calculationsModuleService.GetSafetyGearCalculation(_parameterDictionary, true);
        if (cwtSafetygearResult is not null)
        {
            if (cwtSafetygearResult.PipeRuptureValve)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
                return;
            }

            if (!cwtSafetygearResult.RailHeadAllowed)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählter Schienenkopf ist für diese Gegengewichtsfangvorrichtung nicht zulässig.", SetSeverity(severity)));
                return;
            }

            var cwtLoad = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, "var_Gegengewichtsmasse");
 
            if (cwtSafetygearResult.MinLoad > cwtLoad)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Gegengewichtsfangvorrichtung nicht zulässig Minimalgewicht {cwtSafetygearResult.MinLoad} kg | Gegengewichtsmasse: {cwtLoad} kg unterschritten.", SetSeverity(severity)));
                return;
            }
            if (cwtSafetygearResult.MaxLoad < cwtLoad)
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"Ausgewählte Gegengewichtsfangvorrichtung nicht zulässig Maximalgewicht {cwtSafetygearResult.MaxLoad} kg | Gegengewichtsmasse: {cwtLoad} kg überschritten.", SetSeverity(severity)));
                return;
            }
        }
    }

    private void ValidateDriveSystemTypes(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        var driveSystems = _parametercontext.Set<DriveSystem>().Include(i => i.DriveSystemType)
                                                               .ToList();
        var currentdriveSystem = driveSystems.FirstOrDefault(x => x.Name == value);
        _parameterDictionary["var_Getriebe"].AutoUpdateParameterValue(currentdriveSystem is not null ? currentdriveSystem.DriveSystemType!.Name : string.Empty);
        _parameterDictionary["var_Getriebe"].AutoUpdateParameterValue(_parameterDictionary["var_Getriebe"].Value);
    }

    private void ValidateCarweightWithoutFrame(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }
        int carFrameWeight = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, "var_Rahmengewicht");
        string? fangrahmenTyp = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Bausatz");

        if (string.IsNullOrWhiteSpace(_parameterDictionary["var_Rahmengewicht"].Value) && !string.IsNullOrWhiteSpace(fangrahmenTyp))
        {
            var carFrameType = _parametercontext.Set<CarFrameType>().FirstOrDefault(x => x.Name == fangrahmenTyp);
            if (carFrameType is not null)
            {
                carFrameWeight = carFrameType.CarFrameWeight;
            }
        }

        if (carFrameWeight >= 0)
        {
            int carWeight = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, "var_F");
            if (carWeight > 0)
            {
                _parameterDictionary["var_KabTueF"].AutoUpdateParameterValue(Convert.ToString(carWeight - carFrameWeight));
            }
        }
    }

    private void ValidateCorrectionWeight(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }
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
            if (string.Equals(LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Normen"), "MRL 2006/42/EG"))
            {
                return;
            }
            double load = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Q");
            double reducedLoad = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Q1");
            double area = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_A_Kabine");
            string lift = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Aufzugstyp");
            var cargoTypDB = _parametercontext.Set<LiftType>().Include(i => i.CargoType)
                                                            .ToList()
                                                            .FirstOrDefault(x => x.Name == lift);
            var cargotyp = cargoTypDB is not null ? cargoTypDB.CargoType!.Name! : "Aufzugstyp noch nicht gewählt !";
            string drivesystem = GetDriveSystem(lift);
            if (string.Equals(cargotyp, "Lastenaufzug") && string.Equals(drivesystem, "Hydraulik"))
            {
                var loadTable6 = _calculationsModuleService.GetLoadFromTable(area, "Tabelle6");
                bool skipRatedLoad = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_SkipRatedLoad");
                if (reducedLoad < loadTable6 && !skipRatedLoad)
                {
                    _parameterDictionary["var_Q1"].AutoUpdateParameterValue(Convert.ToString(loadTable6));
                }
                if (reducedLoad < load && skipRatedLoad)
                {
                    _parameterDictionary["var_Q1"].AutoUpdateParameterValue(Convert.ToString(load));
                }
            }
            else
            {
                _parameterDictionary["var_Q1"].AutoUpdateParameterValue(Convert.ToString(load));
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
        if (string.IsNullOrWhiteSpace(_parameterDictionary["var_Aggregat"].Value)
            || _parameterDictionary["var_Aggregat"].Value != "Ziehl-Abegg"
            || string.IsNullOrWhiteSpace(_parameterDictionary["var_Steuerungstyp"].Value))
        {
            _parameterDictionary["var_Erkennungsweg"].AutoUpdateParameterValue("0");
            _parameterDictionary["var_Totzeit"].AutoUpdateParameterValue("0");
            _parameterDictionary["var_Vdetektor"].AutoUpdateParameterValue("0");

            if (name == "var_Steuerungstyp" && _parameterDictionary["var_Aggregat"].Value == "Ziehl-Abegg" && string.IsNullOrWhiteSpace(_parameterDictionary["var_Steuerungstyp"].Value))
            {
                ValidationResult.Add(new ParameterStateInfo(name, displayname, $"UCM-Daten können ohne Steuerungsauswahl nicht berechnet werden.", SetSeverity("Warning")));
            }
            return;
        }

        var currentLiftControlManufacturers = _parametercontext.Set<LiftControlManufacturer>().FirstOrDefault(x => x.Name == _parameterDictionary["var_Steuerungstyp"].Value);

        if (_parameterDictionary["var_Schachtinformationssystem"].Value == "Limax 33CP"
            || _parameterDictionary["var_Schachtinformationssystem"].Value == "NEW-Lift S1-Box"
            || _parameterDictionary["var_Schachtinformationssystem"].Value == "NEW-Lift S2 (FST-3)")
        {
            _parameterDictionary["var_Erkennungsweg"].AutoUpdateParameterValue(Convert.ToString(currentLiftControlManufacturers?.DetectionDistanceSIL3));
            _parameterDictionary["var_Totzeit"].AutoUpdateParameterValue(Convert.ToString(currentLiftControlManufacturers?.DeadTimeSIL3));
            _parameterDictionary["var_Vdetektor"].AutoUpdateParameterValue(Convert.ToString(currentLiftControlManufacturers?.SpeeddetectorSIL3));
        }
        else
        {
            var oldTotzeit = _parameterDictionary["var_Totzeit"].Value;
            var oldVdetektor = _parameterDictionary["var_Vdetektor"].Value;
            var newTotzeit = Convert.ToBoolean(_parameterDictionary["var_ElektrBremsenansteuerung"].Value) ?
                Convert.ToString(currentLiftControlManufacturers?.DeadTimeZAsbc4) :
                Convert.ToString(currentLiftControlManufacturers?.DeadTime);
            var newVdetektor = Convert.ToString(currentLiftControlManufacturers?.Speeddetector);

            if (oldTotzeit == newTotzeit && oldVdetektor == newVdetektor)
            {
                return;
            }

            _parameterDictionary["var_Erkennungsweg"].AutoUpdateParameterValue(Convert.ToString(currentLiftControlManufacturers?.DetectionDistance));
            _parameterDictionary["var_Totzeit"].AutoUpdateParameterValue(newTotzeit);
            _parameterDictionary["var_Vdetektor"].AutoUpdateParameterValue(newVdetektor);
        }
    }

    private void ValidateZAliftData(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(_fullPathXml))
        {
            return;
        }

        var zaHtmlPath = Path.Combine(Path.GetDirectoryName(_fullPathXml)!, "Berechnungen", SpezifikationsNumber + ".html");
        if (!File.Exists(zaHtmlPath))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_parameterDictionary["var_Aggregat"].Value))
        {
            _parameterDictionary["var_Aggregat"].AutoUpdateParameterValue("Ziehl-Abegg");
        }

        if (!string.Equals(_parameterDictionary["var_Aggregat"].Value, "Ziehl-Abegg"))
        {
            return;
        }

        var lastWriteTime = File.GetLastWriteTime(zaHtmlPath);

        if (lastWriteTime != ZaHtmlCreationTime)
        {
            var zaliftHtml = new HtmlDocument();
            zaliftHtml.Load(zaHtmlPath);
            var zliData = zaliftHtml.DocumentNode.SelectNodes("//comment()")?.FirstOrDefault(x => x.InnerHtml.StartsWith("<!-- zli"))?
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
            "var_F" => string.IsNullOrWhiteSpace(value) ? (0 - Convert.ToInt32(zaLiftValue)) <= 10 : Math.Abs(Convert.ToInt32(value) - Convert.ToInt32(zaLiftValue)) <= 10,
            "var_FH" => string.IsNullOrWhiteSpace(value) ? (0 - Convert.ToDouble(zaLiftValue) * 1000) <= 20 : Math.Abs(Convert.ToDouble(value) * 1000 - Convert.ToDouble(zaLiftValue) * 1000) <= 20,
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
        {
            return;
        }

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
        var selectedguideModel = _parameterDictionary[guideTyp].Value;
        IEnumerable<SelectionValue> availableguideModels = value switch
        {
            "Gleitführung" => guideModels.Where(x => x.GuideTypeId == 1).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection }),
            "Rollenführung" => guideModels.Where(x => x.GuideTypeId == 2).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection }),
            _ => guideModels.Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection }),
        };
        if (availableguideModels is not null)
        {
            UpdateDropDownList(guideTyp, availableguideModels);
            CheckListContainsValue(_parameterDictionary[guideTyp]);
        }
    }

    private void ValidatePitLadder(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _parameterDictionary["var_SchachtgrubenleiterKontaktgesichert"].AutoUpdateParameterValue("False");
            return;
        }
        _parameterDictionary["var_SchachtgrubenleiterKontaktgesichert"].AutoUpdateParameterValue(value == "Schachtgrubenleiter EN81:20 mit el. Kontakt" ? "True" : "False");
    }

    private void ValidateDoorTyps(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!name.StartsWith("var_Tuertyp"))
        {
            return;
        }

        if (LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_AdvancedDoorSelection"))
        {
            _parameterDictionary["var_Tuertyp"].DropDownListValue = new SelectionValue();
            _parameterDictionary["var_Tuertyp_B"].DropDownListValue = new SelectionValue();
            _parameterDictionary["var_Tuertyp_C"].DropDownListValue = new SelectionValue();
            _parameterDictionary["var_Tuertyp_D"].DropDownListValue = new SelectionValue();
            _parameterDictionary["var_Tuerbezeichnung"].DropDownListValue = new SelectionValue();
            _parameterDictionary["var_Tuerbezeichnung_B"].DropDownListValue = new SelectionValue();
            _parameterDictionary["var_Tuerbezeichnung_C"].DropDownListValue = new SelectionValue();
            _parameterDictionary["var_Tuerbezeichnung_D"].DropDownListValue = new SelectionValue();
        }

        var liftDoorGroups = name.Replace("var_Tuertyp", "var_Tuerbezeichnung");
        var shaftDoorInstallationTyp = string.Equals(name, "var_Tuertyp") ? "var_ShaftDoorInstallationTypA" : name.Replace("var_Tuertyp_", "var_ShaftDoorInstallationTyp");

        if (string.IsNullOrWhiteSpace(value))
        {
            _parameterDictionary[liftDoorGroups].AutoUpdateParameterValue(string.Empty);
            _parameterDictionary[liftDoorGroups].AutoUpdateParameterValue(null);
            _parameterDictionary[liftDoorGroups].DropDownList.Clear();
            _parameterDictionary[shaftDoorInstallationTyp].AutoUpdateParameterValue(null);
        }
        else
        {
            Expression<Func<LiftDoorGroup, bool>> filterDoorSystems;
            filterDoorSystems = _parameterDictionary[name].DropDownListValue?.Id switch
            {
                1 or 2 or 3 => x => x.DoorManufacturer! == "Meiller",
                4 => x => x.DoorManufacturer! == "Meiller" && x.Name.Contains("DT"),
                5 => x => x.DoorManufacturer! == "Wittur",
                6 => x => x.DoorManufacturer! == "Riedl",
                7 => x => x.DoorManufacturer! == "Meiller" && x.Name.Contains("Kompakt"),
                _ => x => x.DoorManufacturer! != "",
            };
            _parameterDictionary[shaftDoorInstallationTyp].AutoUpdateParameterValue(_parameterDictionary[name].DropDownListValue?.Id switch
            {
                1 => "Nischeneinbau",
                2 => "Schachteinbau",
                3 => "Modernisierung",
                4 => "Drehtuer",
                7 => "Kompakt",
                _ => "Nischeneinbau",
            });

            var availableLiftDoorGroups = _parametercontext.Set<LiftDoorGroup>().Where(filterDoorSystems)
                                                                                .Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName)
                                                                                {
                                                                                    IsFavorite = x.IsFavorite,
                                                                                    SchindlerCertified = x.SchindlerCertified,
                                                                                    OrderSelection = x.OrderSelection
                                                                                });
            if (availableLiftDoorGroups is not null)
            {
                UpdateDropDownList(liftDoorGroups, availableLiftDoorGroups);
                CheckListContainsValue(_parameterDictionary[liftDoorGroups]);
            }
        }
    }

    private void ValidateSetLiftDoors(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!name.StartsWith("var_Tuerbezeichnung") || LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_AdvancedDoorSelection"))
        {
            return;
        }
        var carDoorDescription = string.Equals(name, "var_Tuerbezeichnung") ? "var_CarDoorDescriptionA" : name.Replace("var_Tuerbezeichnung_", "var_CarDoorDescription");
        var shaftDoorDescription = string.Equals(name, "var_Tuerbezeichnung") ? "var_ShaftDoorDescriptionA" : name.Replace("var_Tuerbezeichnung_", "var_ShaftDoorDescription");

        if (string.IsNullOrWhiteSpace(value))
        {
            _parameterDictionary[carDoorDescription].AutoUpdateParameterValue(string.Empty);
            _parameterDictionary[shaftDoorDescription].AutoUpdateParameterValue(string.Empty);
        }
        else
        {
            var liftDoorGroup = _parametercontext.Set<LiftDoorGroup>().Include(i => i.CarDoor)
                                                                      .Include(i => i.ShaftDoor)
                                                                      .FirstOrDefault(x => x.Name == value);
            if (liftDoorGroup is not null && liftDoorGroup.ShaftDoor is not null && liftDoorGroup.CarDoor is not null)
            {
                _parameterDictionary[carDoorDescription].AutoUpdateParameterValue(liftDoorGroup.CarDoor.Name);
                _parameterDictionary[shaftDoorDescription].AutoUpdateParameterValue(liftDoorGroup.ShaftDoor.Name);
            }
        }
    }

    private void ValidateDoorData(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!name.StartsWith("var_ShaftDoorDescription"))
        {
            return;
        }

        var entrance = name[^1];
        var doorOpeningDirection = entrance switch
        {
            'A' => "var_Tueroeffnung",
            'B' => "var_Tueroeffnung_B",
            'C' => "var_Tueroeffnung_C",
            'D' => "var_Tueroeffnung_D",
            _ => string.Empty,
        };
        var doorPanelCount = entrance switch
        {
            'A' => "var_AnzahlTuerfluegel",
            'B' => "var_AnzahlTuerfluegel_B",
            'C' => "var_AnzahlTuerfluegel_C",
            'D' => "var_AnzahlTuerfluegel_D",
            _ => string.Empty,
        };
        if (string.IsNullOrWhiteSpace(doorOpeningDirection) || string.IsNullOrWhiteSpace(doorPanelCount))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            _parameterDictionary[doorOpeningDirection].AutoUpdateParameterValue(string.Empty);
            _parameterDictionary[doorPanelCount].AutoUpdateParameterValue("0");
        }
        else
        {
            var shaftDoor = _parametercontext.Set<ShaftDoor>().Include(i => i.LiftDoorOpeningDirection)
                                                              .FirstOrDefault(x => x.Name == value);
            if (shaftDoor is not null )
            {
                _parameterDictionary[doorPanelCount].AutoUpdateParameterValue(Convert.ToString(shaftDoor.DoorPanelCount));

                if (shaftDoor.LiftDoorOpeningDirection is null)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(_parameterDictionary[doorOpeningDirection].Value) || 
                   !_parameterDictionary[doorOpeningDirection].Value!.StartsWith(shaftDoor.LiftDoorOpeningDirection.Name))
                {
                    _parameterDictionary[doorOpeningDirection].AutoUpdateParameterValue(shaftDoor.LiftDoorOpeningDirection.Name);
                }
            }
        }
    }

    private void ValidateCarEquipmentPosition(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, name))
        {
            return;
        }

        var zugang = name.Last();
        bool hasSpiegel = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, $"var_Spiegel{zugang}");
        bool hasHandlauf = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, $"var_Handlauf{zugang}");
        bool hasRammschutz = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, $"var_Rammschutz{zugang}");
        bool hasPaneel = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, $"var_PaneelPos{zugang}");
        bool hasSchutzgelaender = !string.IsNullOrWhiteSpace(_parameterDictionary[$"var_Schutzgelaender_{zugang}"].Value);
        bool hasTeilungsleiste = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, $"var_Teilungsleiste{zugang}");
        bool hasRueckwand = false;
        if (zugang == 'C')
        {
            hasRueckwand = !string.IsNullOrWhiteSpace(_parameterDictionary["var_Rueckwand"].Value);
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
        var controler = _parameterDictionary["var_Steuerungstyp"].Value;
        var liftPositionSystem = _parameterDictionary["var_Schachtinformationssystem"].Value;

        if (string.IsNullOrWhiteSpace(controler) || string.IsNullOrWhiteSpace(liftPositionSystem))
        {
            return;
        }

        var validSystem = liftPositionSystem switch
        {
            "Limax 33CP" => string.Equals(controler, "Kühn MSZ 9E") || string.Equals(controler, "Kühn MSZ 10"),
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
            _parameterDictionary["var_BodenbelagsTyp"].AutoUpdateParameterValue(string.Empty);
            _parameterDictionary["var_BodenbelagsTyp"].AutoUpdateParameterValue(string.Empty);
            _parameterDictionary["var_BodenbelagsTyp"].DropDownList.Clear();
            return;
        }

        IEnumerable<SelectionValue> availableFloorColors = _parametercontext.Set<CarFloorColorTyp>().Include(i => i.CarFlooring).Where(x => x.CarFlooring!.Name == value).Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified, OrderSelection = x.OrderSelection });

        if (availableFloorColors is not null)
        {
            if (!availableFloorColors.Any())
            {
                _parameterDictionary["var_BodenbelagsTyp"].AutoUpdateParameterValue(string.Empty);
            }
            UpdateDropDownList("var_BodenbelagsTyp", availableFloorColors);
            CheckListContainsValue(_parameterDictionary["var_BodenbelagsTyp"]);
        }
    }

    private void ValidateEntryDimensions(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_AutogenerateFloorDoorData"))
        {
            return;
        }
        var zugang = name.StartsWith("var_TB") ? string.Equals(name[^1..], "_B") || string.Equals(name[^1..], "_C") || string.Equals(name[^1..], "_D") ? name[^1..] : "A"
                                               : string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";

        string carDoorName = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, $"var_CarDoorDescription{zugang}");
        var carDoor = _parametercontext.Set<CarDoor>().FirstOrDefault(x => x.Name == carDoorName);

        if (name.StartsWith("var_TuerEinbau"))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _parameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue(string.Empty);
                return;
            }

            var doorEntrance = Convert.ToDouble(value, CultureInfo.CurrentCulture);
            if (carDoor is null || carDoor.SillWidth >= doorEntrance)
            {
                _parameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue(string.Empty);
                _parameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(string.Empty);
                return;
            }
            _parameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue((doorEntrance - carDoor.SillWidth).ToString());
            _parameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(carDoor.SillWidth.ToString());
            SetCarDesignParameterSill(zugang, carDoor);
        }
        else if (name.StartsWith("var_Tuerbezeichnung"))
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _parameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue(string.Empty);
                _parameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(string.Empty);
                return;
            }

            var doorEntranceString = _parameterDictionary[zugang == "A" ? "var_TuerEinbau" : $"var_TuerEinbau{zugang}"].Value;
            if (!string.IsNullOrWhiteSpace(doorEntranceString))
            {
                var doorEntrance = Convert.ToDouble(doorEntranceString, CultureInfo.CurrentCulture);
                if (carDoor is null || carDoor.SillWidth >= doorEntrance)
                {
                    _parameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue(string.Empty);
                    _parameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(string.Empty);
                    return;
                }
                _parameterDictionary[$"var_EinzugN_{zugang}"].AutoUpdateParameterValue((doorEntrance - carDoor.SillWidth).ToString());
                _parameterDictionary[$"var_Schwellenbreite{zugang}"].AutoUpdateParameterValue(carDoor.SillWidth.ToString());
                SetCarDesignParameterSill(zugang, carDoor);
            }
        }
        else if (name.StartsWith("var_SchwellenprofilKab") || name.StartsWith("var_TB") || name.StartsWith("var_Tueroeffnung"))
        {
            if (string.IsNullOrWhiteSpace(value) || value == "0")
            {
                return;
            }
            SetCarDesignParameterSill(zugang, carDoor);
        }
    }

    private void ValidateHydrauliclock(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.Equals(value, "False", StringComparison.CurrentCultureIgnoreCase))
        {
            _parameterDictionary["var_AufsetzvorrichtungSystem"].AutoUpdateParameterValue(string.Empty);
        }
    }

    private void ValidateCounterweightMass(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }
        string lift = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Aufzugstyp");
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
            double load = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Q");
            double carWeight = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_F");
            double balance = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_GGWNutzlastausgleich");
            double cwtWidth = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Gegengewicht_Einlagenbreite");
            double cwtDepth = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_Gegengewicht_Einlagentiefe");
            double cwtFrameWeight = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_GGW_Rahmen_Gewicht");
            cwtLoad = Math.Round(load * balance + carWeight);
            cwtFillingLoad = cwtLoad - cwtFrameWeight;
            double fillingDensity = cwtWidth * cwtDepth * 7.85d * 0.000001;
            cwtFillingHeight = fillingDensity > 0 ? Math.Round(cwtFillingLoad / fillingDensity) : 0;
        }
        _parameterDictionary["var_Gegengewichtsmasse"].AutoUpdateParameterValue(Convert.ToString(cwtLoad));
        _parameterDictionary["var_GGW_Fuellgewicht"].AutoUpdateParameterValue(Convert.ToString(cwtFillingLoad));
        _parameterDictionary["var_GGW_Fuellhoehe"].AutoUpdateParameterValue(Convert.ToString(cwtFillingHeight));
    }

    private void ValidateProtectiveRailingSwitch(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        string[] sides = ["var_Schutzgelaender_A", "var_Schutzgelaender_B", "var_Schutzgelaender_C", "var_Schutzgelaender_D"];
        bool railingSwitch = false;

        foreach (var side in sides)
        {
            if (!string.IsNullOrWhiteSpace(_parameterDictionary[side].Value))
            {
                if (_parameterDictionary[side].Value!.Contains("klappbar") || _parameterDictionary[side].Value!.Contains("steckbar"))
                {
                    railingSwitch = true;
                    break;
                }
            }
        }
        _parameterDictionary["var_SchutzgelaenderKontakt"].AutoUpdateParameterValue(railingSwitch ? "True" : "False");
    }

    private void ValidateMirrorDimensions(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_AutoDimensionsMirror"))
        {
            return;
        }
        List<string> mirrors = [];
        if (LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_SpiegelA"))
        {
            mirrors.Add("A");
        }
        if (LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_SpiegelB"))
        {
            mirrors.Add("B");
        }
        if (LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_SpiegelC"))
        {
            mirrors.Add("C");
        }
        if (LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_SpiegelD"))
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
            var mirrorWidthSet = _calculationsModuleService.GetMirrorWidth(_parameterDictionary, mirrors[0], 1);
            var mirrorHeightSet = _calculationsModuleService.GetMirrorHeight(_parameterDictionary, mirrors[0], 1);

            mirrorWidth = mirrorWidthSet.Item1.ToString();
            mirrorHeight = mirrorHeightSet.Item1.ToString();
            mirrorDistanceLeft = mirrorWidthSet.Item2.ToString();
            mirrorDistanceCeiling = mirrorHeightSet.Item2.ToString();
        }
        else if (mirrors.Count == 2)
        {
            var mirrorWidthSet = _calculationsModuleService.GetMirrorWidth(_parameterDictionary, mirrors[0], 1);
            var mirrorHeightSet = _calculationsModuleService.GetMirrorHeight(_parameterDictionary, mirrors[0], 1);
            var mirrorWidthSet2 = _calculationsModuleService.GetMirrorWidth(_parameterDictionary, mirrors[1], 2);
            var mirrorHeightSet2 = _calculationsModuleService.GetMirrorHeight(_parameterDictionary, mirrors[1], 2);

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
            var mirrorWidthSet = _calculationsModuleService.GetMirrorWidth(_parameterDictionary, mirrors[0], 1);
            var mirrorHeightSet = _calculationsModuleService.GetMirrorHeight(_parameterDictionary, mirrors[0], 1);
            var mirrorWidthSet2 = _calculationsModuleService.GetMirrorWidth(_parameterDictionary, mirrors[1], 2);
            var mirrorHeightSet2 = _calculationsModuleService.GetMirrorHeight(_parameterDictionary, mirrors[1], 2);
            var mirrorWidthSet3 = _calculationsModuleService.GetMirrorWidth(_parameterDictionary, mirrors[2], 3);
            var mirrorHeightSet3 = _calculationsModuleService.GetMirrorHeight(_parameterDictionary, mirrors[2], 3);

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

        _parameterDictionary["var_BreiteSpiegel"].AutoUpdateParameterValue(mirrorWidth);
        _parameterDictionary["var_HoeheSpiegel"].AutoUpdateParameterValue(mirrorHeight);
        _parameterDictionary["var_BreiteSpiegel2"].AutoUpdateParameterValue(mirrorWidth2);
        _parameterDictionary["var_HoeheSpiegel2"].AutoUpdateParameterValue(mirrorHeight2);
        _parameterDictionary["var_BreiteSpiegel3"].AutoUpdateParameterValue(mirrorWidth3);
        _parameterDictionary["var_HoeheSpiegel3"].AutoUpdateParameterValue(mirrorHeight3);
        _parameterDictionary["var_AbstandSpiegelvonLinks"].AutoUpdateParameterValue(mirrorDistanceLeft);
        _parameterDictionary["var_AbstandSpiegelvonLinks2"].AutoUpdateParameterValue(mirrorDistanceLeft2);
        _parameterDictionary["var_AbstandSpiegelvonLinks3"].AutoUpdateParameterValue(mirrorDistanceLeft3);
        _parameterDictionary["var_AbstandSpiegelDecke"].AutoUpdateParameterValue(mirrorDistanceCeiling);
        _parameterDictionary["var_AbstandSpiegelDecke2"].AutoUpdateParameterValue(mirrorDistanceCeiling2);
        _parameterDictionary["var_AbstandSpiegelDecke3"].AutoUpdateParameterValue(mirrorDistanceCeiling3);
    }

    private void ValidateDoorSill(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!(name.StartsWith("var_Tuertyp") || name.StartsWith("var_CarDoorDescription") || string.Equals(name, "var_EN8171Cat012")))
        {
            return;
        }
        var zugang = string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";

        var shaftSillParameterName = string.Equals(zugang, "A") ? "var_Schwellenprofil" : $"var_Schwellenprofil{zugang}";
        var carSillParameterName = string.Equals(zugang, "A") ? "var_SchwellenprofilKabTuere" : $"var_SchwellenprofilKabTuere{zugang}";

        IEnumerable<SelectionValue> availableDoorSills;

        string carDoorName = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, $"var_CarDoorDescription{zugang}");
        availableDoorSills = GetAvailableDoorSills(carDoorName);

        UpdateDropDownList(shaftSillParameterName, availableDoorSills);
        CheckListContainsValue(_parameterDictionary[shaftSillParameterName]);

        UpdateDropDownList(carSillParameterName, availableDoorSills);
        CheckListContainsValue(_parameterDictionary[carSillParameterName]);
    }

    private void ValidateCarDoorHeaders(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (!name.StartsWith("var_CarDoorDescription"))
        {
            return;
        }
        var zugang = string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";

        if (string.IsNullOrWhiteSpace(value))
        {
            _parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownList.Clear();
            _parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownList.Clear();
            _parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].AutoUpdateParameterValue(string.Empty);
            _parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].AutoUpdateParameterValue(string.Empty);
        }

        var carDoor = _parametercontext.Set<CarDoor>().FirstOrDefault(x => x.Name == value);
        var carDoorHeaderDepths = carDoor?.CarDoorHeaderDepth.ToImmutableList();
        var carDoorHeaderHeights = carDoor?.CarDoorHeaderHeight.ToImmutableList();

        if (carDoorHeaderDepths is not null)
        {
            List<SelectionValue> availablcarDoorHeaderDepths = [];
            for (int i = 0; i < carDoorHeaderDepths.Count; i++)
            {
                availablcarDoorHeaderDepths.Add(new SelectionValue(i + 1, carDoorHeaderDepths[i].ToString(), carDoorHeaderDepths[i].ToString()) { IsFavorite = false, SchindlerCertified = false });
            }
            UpdateDropDownList($"var_KabTuerKaempferBreite{zugang}", availablcarDoorHeaderDepths, false);

            if (_parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownList.Count == 1)
            {
                _parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].AutoUpdateParameterValue(_parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].DropDownList[0].Name);
            }
            else
            {
                CheckListContainsValue(_parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"]);
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

            if (_parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownList.Count == 1)
            {
                _parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].AutoUpdateParameterValue(_parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"].DropDownList[0].Name);
            }
            else
            {
                CheckListContainsValue(_parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"]);
            }
        }
    }

    private void ValidateReducedCarDoorHeaderHeight(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value) && !string.Equals(name, "var_Tuerverriegelung"))
        {
            return;
        }
        var zugang = name.StartsWith("var_TB") ? string.Equals(name[^1..], "_B") || string.Equals(name[^1..], "_C") || string.Equals(name[^1..], "_D") ? name[^1..] : "A"
                                               : string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";

        var carDoorHeaderDepthParameter = _parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"];
        var carDoorHeaderHeightParameter = _parameterDictionary[$"var_KabTuerKaempferHoehe{zugang}"];

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
                if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_Tuerverriegelung"].Value) && _parameterDictionary["var_Tuerverriegelung"].Value!.Contains("54"))
                {
                    carDoorHeaderHeightParameter.AddError("Value", new ParameterStateInfo(carDoorHeaderHeightParameter.Name!, carDoorHeaderHeightParameter.DisplayName!, errorMessage, ErrorLevel.Error, false));
                    return;
                }

                int doorWidth = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, string.Equals(zugang, "A") ? "var_TB" : $"var_TB_{zugang}");

                switch (_parameterDictionary[string.Equals(zugang, "A") ? "var_AnzahlTuerfluegel" : $"var_AnzahlTuerfluegel_{zugang}"].Value)
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
        if (!LiftParameterHelper.IsDefaultCarTyp(_parameterDictionary["var_Fahrkorbtyp"].Value))
        {
            return;
        }
        var ruleActivationDate = new DateTime(2024, 01, 11);
        var creationDate = DateTime.MinValue;
        if (DateTime.TryParse(LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_ErstelltAm"), out DateTime parsedDate))
            creationDate = parsedDate;
        if (!string.IsNullOrWhiteSpace(_parameterDictionary["var_KD"].Value) && ruleActivationDate.CompareTo(creationDate) > 0)
        {
            return;
        }
        switch (name)
        {
            case "var_KBI" or "var_overrideDefaultCeiling":
                _parameterDictionary["var_GrundDeckenhoehe"].AutoUpdateParameterValue(GetDefaultCeiling().ToString());
                break;
            case "var_abgDecke" or "var_overrideSuspendedCeiling":
                var suspendedCeilingHeight = GetSuspendedCeiling();
                if (suspendedCeilingHeight == 0)
                {
                    _parameterDictionary["var_abgeDeckeHoehe"].AutoUpdateParameterValue(string.Empty);
                    _parameterDictionary["var_overrideSuspendedCeiling"].AutoUpdateParameterValue(string.Empty);
                }
                else
                {
                    _parameterDictionary["var_abgeDeckeHoehe"].AutoUpdateParameterValue(suspendedCeilingHeight.ToString());
                }
                break;
            default:
                break;
        }

        double cRail = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_DeckenCSchienenHoehe");
        double defaultCeiling = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_GrundDeckenhoehe");
        double suspendedCeiling = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_abgeDeckeHoehe");

        _parameterDictionary["var_KD"].AutoUpdateParameterValue(Convert.ToString(cRail + defaultCeiling + suspendedCeiling));
    }

    private void ValidateCarHeight(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        double bodenHoehe = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_KU");
        double kabinenHoeheInnen = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_KHLicht");
        double kabinenHoeheAussen = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_KHA");
        double deckenhoehe = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_KD");

        if (bodenHoehe + kabinenHoeheInnen + deckenhoehe == kabinenHoeheAussen)
        {
            return;
        }
        _parameterDictionary["var_KHA"].AutoUpdateParameterValue(Convert.ToString(bodenHoehe + kabinenHoeheInnen + deckenhoehe));
    }

    private void ValidateCarHeightExcludingSuspendedCeiling(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        if (string.IsNullOrWhiteSpace(value) && string.Equals(name, "var_KHLicht"))
        {
            _parameterDictionary["var_KHRoh"].AutoUpdateParameterValue(string.Empty);
            return;
        }

        var suspendedCeilingHeight = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_abgeDeckeHoehe");
        var carHeight = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_KHLicht");
        _parameterDictionary["var_KHRoh"].AutoUpdateParameterValue(Convert.ToString(carHeight + suspendedCeilingHeight, CultureInfo.CurrentCulture));
    }

    private void ValidateGlassPanelColor(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        switch (name)
        {
            case "var_Paneelmaterial":
                if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("ESG"))
                {
                    _parameterDictionary["var_PaneelmaterialGlas"].AutoUpdateParameterValue(string.Empty);
                    _parameterDictionary["var_SchattenfugenGlaspaneele"].AutoUpdateParameterValue(string.Empty);
                    _parameterDictionary["var_PaneelGlasRAL"].AutoUpdateParameterValue(string.Empty);
                }
                break;
            case "var_PaneelmaterialGlas":
                if (string.IsNullOrWhiteSpace(value) || !(value.StartsWith("Euro") || value.StartsWith("Weissglas")))
                {
                    _parameterDictionary["var_PaneelGlasRAL"].AutoUpdateParameterValue(string.Empty);
                }
                break;
            default:
                break;
        }
    }

    private void ValidateCarFramePosition(string name, string displayname, string? value, string? severity, string? optional = null)
    {
        Dictionary<string, bool> entrances = new()
        {
            {"A", false },
            {"B", false },
            {"C", false },
            {"D", false },
        };
        var availableCarFramePositions = _parametercontext.Set<CarFramePosition>().Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName) { IsFavorite = x.IsFavorite, SchindlerCertified = x.SchindlerCertified }).ToList();
        foreach (var entrance in entrances)
        {
            entrances[entrance.Key] = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, $"var_ZUGANSSTELLEN_{entrance.Key}");
            if (entrances[entrance.Key])
            {
                var selectedEntrance = availableCarFramePositions.FirstOrDefault(x => x.Name == entrance.Key);
                if (selectedEntrance != null)
                {
                    availableCarFramePositions.Remove(selectedEntrance);
                }
            }
        }
        _parameterDictionary["var_Durchladung"].AutoUpdateParameterValue(entrances["A"] && entrances["C"] || entrances["B"] && entrances["D"] ? "True" : "False");

        if (!entrances["C"])
        {
            var carFrame = _calculationsModuleService.GetCarFrameTyp(_parameterDictionary);
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

        var carFramePosition = _parameterDictionary["var_Bausatzlage"].Value;
        if (!string.IsNullOrWhiteSpace(carFramePosition))
        {
            switch (carFramePosition)
            {
                case "A":
                    _parameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("False");
                    _parameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("False");
                    _parameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("False");
                    break;
                case "B":
                    _parameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("False");
                    _parameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("True");
                    _parameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("False");
                    break;
                case "C":
                    _parameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("False");
                    _parameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("False");
                    _parameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("True");
                    break;
                case "D":
                    _parameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("True");
                    _parameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("False");
                    _parameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("False");
                    break;
                default:
                    _parameterDictionary["var_RahmenPosL"].AutoUpdateParameterValue("False");
                    _parameterDictionary["var_RahmenPosR"].AutoUpdateParameterValue("False");
                    _parameterDictionary["var_RahmenPosH"].AutoUpdateParameterValue("False");
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
            double load = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, loadName);
            _parameterDictionary[$"{loadName}_AZ"].Value = LiftParameterHelper.GetLayoutDrawingLoad(load).ToString();
        }
    }

    private void ValidateCarFrameProgramData(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(_fullPathXml))
        {
            return;
        }
        var cFPPath = Path.Combine(Path.GetDirectoryName(_fullPathXml)!, "Berechnungen", SpezifikationsNumber + ".dat");
        if (!File.Exists(cFPPath))
        {
            return;
        }
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

            var cFPDataFileLines = cFPDataFile.Split([Environment.NewLine], StringSplitOptions.None);

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
            "var_Stichmass" => "Stichmaß",
            "var_v" => "Sollgeschwindigkeit abw",
            "var_TypFV" => "Fangvorrichtung",
            "var_FuehrungsschieneFahrkorb" => "Schienentyp",
            "var_FuehrungsschieneGegengewicht" => "Hilfsschienentyp",
            "var_Geschwindigkeitsbegrenzer" => "GB_ID",
            "var_TypFuehrung" => "Fuehrungsart",
            "var_Puffertyp" => "Puffer",
            "var_RHU" => _calculationsModuleService.IsRopeLift(_parameterDictionary["var_Bausatz"].DropDownListValue) ? "Reservehub_Zylinder_unten" : "Reservehub_Kabine_unten",
            "var_RHO" => "Reservehub_Zylinder_oben",
            _ => string.Empty,
        };

        if (CFPDataDictionary.TryGetValue(searchString, out string? cFPValue))
        {
            if (cFPValue is not null)
            {
                var isValid = name switch
                {
                    "var_Q" => string.Equals(value, cFPValue, StringComparison.CurrentCultureIgnoreCase) || (value ?? string.Empty) == (cFPValue ?? string.Empty),
                    "var_F" => Math.Abs(Convert.ToInt32(value) - Convert.ToInt32(cFPValue)) <= 10,
                    "var_FH" => Math.Abs(Convert.ToDouble(value) * 1000 - Convert.ToDouble(cFPValue) * 1000) <= 20,
                    "var_SG" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_SK" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_SB" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_ST" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_KBI" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_KTI" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_RHU" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_RHO" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_KHLicht" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_KHA" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_Stichmass" => LiftParameterHelper.ConvertStringToDouble(value) == Math.Round(LiftParameterHelper.ConvertStringToDouble(cFPValue) * 1000),
                    "var_v" => LiftParameterHelper.ConvertStringToDouble(value) == LiftParameterHelper.ConvertStringToDouble(cFPValue),
                    "var_TypFV" => value switch
                    {
                        "Bucher RSG55" => true,
                        "Bucher RSG70" => true,
                        "Bucher RSG90" => true,
                        _ => string.Equals(value, cFPValue, StringComparison.CurrentCultureIgnoreCase) || (value ?? string.Empty) == (cFPValue ?? string.Empty),
                    },
                    "var_FuehrungsschieneFahrkorb" => string.Equals(value, cFPValue, StringComparison.CurrentCultureIgnoreCase) || (value ?? string.Empty) == (cFPValue ?? string.Empty),
                    "var_FuehrungsschieneGegengewicht" => string.Equals(value, cFPValue, StringComparison.CurrentCultureIgnoreCase) || (value ?? string.Empty) == (cFPValue ?? string.Empty),
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
                        "14" => string.Equals(value, "kein GB", StringComparison.CurrentCultureIgnoreCase) || string.Equals(value, "GB Ersatz durch Limax", StringComparison.CurrentCultureIgnoreCase),
                        "15" => string.Equals(value, "HJ200, FA u. el. Vorab. 230V (elektrom. Rückst.)", StringComparison.CurrentCultureIgnoreCase),
                        "16" => string.Equals(value, "HJ200, AS 24V, el. Vorab. 230V (elektrom. Rückst.)", StringComparison.CurrentCultureIgnoreCase),
                        "18" => string.Equals(value, "HJ300, FA u. el. Vorab. 230V (elektrom. Rückst.)", StringComparison.CurrentCultureIgnoreCase),
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
                    "var_Puffertyp" => string.Equals(value, cFPValue, StringComparison.CurrentCultureIgnoreCase) || (value ?? string.Empty) == (cFPValue ?? string.Empty),
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
        if (string.Equals(_parameterDictionary["var_Fahrkorbtyp"].Value, "Fremdkabine", StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }
        var zugang = string.Equals(name[^1..], "B") || string.Equals(name[^1..], "C") || string.Equals(name[^1..], "D") ? name[^1..] : "A";
        string carDoorName = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, $"var_CarDoorDescription{zugang}");

        if (string.IsNullOrWhiteSpace(carDoorName))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
        }
        else
        {
            var carDoor = _parametercontext.Set<CarDoor>().FirstOrDefault(x => x.Name == carDoorName);
            if (carDoor is null)
            {
                return;
            }
            double minMountingSpace = carDoor.MinimalMountingSpace;
            double minMountingSpaceReduced = 0d;
            if (_parameterDictionary[$"var_KabTuerKaempferBreite{zugang}"].Value == "97")
            {
                minMountingSpaceReduced = carDoor.ReducedMinimalMountingSpace;
            }
            string doorMounting = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, zugang == "A" ? "var_TuerEinbau" : $"var_TuerEinbau{zugang}");

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
        var lpsSchindlerCertified = _parameterDictionary[name].DropDownListValue?.Id == 2;
        var msz9e = _parameterDictionary["var_Steuerungstyp"].DropDownList.FirstOrDefault(x => x.Name == "Kühn MSZ 9E");
        msz9e?.SchindlerCertified = lpsSchindlerCertified;
        var msz10 = _parameterDictionary["var_Steuerungstyp"].DropDownList.FirstOrDefault(x => x.Name == "Kühn MSZ 10");
        msz10?.SchindlerCertified = lpsSchindlerCertified;
    }

    private void ValidateUCMDetectingAndTriggeringComponents(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        List<SelectionValue> availableDetectingAndTriggeringComponents = [];

        var overSpeedGovernor = _parameterDictionary["var_Geschwindigkeitsbegrenzer"].DropDownListValue;
        var liftPositionSystem = _parameterDictionary["var_Schachtinformationssystem"].DropDownListValue;
        var liftControler = _parameterDictionary["var_Steuerungstyp"].DropDownListValue;

        if (overSpeedGovernor is not null)
        {
            var overSpeedGovernorDb = _parametercontext.Set<OverspeedGovernor>().FirstOrDefault(x => x.Id == overSpeedGovernor.Id);
            if (overSpeedGovernorDb is not null && overSpeedGovernorDb.HasUCMPCertification)
            {
                overSpeedGovernor.Id = 1;
                availableDetectingAndTriggeringComponents.Add(overSpeedGovernor);
            }
        }
        if (liftPositionSystem is not null)
        {
            var liftPositionSystemDb = _parametercontext.Set<LiftPositionSystem>().FirstOrDefault(x => x.Id == liftPositionSystem.Id);
            if (liftPositionSystemDb is not null && liftPositionSystemDb.TypeExaminationCertificateId != 1)
            {
                liftPositionSystem.Id = 2;
                availableDetectingAndTriggeringComponents.Add(liftPositionSystem);
            }
        }
        if (liftControler is not null)
        {
            liftControler.Id = 3;
            availableDetectingAndTriggeringComponents.Add(liftControler);
        }

        UpdateDropDownList("var_UCMP_DetektierendesElement", availableDetectingAndTriggeringComponents);
        UpdateDropDownList("var_UCMP_AusloesendesElement", availableDetectingAndTriggeringComponents);
        if (availableDetectingAndTriggeringComponents.Count == 1)
        {
            _parameterDictionary["var_UCMP_DetektierendesElement"].AutoUpdateParameterValue(availableDetectingAndTriggeringComponents[0].Name);
            _parameterDictionary["var_UCMP_AusloesendesElement"].AutoUpdateParameterValue(availableDetectingAndTriggeringComponents[0].Name);
        }
    }

    private void ValidateUCMBrakingComponents(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        var isRopedrive = string.IsNullOrWhiteSpace(_parameterDictionary["var_Getriebe"].Value) || _parameterDictionary["var_Getriebe"].Value != "hydraulisch";
        if (isRopedrive && string.Equals(name, "var_Antrieb"))
        {
            var availableBrakingComponents = _parametercontext.Set<DriveSafetyBrake>().Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName)
            {
                IsFavorite = x.IsFavorite,
                SchindlerCertified = x.SchindlerCertified,
                OrderSelection = x.OrderSelection
            });
            UpdateDropDownList("var_UCMP_BremsendesElement", availableBrakingComponents);
            if (string.IsNullOrWhiteSpace(value))
            {
                _parameterDictionary["var_UCMP_BremsendesElement"].AutoUpdateParameterValue(string.Empty);
                return;
            }
            var drive = _parametercontext.Set<ZiehlAbeggDrive>().Include(i => i.DriveSafetyBrake).FirstOrDefault(x => x.Name == value.Trim());
            if (drive is null)
            {
                _parameterDictionary["var_UCMP_BremsendesElement"].AutoUpdateParameterValue(string.Empty);
                return;
            }
            _parameterDictionary["var_UCMP_BremsendesElement"].AutoUpdateParameterValue(drive.DriveSafetyBrake?.Name);
        }
        if (!isRopedrive && string.Equals(name, "var_Hydraulikventil"))
        {
            var availableBrakingComponents = _parametercontext.Set<HydraulicValve>().Select(x => new SelectionValue(x.Id, x.Name, x.DisplayName)
            {
                IsFavorite = x.IsFavorite,
                SchindlerCertified = x.SchindlerCertified,
                OrderSelection = x.OrderSelection
            });
            UpdateDropDownList("var_UCMP_BremsendesElement", availableBrakingComponents);
            _parameterDictionary["var_UCMP_BremsendesElement"].AutoUpdateParameterValue(value?.Trim());
        }
    }

    private void ValidateOverAndUnderTravels(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (!_calculationsModuleService.IsRopeLift(_parameterDictionary["var_Bausatz"].DropDownListValue))
        {
            return;
        }
        switch (name)
        {
            case "var_FUBP" or "var_Puffertyp":
                double freeUnderTravel = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_FUBP");
                var bufferstokeCar = _calculationsModuleService.GetmaxBufferStoke(_parameterDictionary["var_Puffertyp"].Value);
                _parameterDictionary["var_RHU"].AutoUpdateParameterValue((freeUnderTravel + bufferstokeCar).ToString());
                break;
            case "var_FUEBP" or "var_Puffertyp_GGW":
                double freeOverTravel = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_FUEBP");
                var bufferstokeCWT = _calculationsModuleService.GetmaxBufferStoke(_parameterDictionary["var_Puffertyp"].Value);
                _parameterDictionary["var_RHO"].AutoUpdateParameterValue((freeOverTravel + bufferstokeCWT).ToString());
                break;
            default:
                return;
        }
    }

    private void ValidateLiftBuffers(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
            return;
        }
        double liftspeed = LiftParameterHelper.GetLiftParameterValue<double>(_parameterDictionary, "var_v");
        double bufferLoad = 0.0;
        int bufferCount = 0;
        string bufferTyp = string.Empty;

        switch (name)
        {
            case "var_Puffertyp" or "var_Anzahl_Puffer_FK":
                bufferTyp = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Puffertyp");
                bufferCount = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, "var_Anzahl_Puffer_FK");
                bufferLoad = _calculationsModuleService.GetCurrentBufferForce(_parameterDictionary, "var_Puffertyp");
                break;
            case "var_Puffertyp_GGW" or "var_Anzahl_Puffer_GGW":
                bufferTyp = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Puffertyp_GGW");
                bufferCount = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, "var_Anzahl_Puffer_GGW");
                bufferLoad = _calculationsModuleService.GetCurrentBufferForce(_parameterDictionary, "var_Puffertyp_GGW");
                break;
            case "var_Puffertyp_EM_SG" or "var_Anzahl_Puffer_EM_SG":
                bufferTyp = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Puffertyp_EM_SG");
                bufferCount = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, "var_Anzahl_Puffer_EM_SG");
                bufferLoad = _calculationsModuleService.GetCurrentBufferForce(_parameterDictionary, "var_Puffertyp_EM_SG");
                break;
            case "var_Puffertyp_EM_SK" or "var_Anzahl_Puffer_EM_SK" or "var_ErsatzmassnahmenSK_unter_GGW":
                liftspeed = LiftParameterHelper.GetLiftParameterValue<bool>(_parameterDictionary, "var_ErsatzmassnahmenSK_Inspektionsgeschwindigkeit") ? 0.6 : liftspeed;
                bufferTyp = LiftParameterHelper.GetLiftParameterValue<string>(_parameterDictionary, "var_Puffertyp_EM_SK");
                bufferCount = LiftParameterHelper.GetLiftParameterValue<int>(_parameterDictionary, "var_Anzahl_Puffer_EM_SK");
                bufferLoad = _calculationsModuleService.GetCurrentBufferForce(_parameterDictionary, "var_Puffertyp_EM_SK");
                break;
            case "var_Q" or "var_F" or "var_v":
                ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = ["var_Puffertyp", "var_Puffertyp_GGW", "var_Puffertyp_EM_SG", "var_Puffertyp_EM_SK"] });
                return;
            default:
                break;
        }

        string[] dependentParameter = [];
        if (optionalCondition is not null)
        {
            dependentParameter = [optionalCondition];
        }
            
        if (string.IsNullOrWhiteSpace(bufferTyp) || _calculationsModuleService.ValidateBufferRange(bufferTyp, liftspeed, bufferLoad))
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true) { DependentParameter = dependentParameter });
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{displayname}: {bufferCount}x {bufferTyp} nicht zulässig! (Last: {bufferLoad} kg/Puffer Betriebsgeschwindigkeit: {liftspeed} m/s)", SetSeverity(severity)) 
            { DependentParameter = dependentParameter });
        }      
    }

    private void ValidateHasOilbuffer(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        if (!string.Equals(name, "var_Puffertyp"))
        {
            return;
        }
        bool hasOilBuffer = !string.IsNullOrWhiteSpace(value) && value.StartsWith("LSB");
        _parameterDictionary["var_HasOilbuffer"].AutoUpdateParameterValue(LiftParameterHelper.FirstCharToUpperAsSpan(hasOilBuffer.ToString()));
    }

    private void ValidateMotorP3(string name, string displayname, string? value, string? severity, string? optionalCondition = null)
    {
        bool driveIsValidate = false;

        if (!string.IsNullOrWhiteSpace(value))
        {
            var drive = _parametercontext.Set<ZiehlAbeggDrive>().FirstOrDefault(x => x.Name.StartsWith(value));
            driveIsValidate = drive is null || drive.SchindlerCertified;
        }
        else
        {
            driveIsValidate = true;
        }

        if (!driveIsValidate)
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, $"{displayname}: {value} ist nicht auf der Schindler Greenlist, P3 Antrag erforderlich!", SetSeverity(severity)));
        }
        else
        {
            ValidationResult.Add(new ParameterStateInfo(name, displayname, true));
        }
    }
}