using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Models.PdfDocuments;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PDFTests.Services.DocumentGeneration;

public class KabinengewichtDocument : PdfBaseDocument
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly bool LowPrintColor;
    public CarWeightResult CarWeightResult = new();

    public KabinengewichtDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService, bool lowPrintColor)
    {
        _calculationsModuleService = calculationsModuleService;
        ParameterDictionary = parameterDictionary;
        CarWeightResult = _calculationsModuleService.GetCarWeightCalculation(parameterDictionary);
        Title = "Kabinengewicht";
        LowPrintColor = lowPrintColor;
        SetPdfStyle(LowPrintColor, false);
    }

    protected override void Content(IContainer container)
    {
        container.PaddingLeft(10, Unit.Millimetre).Column(column =>
        {
            column.Item().PaddingTop(5, Unit.Millimetre).Text("Daten der Aufzugkabine C100").FontSize(fontSizeXL).Bold();
            column.Item().Row(row =>
            {
                row.AutoItem().MinHeight(300).Element(TableCarData);
                row.RelativeItem().PaddingVertical(5, Unit.Millimetre).PaddingHorizontal(15, Unit.Millimetre).Component(new CarDesignComponent(ParameterDictionary, LowPrintColor));
            });
            column.Item().PaddingLeft(10).Element(TableCarEquipment);
            column.Item().PageBreak();
            column.Item().PaddingTop(10, Unit.Millimetre).PaddingLeft(10).Element(TableCarInterior);
            column.Item().PaddingLeft(10).Element(TableCarAccesories);
            column.Item().PaddingLeft(10).Element(TableCarweights);
        });
    }

    void TableCarData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.ConstantColumn(25, Unit.Millimetre);
            });
            table.Cell().Row(1).Column(1).PaddingLeft(10).Text("Fahrkorbbreite (KB)");
            table.Cell().Row(1).Column(2).AlignRight().Text($"{ParameterDictionary["var_KBI"].Value} mm");
            table.Cell().Row(2).Column(1).PaddingLeft(10).Text("Fahrkorbtiefe (KT)");
            table.Cell().Row(2).Column(2).AlignRight().Text($"{ParameterDictionary["var_KTI"].Value} mm");
            table.Cell().Row(3).Column(1).PaddingLeft(10).Text("Fahrkorbhöhe (KH)");
            table.Cell().Row(3).Column(2).AlignRight().Text($"{ParameterDictionary["var_KHLicht"].Value} mm");
            table.Cell().Row(4).Column(1).PaddingLeft(10).Text("Anzahl Kabinentüren");
            table.Cell().Row(4).Column(2).AlignRight().Text($"{CarWeightResult.AnzahlKabinentueren} Stk");
            table.Cell().Row(5).Column(1).ColumnSpan(2).ShowIf(CarWeightResult.ZugangA).PaddingLeft(10).Column(column =>
            {
                column.Item().Text("Zugang A").Bold();
                column.Item().Text($"{ParameterDictionary["var_Tuertyp"].Value}{ParameterDictionary["var_Tuerbezeichnung"].Value}");
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Türbreite (TB)");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_TB"].Value} mm");
                });
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Hals L1");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_L1"].Value} mm");
                });
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Hals R1");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_R1"].Value} mm");
                });
            });
            table.Cell().Row(6).Column(1).ColumnSpan(2).ShowIf(CarWeightResult.ZugangB).PaddingLeft(10).Column(column =>
            {
                column.Item().Text("Zugang B").Bold();
                column.Item().Text($"{ParameterDictionary["var_Tuertyp_B"].Value}{ParameterDictionary["var_Tuerbezeichnung_B"].Value}");
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Türbreite (TB)");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_TB_B"].Value} mm");
                });
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Hals L3");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_L3"].Value} mm");
                });
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Hals R3");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_R3"].Value} mm");
                });
            });
            table.Cell().Row(7).Column(1).ColumnSpan(2).ShowIf(CarWeightResult.ZugangC).PaddingLeft(10).Column(column =>
            {
                column.Item().Text("Zugang C").Bold();
                column.Item().Text($"{ParameterDictionary["var_Tuertyp_C"].Value}{ParameterDictionary["var_Tuerbezeichnung_C"].Value}");
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Türbreite (TB)");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_TB_C"].Value} mm");
                });
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Hals L2");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_L2"].Value} mm");
                });
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Hals R2");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_R2"].Value} mm");
                });
            });
            table.Cell().Row(8).Column(1).ColumnSpan(2).ShowIf(CarWeightResult.ZugangD).PaddingLeft(10).Column(column =>
            {
                column.Item().Text("Zugang D").Bold();
                column.Item().Text($"{ParameterDictionary["var_Tuertyp_D"].Value}{ParameterDictionary["var_Tuerbezeichnung_D"].Value}");
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Türbreite (TB)");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_TB_D"].Value} mm");
                });
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Hals L4");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_L4"].Value} mm");
                });
                column.Item().Row(row =>
                {
                    row.AutoItem().Text("Hals R4");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_R4"].Value} mm");
                });
            });
            table.Cell().Row(9).Column(1).PaddingLeft(10).Text($"Bodenausführung ({ParameterDictionary["var_Bodentyp"].Value})");
            table.Cell().Row(9).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.KabinenBodengewicht)} kg");
            table.Cell().Row(10).Column(1).PaddingLeft(10).Text("Bodenblechstärke");
            table.Cell().Row(10).Column(2).AlignRight().Text($"{CarWeightResult.Bodenblech} mm");
            table.Cell().Row(11).Column(1).PaddingLeft(10).Text("Bodenprofile");
            table.Cell().Row(11).Column(2).PaddingLeft(10).Text($"{ParameterDictionary["var_BoPr"].Value} ({CarWeightResult.BodenProfilGewicht} kg/m)");
        });
    }

    void TableCarEquipment(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.5f);
                columns.RelativeColumn(0.5f);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });
            table.Cell().Row(1).Column(1).ColumnSpan(4).Text("Kabinenausführung").FontSize(fontSizeXL).Bold();
            table.Cell().Row(2).Column(1).Text($"Bodenbelag ({ParameterDictionary["var_Bodenbelag"].Value})");
            table.Cell().Row(2).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.BodenBelagGewicht)} kg");
            table.Cell().Row(2).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.BodenBelagGewichtproQm, 2)} kg/m²");
            table.Cell().Row(3).Column(1).Text($"Schotten ({ParameterDictionary["var_Materialstaerke"].Value} mm)");
            table.Cell().Row(3).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.Schottengewicht)} kg");
            table.Cell().Row(4).Column(1).Text($"Hälse ({ParameterDictionary["var_Materialstaerke"].Value} mm)");
            table.Cell().Row(4).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.Haelsegewicht)} kg");
            table.Cell().Row(5).Column(1).Text("Antidröhn");
            table.Cell().Row(5).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.AndidroehnGewicht)} kg");
            table.Cell().Row(6).Column(1).Text(CarWeightResult.SchuerzeVerstaerkt ? "Schürze (verstärkt)" : "Schürze (Standard)");
            table.Cell().Row(6).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.SchuerzeGewicht)} kg");
            table.Cell().Row(7).Column(1).Text("Decke");
            table.Cell().Row(7).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.Deckegewicht)} kg");
            table.Cell().Row(8).Column(1).Text("abgehängte Decke");
            table.Cell().Row(8).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.GewichtAbgehaengteDecke)} kg");
            table.Cell().Row(8).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.AbgehaengteDeckeGewichtproQm, 2)} kg/m²");
            table.Cell().Row(9).Column(1).Text("Decke Sichtseite belegt");
            table.Cell().Row(9).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.DeckeBelegtGewicht)} kg");
            table.Cell().Row(9).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.DeckeBelegtGewichtproQm, 2)} kg/m²");
            table.Cell().Row(10).Column(1).Text("Belag auf dem Kabinendach");
            table.Cell().Row(10).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.BelagAufDerDeckeGewicht)} kg");
            table.Cell().Row(10).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.BelagAufDerDeckeGewichtproQm, 2)} kg/m²");
        });
    }

    void TableCarInterior(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.5f);
                columns.RelativeColumn(0.5f);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });
            table.Cell().Row(1).Column(1).ColumnSpan(4).Text("Kabineninnenausstattung").FontSize(fontSizeXL).Bold();
            table.Cell().Row(2).Column(1).Text("Spiegel (Standard)");
            table.Cell().Row(2).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.SpiegelGewicht)} kg");
            table.Cell().Row(2).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.SpiegelGewichtproQm, 2)} kg/m²");
            table.Cell().Row(2).Column(4).PaddingLeft(30).Text($"{Math.Round(CarWeightResult.SpiegelQm, 2)} m²");
            table.Cell().Row(3).Column(1).Text("Paneele");
            table.Cell().Row(3).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.PaneeleGewicht)} kg");
            table.Cell().Row(3).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.PaneeleGewichtproQm, 2)} kg/m²");
            table.Cell().Row(3).Column(4).PaddingLeft(30).Text($"{Math.Round(CarWeightResult.PaneeleQm, 2)} m²");
            table.Cell().Row(4).Column(1).Text("Paneele (Spiegel)");
            table.Cell().Row(4).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.PaneeleSpiegelGewicht)} kg");
            table.Cell().Row(4).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.PaneeleSpiegelGewichtproQm, 2)} kg/m²");
            table.Cell().Row(4).Column(4).PaddingLeft(30).Text($"{Math.Round(CarWeightResult.PaneeleSpiegelQm, 2)} m²");
            table.Cell().Row(5).Column(1).Text($"Glaswände {CarWeightResult.VSGTyp}");
            table.Cell().Row(5).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.VSGGewicht)} kg");
            table.Cell().Row(5).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.VSGGewichtproQm, 2)} kg/m²");
            table.Cell().Row(5).Column(4).PaddingLeft(30).Text($"{Math.Round(CarWeightResult.VSGQm, 2)} m²");
            table.Cell().Row(6).Column(1).Text("Außenverkleidung");
            table.Cell().Row(6).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.AussenVerkleidungGewicht)} kg");
            table.Cell().Row(6).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.AussenVerkleidungGewichtproQm, 2)} kg/m²");
            table.Cell().Row(6).Column(4).PaddingLeft(30).Text($"{Math.Round(CarWeightResult.AussenVerkleidungQm, 2)} m²");
            table.Cell().Row(7).Column(1).Text("Stoßleiste");
            table.Cell().Row(7).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.StossleisteGewicht)} kg");
            table.Cell().Row(7).Column(3).AlignRight().PaddingRight(15).Text($"({CarWeightResult.AnzahlReihenStossleiste}x) {Math.Round(CarWeightResult.StossleisteGewichtproMeter, 2)} kg/m");
            table.Cell().Row(7).Column(4).PaddingLeft(30).Text($"{Math.Round(CarWeightResult.StossleisteLaenge, 2)} m");
            table.Cell().Row(8).Column(1).Text($"Handlauf {ParameterDictionary["var_Handlauf"].Value}");
            table.Cell().Row(8).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.HandlaufGewicht)} kg");
            table.Cell().Row(8).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.HandlaufGewichtproMeter, 2)} kg/m");
            table.Cell().Row(8).Column(4).PaddingLeft(30).Text($"{Math.Round(CarWeightResult.HandlaufLaenge, 2)} m");
            table.Cell().Row(9).Column(1).Text($"Sockelleiste {ParameterDictionary["var_Sockelleiste"].Value}");
            table.Cell().Row(9).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.SockelleisteGewicht)} kg");
            table.Cell().Row(9).Column(3).AlignRight().PaddingRight(15).Text($"{Math.Round(CarWeightResult.SockelleisteGewichtproMeter, 2)} kg/m");
            table.Cell().Row(9).Column(4).PaddingLeft(30).Text($"{Math.Round(CarWeightResult.SockelleisteLaenge, 2)} m");
        });
    }

    void TableCarAccesories(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.5f);
                columns.RelativeColumn(0.5f);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });
            table.Cell().Row(1).Column(1).ColumnSpan(4).Text("Kabinenzubehör").FontSize(fontSizeXL).Bold();
            table.Cell().Row(2).Column(1).Text("Schutzgeländer");
            table.Cell().Row(2).Column(2).AlignRight().Text($"{Math.Round(CarWeightResult.SchutzgelaenderGewicht)} kg");
            table.Cell().Row(2).Column(3).ColumnSpan(2).PaddingLeft(15).Text($"Anzahl Pfosten: {CarWeightResult.SchutzgelaenderAnzahlPfosten} Stk.");
            table.Cell().Row(3).Column(1).Text("Klemmkasten");
            table.Cell().Row(3).Column(2).AlignRight().Text($"{CarWeightResult.KlemmkastenGewicht} kg");
            table.Cell().Row(4).Column(1).Text("Schraubenzubehör");
            table.Cell().Row(4).Column(2).AlignRight().Text($"{CarWeightResult.SchraubenZubehoerGewicht} kg");
            table.Cell().Row(5).Column(1).Text("Tableau");
            table.Cell().Row(5).Column(2).AlignRight().Text($"{CarWeightResult.TableauGewicht} kg");
            table.Cell().Row(5).Column(3).ColumnSpan(2).PaddingLeft(15).Text($"Abzug Tableaubreite ({CarWeightResult.TableauBreite} mm) für Paneel- und Glasberechnung");
        });
    }

    void TableCarweights(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });
            table.Cell().Row(1).Column(1).ColumnSpan(2).Text("Kabinendetailgewichte").FontSize(fontSizeXL).Bold();
            table.Cell().Row(2).Column(1).Background(string.IsNullOrWhiteSpace(ParameterDictionary["var_KabinengewichtCAD"].Value) ? secondaryVariantColor : highlightColor).Text(string.IsNullOrWhiteSpace(ParameterDictionary["var_KabinengewichtCAD"].Value) ? "Kabinengewicht gesamt" : "ermitteltes Kabinengewicht CAD/berechnet");
            table.Cell().Row(2).Column(2).PaddingRight(80, Unit.Millimetre).Background(string.IsNullOrWhiteSpace(ParameterDictionary["var_KabinengewichtCAD"].Value) ? secondaryVariantColor : highlightColor).AlignRight().Text($"{Math.Round(CarWeightResult.KabinenGewichtGesamt)} kg  ");
            table.Cell().Row(3).Column(1).Background(secondaryVariantColor).Text("Kabinenkorrekturgewicht");
            table.Cell().Row(3).Column(2).PaddingRight(80, Unit.Millimetre).Background(secondaryVariantColor).AlignRight().Text($"{ParameterDictionary["var_F_Korr"].Value} kg  ");
            table.Cell().Row(4).Column(1).Background(secondaryVariantColor).Text("Kabinentürgewicht");
            table.Cell().Row(4).Column(2).PaddingRight(80, Unit.Millimetre).Background(secondaryVariantColor).AlignRight().Text($"{Math.Round(CarWeightResult.KabinenTuerGewicht)} kg  ");
            table.Cell().Row(5).Column(1).Background(secondaryVariantColor).Text("Fangrahmengewicht");
            table.Cell().Row(5).Column(2).PaddingRight(80, Unit.Millimetre).Background(secondaryVariantColor).AlignRight().Text($"{Math.Round(CarWeightResult.FangrahmenGewicht)} kg  ");
            table.Cell().Row(6).Column(1).Background(secondaryVariantColor).Text("Gesamtgewicht [F]").Bold();
            table.Cell().Row(6).Column(2).PaddingRight(80, Unit.Millimetre).Background(secondaryVariantColor).AlignRight().Text($"{Math.Round(CarWeightResult.FahrkorbGewicht)} kg  ").Bold();
        });
    }
}
