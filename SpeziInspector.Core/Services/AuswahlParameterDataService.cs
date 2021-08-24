using SpeziInspector.Core.Contracts.Services;
using SpeziInspector.Core.Helpers;
using SpeziInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace SpeziInspector.Core.Services
{
    public class AuswahlParameterDataService : IAuswahlParameterDataService
    {
        public Dictionary<string, AuswahlParameter> AuswahlParameterDictionary { get; set; } = new();

        public string AuswahlParameterDataPath { get; set; }

        public AuswahlParameterDataService()
        {
            AuswahlParameterDataPath = @"C:\Work\Administration\Spezifikation\Auswahlparameter.json";
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

        public void UpdateAuswahlparameter()
        {
            string excelFilePath = @"C:\Work\Administration\Spezifikation\Spezifikation.xlsm";
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
                { "Daten Bausatz", "Typ","var_TypFuehrung" },
                { "Daten Bausatz", "Ersatzmaßnahmen","var_Ersatzmassnahmen" },
                { "Daten Bausatz", "Führungsschienen","var_FuehrungsschieneFahrkorb" },
                { "Daten Bausatz", "Führungsschienen GGW / Joch","var_FuehrungsschieneGegengewicht" },
                { "Daten Bausatz", "Status Schienen","var_StatusFuehrungsschienen" },
                { "Daten Bausatz", "Status Schienen","var_var_StatusGGWSchienen" },
                { "Daten Bausatz", "Lastmessvorrichtung","var_Lastmesseinrichtung" },
                { "Daten Bausatz", "Beschichtung","var_Beschichtung" },
                { "Daten Bausatz", "Fangvorrichtung","var_TypFV" },
                { "Daten Bausatz", "Fangvorrichtungstyp Bremsfang","var_Fangvorrichtung" },
                { "Daten Bausatz", "Fangvorrichtungstyp Sperrfang","var_Fangvorrichtung" },
                { "Daten Bausatz", "Schachtinformation","var_Schachtinformation" },
                { "Daten Bausatz", "Geschwindigkeitsbegrenzer","var_Geschwindigkeitsbegrenzer" },
                { "Daten Normen", "Normen","var_Normen" },
                { "Daten Schacht", "Schacht / Maschinenraum","var_var_Schachtinformationen" },
                { "Daten Schacht", "Schachttyp","var_SchachtInformationen" },
                { "Daten Schacht", "Maschinenraumposition","var_Maschinenraum" },
                { "Daten Schacht", "Brandabschluß","var_Brandabschlussinfo" },
                { "Daten Schacht", "Schachtentrauchung","var_Schachtentrauchung" },
                { "Daten Schacht", "Schienenbügelbefestigung","var_Befestigung" },
                { "Daten Kabine", "Kabinentyp","var_xx" },
                { "Daten Kabine", "Antidröhn","var_Antidroehn" },
                { "Daten Kabine", "Decke","var_Decke" },
                { "Daten Kabine", "Beleuchtung","var_Beleuchtung" },
                { "Daten Kabine", "Material","var_Material" },
                { "Daten Kabine", "Materialstärke","var_Materialstaerke" },
                { "Daten Kabine", "Bodentyp","var_Bodentyp" },
                { "Daten Kabine", "Bodenblech","var_Bodenblech" },
                { "Daten Kabine", "Sonderbleche","var_Sonderbleche" },
                { "Daten Kabine", "Bodenprofile","var_xx" },
                { "Daten Kabine", "Bodenbelag","var_Bodenbelag" },
                { "Daten Kabine", "Paneele","var_Paneelmaterial" },
                { "Daten Kabine", "Spiegel","var_Spiegel" },
                { "Daten Kabine", "Handlauf","var_Handlauf" },
                { "Daten Kabine", "Sockelleiste","var_Sockelleiste" },
                { "Daten Kabine", "Rammschutz","var_Rammschutz" },
                { "Daten Türen", "Türtyp","var_Tuertyp" },
                { "Daten Türen", "Oberflächen Türe/Tableau","var_Tueroberflaeche" },
                { "Daten Türen", "Türöffnung","var_Tueroeffnung" },
                { "Daten Türen", "Türsteuerung","var_Tuersteuerung" },
                { "Daten Türen", "Türsteuerung mit V3F-Antrieb","var_xx" },
                { "Daten Türen", "Türüberwachung","var_Lichtgitter" },
                { "Daten Türen", "Schwellenprofile","var_Schwellenprofil" },
                { "Daten Türen", "Türzulassung","var_ZulassungTuere" },
                { "Daten Tableau", "Tableau","var_KabTabKabinentableau" },
                { "Daten Tableau", "Aufbau","var_KabTabAufbau" },
                { "Daten Tableau", "Anzeigen","var_xx" },
                { "Daten Tableau", "Farbe Punktmatrix bzw. TFT","var_xx" },
                { "Daten Tableau", "Taster","var_xx" },
                { "Daten Tableau", "Tasterplatten","var_xx" },
                { "Daten Tableau", "Farbe Quittung LED Taster","var_xx" },
                { "Daten Tableau", "AT Befestigung","var_xx" },
                { "Daten Tableau", "Leuchtfeld Größe","var_xx" },
                { "Daten Tableau", "Anbauort WFA","var_WfaAnbauortWFA" },
                { "Daten Tableau", "Gongposition","var_WfaGongposition" },
                { "Daten Tableau", "Material Tasterplatten","var_xx" },
                { "Daten Tableau", "Material","var_xx" },
                { "Daten Steuerung und Antrieb", "Steuerung","var_Steuerungstyp" },
                { "Daten Steuerung und Antrieb", "Lage Schaltschrank","var_LageSchaltschrank" },
                { "Daten Steuerung und Antrieb", "Stromanschluss","var_Stromanschluss" },
                { "Daten Steuerung und Antrieb", "Aggregat","var_Aggregat" },
                { "Daten Steuerung und Antrieb", "Getriebe","var_Getriebe" },
                { "Daten Steuerung und Antrieb", "Notruf","var_Notruf" },
                { "Daten Steuerung und Antrieb", "Lieferoption Notruftaster","var_xx" },
                { "Daten Steuerung und Antrieb", "Notrufleitung","var_xx" },
                { "Daten Steuerung und Antrieb", "Notruftyp","var_Notruftyp" },
                { "Daten Steuerung und Antrieb", "AT / Steuerungsart","var_xx" },
                { "Daten Steuerung und Antrieb", "Schaltschrankgrößen","var_xx" }
            };

            for (int i = 0; i < (importAusawahlParameter.Length) / 3; i++)
            {

                var parList = ExcelHelper.ReadExcelParameterListe(excelFilePath, importAusawahlParameter[i, 0], importAusawahlParameter[i, 1]);
                AuswahlParameter _auswahlParameter = new();
                _auswahlParameter.Name = importAusawahlParameter[i, 2];

                foreach (string par in parList)
                {
                    _auswahlParameter.Auswahlliste.Add(par);
                }
                _data.Add(_auswahlParameter);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            string json = JsonSerializer.Serialize(_data, options);
            File.WriteAllText(AuswahlParameterDataPath, json);
        }

        private void FillParameterDictionary()
        {
            var jsonString = File.ReadAllText(AuswahlParameterDataPath);
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
