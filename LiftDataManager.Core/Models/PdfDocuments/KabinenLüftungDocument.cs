using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Models.PdfDocuments;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PDFTests.Services.DocumentGeneration;

public class KabinenLüftungDocument : PdfBaseDocument
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly bool LowPrintColor;
    public CarVentilationResult CarVentilationResult = new();

    public KabinenLüftungDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService, bool lowPrintColor)
    {
        _calculationsModuleService = calculationsModuleService;
        ParameterDictionary = parameterDictionary;
        CarVentilationResult = _calculationsModuleService.GetCarVentilationCalculation(parameterDictionary);
        Title = "Be- und Entlüftung";
        LowPrintColor = lowPrintColor;
        SetPdfStyle(LowPrintColor, false);
        LowPrintColor = lowPrintColor;
    }

    protected override void Content(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingLeft(10, Unit.Millimetre).Element(TableCarData);
            column.Item().PaddingLeft(10, Unit.Millimetre).Element(TableVentilationCeiling);
            column.Item().PaddingLeft(10, Unit.Millimetre).Element(TableVentilationCarDoor);
            column.Item().PageBreak();
            column.Item().PaddingTop(10, Unit.Millimetre).PaddingLeft(10, Unit.Millimetre).Element(TableVentilationSkirtingBoard);
        });
    }

    void TableCarData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.RelativeColumn();
            });

            table.Cell().Row(1).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Daten der Aufzugkabine C100 (aufg. Sockel)").FontSize(fontSizeXL).Bold();
            table.Cell().Row(2).Column(1).PaddingLeft(15).Text("Türbreite (TB)");
            table.Cell().Row(2).Column(2).AlignRight().Text($"{ParameterDictionary["var_TB"].Value} mm");
            table.Cell().Row(3).Column(1).PaddingLeft(15).Text("Türhöhe (TH)");
            table.Cell().Row(3).Column(2).AlignRight().Text($"{ParameterDictionary["var_TH"].Value} mm");
            table.Cell().Row(4).Column(1).PaddingLeft(15).Text("Fahrkorbbreite (KB)");
            table.Cell().Row(4).Column(2).AlignRight().Text($"{ParameterDictionary["var_KBI"].Value} mm");
            table.Cell().Row(5).Column(1).PaddingLeft(15).Text("Fahrkorbhöhe (KH)");
            table.Cell().Row(5).Column(2).AlignRight().Text($"{ParameterDictionary["var_KHLicht"].Value} mm");
            table.Cell().Row(6).Column(1).Background(secondaryColor).PaddingLeft(15).Text("Kabinengrundfläche (A)");
            table.Cell().Row(6).Column(2).Background(secondaryColor).AlignRight().Text($"{ParameterDictionary["var_A_Kabine"].Value} m²");
            table.Cell().Row(7).Column(1).Background(secondaryColor).PaddingLeft(15).Text("Kabinengrundfläche (1%)");
            table.Cell().Row(7).Column(2).Background(secondaryColor).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AKabine1Pozent.ToString());
                text.Span(" mm²");
            });
        });
    }

    void TableVentilationCeiling(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.RelativeColumn();
            });

            table.Cell().Row(1).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Belüftung durch die Decke").FontSize(fontSizeXL).Bold();
            table.Cell().Row(2).Column(1).ColumnSpan(3).PaddingLeft(15).PaddingBottom(5).PaddingRight(55, Unit.Millimetre)
                 .Text("In Fahrkorbtiefe ist links und rechts der Decke ein Luftspalt von 10 mm, hierdurch ist ein Luftaustritt durch die offene Decke ins Freie gewährleistet. " +
                 "Die zusätzliche Belüftung durch die Beleuchtungseinheit wurde hier nicht berücksichtigt.");

            table.Cell().Row(3).Column(1).PaddingLeft(15).Text("Fahrkorbtiefe (KT)");
            table.Cell().Row(3).Column(2).AlignRight().Text($"{ParameterDictionary["var_KTI"].Value} mm");
            table.Cell().Row(4).Column(1).PaddingLeft(15).Text("Luftspaltöffnung");
            table.Cell().Row(4).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.Luftspaltoeffnung.ToString());
                text.Span(" mm");
            });
            table.Cell().Row(5).Column(1).Background(secondaryColor).PaddingLeft(15).Text("Belüftung pro Seite");
            table.Cell().Row(5).Column(2).Background(secondaryColor).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.Belueftung1Seite.ToString());
                text.Span(" mm²");
            });
            table.Cell().Row(6).Column(1).Background(secondaryColor).PaddingLeft(15).Text("Gesamtbelüftung (2 Seiten)");
            table.Cell().Row(6).Column(2).Background(secondaryColor).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.Belueftung2Seiten.ToString());
                text.Span(" m²");
            });
            table.Cell().Row(7).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Prüfung Belüftung über Decke").FontSize(16).Bold();
            table.Cell().Row(8).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Row(row =>
            {
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text("Gesamtbelüftung (2 Seiten)");
                row.RelativeItem().AlignCenter().Text(">");
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text("Kabinengrundfläche (1%)");
            });
            table.Cell().Row(9).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Row(row =>
            {
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text(text =>
                {
                    text.Span(CarVentilationResult.Belueftung2Seiten.ToString());
                    text.Span(" mm²");
                });
                row.RelativeItem().AlignCenter().Text(">");
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text(text =>
                {
                    text.Span(CarVentilationResult.AKabine1Pozent.ToString());
                    text.Span(" mm²");
                });
            });
            table.Cell().Row(10).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Text(text =>
            {
                if (CarVentilationResult.ErgebnisBelueftungDecke)
                { text.Span("  Vorschrift erfüllt !  ").Bold().BackgroundColor(successfulColor); }
                else
                { text.Span("  Vorschrift nicht erfüllt !  ").Bold().BackgroundColor(errorColor); };
            });
        });
    }

    void TableVentilationCarDoor(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(80, Unit.Millimetre);
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });

            table.Cell().Row(1).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Entlüftung durch die Fahrkorbtüre").FontSize(fontSizeXL).Bold();
            table.Cell().Row(2).Column(1).PaddingLeft(15).Text("Anzahl Kabinentüren");
            table.Cell().Row(2).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlKabinentueren.ToString());
                text.Span(" Stk");
            });
            table.Cell().Row(3).Column(1).PaddingLeft(15).Text("Anzahl Türflügel je Kabinentür");
            table.Cell().Row(3).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_AnzahlTuerfluegel"].Value);
                text.Span(" Stk");
            });
            table.Cell().Row(4).Column(1).ColumnSpan(2).PaddingLeft(15).Text("Anrechnung zu 50 % des Türluftspaltes auf 1 % der Grundfläche");

            table.Cell().Row(5).Column(1).PaddingLeft(15).Text("Türbreite (TB)");
            table.Cell().Row(5).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_TB"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(6).Column(1).PaddingLeft(15).Text("Türhöhe (TH)");
            table.Cell().Row(6).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_TH"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(7).Column(1).PaddingLeft(15).Text("Luftspaltöffnungen in TB");
            table.Cell().Row(7).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlLuftspaltoeffnungenTB.ToString());
                text.Span(" x ");
                text.Span(CarVentilationResult.Tuerspalt.ToString());
                text.Span("mm x (TB) = ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenTB.ToString());
                text.Span(" mm");
            });
            table.Cell().Row(8).Column(1).PaddingLeft(15).Text("Luftspaltöffnungen in TH");
            table.Cell().Row(8).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlLuftspaltoeffnungenTH.ToString());
                text.Span(" x ");
                text.Span(CarVentilationResult.Tuerspalt.ToString());
                text.Span("mm x (TH) = ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenTH.ToString());
                text.Span(" mm");
            });

            table.Cell().Row(9).Column(1).ColumnSpan(2).Background(secondaryColor).PaddingLeft(15).Row(row =>
            {
                row.AutoItem().Text("Entlüftung durch die Türspalten 50 % (F3.1)");
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span(CarVentilationResult.EntlueftungTuerspalten50Pozent.ToString());
                    text.Span(" mm²");
                });
            });
        });
    }

    void TableVentilationSkirtingBoard(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.RelativeColumn();
            });

            table.Cell().Row(1).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Entlüftung durch die Sockelleisten").FontSize(fontSizeXL).Bold();
            table.Cell().Row(2).Column(1).ColumnSpan(3).PaddingLeft(15).PaddingBottom(5).PaddingRight(55, Unit.Millimetre)
                 .Text("An den Fahrkorbwänden ist links, rechts (und hinten) ein Luftspalt zwischen Sockelleiste und Kabinenboden von 10 mm mit Luftaustritt durch Öffnungen in der Kabinenwand ins Freie.");
            table.Cell().Row(3).Column(1).PaddingLeft(15).Text("Fahrkorbbreite (KB)");
            table.Cell().Row(3).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_KBI"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(4).Column(1).PaddingLeft(15).Text("Fahrkorbtiefe (KT)");
            table.Cell().Row(4).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_KTI"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(5).Column(1).PaddingLeft(15).Text("Luftspaltöffnungen in FB");
            table.Cell().Row(5).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlLuftspaltoeffnungenFB.ToString());
                text.Span(" x ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenFB.ToString());
                text.Span(" mm² = ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenFBGesamt.ToString());
                text.Span(" mm");
            });
            table.Cell().Row(6).Column(1).PaddingLeft(15).Text("Luftspaltöffnungen in FT");
            table.Cell().Row(6).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlLuftspaltoeffnungenFT.ToString());
                text.Span(" x ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenFT.ToString());
                text.Span(" mm² = ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenFTGesamt.ToString());
                text.Span(" mm");
            });
            table.Cell().Row(7).Column(1).ColumnSpan(2).Background(secondaryColor).PaddingLeft(15).Row(row =>
            {
                row.AutoItem().Text("Entlüftung durch die Spalten an den Sockelleisten (F3.2)");
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span(CarVentilationResult.FlaecheEntLueftungSockelleisten.ToString());
                    text.Span(" mm²");
                });
            });
            table.Cell().Row(8).Column(1).Background(secondaryColor).PaddingLeft(15).Text("Entlüftung gesamt (F3.1 + F3.2)");
            table.Cell().Row(8).Column(2).Background(secondaryColor).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.FlaecheEntLueftungGesamt.ToString());
                text.Span(" mm²");
            });

            table.Cell().Row(9).Column(1).ColumnSpan(2).PaddingLeft(15).PaddingTop(15).AlignCenter().Row(row =>
            {
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text("Entlüftung gesamt (F3.1 + F3.2)");
                row.RelativeItem().AlignCenter().Text(">");
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text("Kabinengrundfläche (1%)");
            });
            table.Cell().Row(10).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Row(row =>
            {
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text(text =>
                {
                    text.Span(CarVentilationResult.FlaecheEntLueftungGesamt.ToString());
                    text.Span(" mm²");
                });
                row.RelativeItem().AlignCenter().Text(">");
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text(text =>
                {
                    text.Span(CarVentilationResult.AKabine1Pozent.ToString());
                    text.Span(" mm²");
                });
            });
            table.Cell().Row(11).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Text(text =>
            {
                if (CarVentilationResult.ErgebnisEntlueftung)
                { text.Span("  Vorschrift erfüllt !  ").Bold().BackgroundColor(successfulColor); }
                else
                { text.Span("  Vorschrift nicht erfüllt !  ").Bold().BackgroundColor(errorColor); };
            });
        });
    }
}
