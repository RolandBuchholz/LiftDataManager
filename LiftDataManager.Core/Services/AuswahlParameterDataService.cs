using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Helpers;
using LiftDataManager.Core.Models;

namespace LiftDataManager.Core.Services
{
    public class AuswahlParameterDataService : IAuswahlParameterDataService
    {
        public Dictionary<string, AuswahlParameter> AuswahlParameterDictionary { get; set; } = new();
        private readonly string AuswahlParameterDataPath = @"C:\Work\Administration\Spezifikation\Auswahlparameter.json";
        private readonly string excelFilePath = @"C:\Work\Administration\Spezifikation\Spezifikation.xlsm";


        public AuswahlParameterDataService()
        {
            FillParameterDictionary();
        }

        public List<string> GetListeAuswahlparameter(string name)
        {
            var AuswahlParameterListe = AuswahlParameterDictionary[name].Auswahlliste;
            return AuswahlParameterListe;
        }

        public bool ParameterHasAuswahlliste(string name)
        {
            if (AuswahlParameterDictionary.ContainsKey(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> UpdateAuswahlparameterAsync()
        {
            if (!File.Exists(excelFilePath))
            {
                throw new ArgumentException("Spezifikation nicht vorhanden ", nameof(excelFilePath));
            }

            FileInfo SpezifikationInfo = new FileInfo(excelFilePath);

            if (SpezifikationInfo.IsReadOnly)
            {
                SpezifikationInfo.IsReadOnly = false;
            }

            List<AuswahlParameter> _data = new List<AuswahlParameter>();

            string[,] importAusawahlParameter =
            {
                { "Daten Allgemein", "Lieferart","var_Lieferart" },
                { "Daten Allgemein", "Information","var_InformationAufzug" },
                { "Daten Allgemein", "Gebäudetyp","var_Gebaeudetyp" },
                { "Daten Allgemein", "Beladegerät","var_Beladegeraet" },
                { "Daten Allgemein", "CE-Nummer","var_CeNummer" },
                { "Daten Allgemein", "Aufzugstyp","var_Aufzugstyp" },
                { "Daten Bausatz", "Bausatz","var_Bausatz" },
                { "Daten Bausatz", "Führung","var_Fuehrungsart" },
                { "Daten Bausatz", "Führungstyp","var_TypFuehrung" },
                { "Daten Bausatz", "Führung","var_Fuehrungsart_GGW" },
                { "Daten Bausatz", "Führungstyp","var_TypFuehrung_GGW" },
                { "Daten Bausatz", "Ersatzmaßnahmen","var_Ersatzmassnahmen" },
                { "Daten Bausatz", "Führungsschienen","var_FuehrungsschieneFahrkorb" },
                { "Daten Bausatz", "Führungsschienen GGW / Joch","var_FuehrungsschieneGegengewicht" },
                { "Daten Bausatz", "Status Schienen","var_StatusFuehrungsschienen" },
                { "Daten Bausatz", "Status Schienen","var_StatusGGWSchienen" },
                { "Daten Bausatz", "Lastmessvorrichtung","var_Lastmesseinrichtung" },
                { "Daten Bausatz", "Beschichtung","var_Beschichtung" },
                { "Daten Bausatz", "Fangvorrichtung","var_Fangvorrichtung" },
                { "Daten Bausatz", "Fangvorrichtungstyp Bremsfang","var_TypFV" },
                { "Daten Bausatz", "Fangvorrichtungstyp Sperrfang","var_TypFV" },
                { "Daten Bausatz", "Schachtinformation","var_Schachtinformationssystem" },
                { "Daten Bausatz", "Geschwindigkeitsbegrenzer","var_Geschwindigkeitsbegrenzer" },
                { "Daten Normen", "Normen","var_Normen" },
                { "Daten Normen", "EN81-31 / 41","var_EN8131" },
                { "Daten Normen", "EN81-71","var_EN8171Cat012" },
                { "Daten Schacht", "Schacht / Maschinenraum","var_Schacht" },
                { "Daten Schacht", "Schachttyp","var_SchachtInformation" },
                { "Daten Schacht", "Maschinenraumposition","var_Maschinenraum" },
                { "Daten Schacht", "Brandabschluß ","var_BsMit" },
                { "Daten Schacht", "Ausgeführt durch","var_BsLiefer"},
                { "Daten Schacht", "Schachtentrauchung","var_Schachtentrauchung" },
                { "Daten Schacht", "Schienenbügelbefestigung","var_Befestigung" },
                { "Daten Schacht", "Schachtgerüst Feldfüllung","var_GeruestFeldfuellung"},
                { "Daten Kabine", "Kabinentyp","var_Fahrkorbtyp" },
                { "Daten Kabine", "Antidröhn","var_Antidroehn" },
                { "Daten Kabine", "Decke","var_Decke" },
                { "Daten Kabine", "Beleuchtung","var_Beleuchtung" },
                { "Daten Kabine", "Material","var_Seitenwaende" },
                { "Daten Kabine", "Material","var_Rueckwand" },
                { "Daten Kabine", "Material","var_Eingangswand" },
                { "Daten Kabine", "Materialstärke","var_Materialstaerke" },
                { "Daten Kabine", "Bodentyp","var_Bodentyp" },
                { "Daten Kabine", "Bodenblech","var_Bodenblech" },
                { "Daten Kabine", "Sonderbleche","var_Sonderbleche" },
                { "Daten Kabine", "Sonderbleche","var_BelagAufDemKabinendach" },
                { "Daten Kabine", "Bodenprofile","var_BoPr" },
                { "Daten Kabine", "Bodenbelag","var_Bodenbelag" },
                { "Daten Kabine", "Paneele","var_Paneelmaterial" },
                { "Daten Kabine", "Spiegel","var_Spiegel" },
                { "Daten Kabine", "Handlauf","var_Handlauf" },
                { "Daten Kabine", "Sockelleiste","var_Sockelleiste" },
                { "Daten Kabine", "Rammschutz","var_Rammschutz" },
                { "Daten Türen", "Türtyp","var_Tuertyp" },
                { "Daten Türen", "Meiller","var_Tuerbezeichnung" },
                { "Daten Türen", "Oberflächen Türe/Tableau","var_Tueroberflaeche" },
                { "Daten Türen", "Oberflächen Türe/Tableau","var_OberflaecheKabinentuere" },
                //{ "Daten Türen", "Oberflächen Türe/Tableau","var_Mauerumfassungszargen" },
                //{ "Daten Türen", "Oberflächen Türe/Tableau","var_Portale" },
                { "Daten Türen", "Türöffnung","var_Tueroeffnung" },
                { "Daten Türen", "Türsteuerung","var_Tuersteuerung" },
                { "Daten Türen", "Türsteuerung mit V3F-Antrieb","var_Frequenzumrichter" },
                { "Daten Türen", "Türüberwachung","var_Lichtgitter" },
                { "Daten Türen", "Schwellenprofile","var_Schwellenprofil" },
                { "Daten Türen", "Türzulassung","var_ZulassungTuere" },
                { "Daten Türen", "Türverriegelung","var_Tuerverriegelung" },
                { "Daten Tableau", "Tableau","var_KabTabKabinentableau" },
                { "Daten Tableau", "Aufbau","var_KabTabAufbau" },
                { "Daten Tableau", "Anzeigen","var_KabTabPunktmatrixTyp" },
                { "Daten Tableau", "Anzeigen","var_AussTabPunktmatrixTyp" },
                { "Daten Tableau", "Anzeigen","var_WfaPunktmatrixTyp" },
                { "Daten Tableau", "Farbe Punktmatrix bzw. TFT","var_KabTabFarbe" },
                { "Daten Tableau", "Farbe Punktmatrix bzw. TFT","var_AussTabFarbe" },
                { "Daten Tableau", "Farbe Punktmatrix bzw. TFT","var_WfaFarbe" },
                { "Daten Tableau", "Taster","var_KabTabTaster" },
                { "Daten Tableau", "Taster","var_AussTabTaster" },
                { "Daten Tableau", "Tasterplatten","var_KabTabTasterplatten" },
                { "Daten Tableau", "Tasterplatten","var_AussTabTasterplatten" },
                { "Daten Tableau", "Farbe Quittung LED Taster","var_ColLedFT" },
                { "Daten Tableau", "Farbe Quittung LED Taster","var_ColLedAT" },
                { "Daten Tableau", "AT Befestigung","var_AussTabATBefestigung" },
                { "Daten Tableau", "AT Befestigung","var_WfaWFABefestigung" },
                { "Daten Tableau", "Leuchtfeld Größe","var_AussTabLeuchtfeldGroee" },
                { "Daten Tableau", "Leuchtfeld Größe","var_WfaLeuchtfeldGroee" },
                { "Daten Tableau", "Anbauort WFA","var_WfaAnbauortWFA" },
                { "Daten Tableau", "Gongposition","var_WfaGongposition" },
                { "Daten Tableau", "Material Tasterplatten","var_KabTabTasterplattenmaterial" },
                { "Daten Tableau", "Material Tasterplatten","var_AussTabTasterplattenmaterial" },
                { "Daten Tableau", "Material","var_KabTabMaterial" },
                { "Daten Tableau", "Material","var_AussTabMaterial" },
                { "Daten Tableau", "Material","var_Material" },
                { "Daten Steuerung und Antrieb", "Steuerung","var_Steuerungstyp" },
                { "Daten Steuerung und Antrieb", "Lage Schaltschrank","var_LageSchaltschrank" },
                { "Daten Steuerung und Antrieb", "Schaltschrankausführung","var_Schaltschrankoberflaeche" },
                { "Daten Steuerung und Antrieb", "Stromanschluss","var_Stromanschluss" },
                { "Daten Steuerung und Antrieb", "Aggregat","var_Aggregat" },
                { "Daten Steuerung und Antrieb", "Getriebe","var_Getriebe" },
                { "Daten Steuerung und Antrieb", "Notruf","var_Notruf" },
                { "Daten Steuerung und Antrieb", "Lieferoption Notruftaster","var_NotruftasterKabineDachSG" },
                { "Daten Steuerung und Antrieb", "Notrufleitung","var_NotrufNetz" },
                { "Daten Steuerung und Antrieb", "Notruftyp","var_Notruftyp" },
                { "Daten Steuerung und Antrieb", "AT / Steuerungsart","var_XKnopfsteuerung" },
                { "Daten Steuerung und Antrieb", "Schaltschrankgrößen","var_Schaltschrankgroesse" }
            };

            Task<List<AuswahlParameter>> parameterInfo = ExcelHelper.ReadExcelParameterListeAsync(excelFilePath, importAusawahlParameter);

            _data = parameterInfo.Result;

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.WriteIndented = true;
            jsonOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            string json = JsonSerializer.Serialize<List<AuswahlParameter>>(_data, jsonOptions);
            await File.WriteAllTextAsync(AuswahlParameterDataPath, json);

            if (!SpezifikationInfo.IsReadOnly)
            {
                SpezifikationInfo.IsReadOnly = true;
            }

            return json;
        }

        private void FillParameterDictionary()
        {
            string jsonString;

            if (!File.Exists(AuswahlParameterDataPath))
            {
                jsonString = Task.Run(() => UpdateAuswahlparameterAsync()).Result;
            }
            else
            {
                FileInfo AuswahlparameterInfo = new(AuswahlParameterDataPath);

                if (AuswahlparameterInfo.IsReadOnly)
                {
                    AuswahlparameterInfo.IsReadOnly = false;
                }
                jsonString = File.ReadAllText(AuswahlParameterDataPath);
            }

            if (!String.IsNullOrWhiteSpace(jsonString))
            {
                try
                {
                    var AuswahlParameterList = JsonSerializer.Deserialize<List<AuswahlParameter>>(jsonString);
                    foreach (AuswahlParameter Auswahlpar in AuswahlParameterList)
                    {
                        if (!AuswahlParameterDictionary.ContainsKey(Auswahlpar.Name))
                        {
                            AuswahlParameterDictionary.Add(Auswahlpar.Name, Auswahlpar);
                        }
                        else
                        {
                            AuswahlParameterDictionary[Auswahlpar.Name].Auswahlliste.AddRange(Auswahlpar.Auswahlliste);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}