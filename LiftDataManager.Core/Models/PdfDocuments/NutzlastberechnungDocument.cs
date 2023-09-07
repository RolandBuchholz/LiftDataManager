using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Models.PdfDocuments;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PDFTests.Services.DocumentGeneration;

public class NutzlastberechnungDocument : PdfBaseDocument
{
    private readonly ICalculationsModule _calculationsModuleService;
    public PayLoadResult PayLoadResult = new();
    private readonly bool LowPrintColor;

    public NutzlastberechnungDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService, bool lowPrintColor)
    {
        _calculationsModuleService = calculationsModuleService;
        ParameterDictionary = parameterDictionary;
        PayLoadResult = _calculationsModuleService.GetPayLoadCalculation(parameterDictionary);
        Title = "Nutzfläche des Fahrkorbs";
        LowPrintColor = lowPrintColor;
        SetPdfStyle(LowPrintColor, false);
    }

    protected override void Content(IContainer container)
    {
        container.PaddingLeft(10, Unit.Millimetre).Column(column =>
        {
            column.Item().PaddingTop(5, Unit.Millimetre).Text("Nutzfläche des Fahrkorbs, Nennlast,Anzahl der Personen (EN81:20 - 5.4.2)").FontSize(fontSizeXL).Bold();
            column.Item().PaddingTop(2, Unit.Millimetre).Text(text =>
            {
                text.Span(PayLoadResult.CargoTyp).FontSize(fontSizeL).Bold();
                text.Span(" (");
                text.Span(PayLoadResult.DriveSystem).FontSize(fontSizeL).Bold();
                text.Span(") ");
                text.Span(ParameterDictionary["var_Aufzugstyp"].Value).FontSize(fontSizeS).Bold();
            });
            column.Item().Row(row =>
            {
                row.AutoItem().MinHeight(475).Element(TableCarData);
                row.RelativeItem().PaddingVertical(5, Unit.Millimetre).PaddingHorizontal(15, Unit.Millimetre).Component(new CarDesignComponent(ParameterDictionary, LowPrintColor));
            });
            column.Item().Text(text =>
            {
                text.Line("* 5.4.2.1.3").FontSize(10).Bold();
                text.Line("a) Flächen mit einer Tiefe zu einem Türblatt von 100 mm oder weniger (einschließlich schneller und langsamer Türblätter bei mehrblättrigen Türen) dürfen bei der Grundfläche nicht berücksichtigt werden.").FontSize(10);
                text.Line("b) Bei Flächen, die mehr als 100 mm tief sind, muss die gesamte Nutzfläche der Türen zur Grundfläche hinzugefügt werden.").FontSize(10);
                text.Line("Info:").FontSize(10).Bold();
                text.Line("Bei Türbreite gleich Kabinenbreite wird die gesamte Nutzfläche der Türen zur Grundfläche hinzugefügt.").FontSize(10);
            });
            column.Item().PageBreak();
            column.Item().PaddingTop(15).PaddingBottom(0).Text("Nennlast und größte Nutzfläche des Fahrkorbs").FontSize(14).Bold();
            column.Item().Row(row =>
            {
                row.AutoItem().Element(TablePersonData);
                row.RelativeItem().PaddingTop(5, Unit.Millimetre).PaddingHorizontal(10, Unit.Millimetre).Component(new TableENComponent(_calculationsModuleService.Table8.Values.ToList(), LowPrintColor, false));
            });

            column.Item().PaddingTop(10, Unit.Millimetre).PaddingLeft(10).PaddingRight(60, Unit.Millimetre).Component(new TableENComponent(_calculationsModuleService.Table6.Values.ToList(), LowPrintColor, false));
            column.Item().PaddingTop(10, Unit.Millimetre).PaddingLeft(10).PaddingRight(60, Unit.Millimetre).Component(new TableENComponent(_calculationsModuleService.Table7.Values.ToList(), LowPrintColor, false));
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
            table.Cell().Row(3).Column(1).PaddingLeft(10).Text("Anzahl Kabinentüren");
            table.Cell().Row(3).Column(2).AlignRight().Text($"{PayLoadResult.AnzahlKabinentueren} Stk");
            table.Cell().Row(4).Column(1).ColumnSpan(2).ShowIf(PayLoadResult.ZugangA).PaddingLeft(10).Column(column =>
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
                    row.AutoItem().Text("Türeinbaumaß");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_TuerEinbau"].Value} mm");
                });
            });
            table.Cell().Row(5).Column(1).ColumnSpan(2).ShowIf(PayLoadResult.ZugangB).PaddingLeft(10).Column(column =>
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
                    row.AutoItem().Text("Türeinbaumaß");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_TuerEinbauB"].Value} mm");
                });
            });
            table.Cell().Row(6).Column(1).ColumnSpan(2).ShowIf(PayLoadResult.ZugangC).PaddingLeft(10).Column(column =>
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
                    row.AutoItem().Text("Türeinbaumaß");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_TuerEinbauC"].Value} mm");
                });
            });
            table.Cell().Row(7).Column(1).ColumnSpan(2).ShowIf(PayLoadResult.ZugangD).PaddingLeft(10).Column(column =>
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
                    row.AutoItem().Text("Türeinbaumaß");
                    row.RelativeItem().AlignRight().Text($"{ParameterDictionary["var_TuerEinbauD"].Value} mm");
                });
            });
            table.Cell().Row(8).Column(1).PaddingLeft(10).Text("Kabinen Nutzfläche").FontSize(fontSizeL).Bold();
            table.Cell().Row(9).Column(1).PaddingLeft(10).Text("Fahrkorbfläche");
            table.Cell().Row(9).Column(2).AlignRight().Text($"{PayLoadResult.NutzflaecheKabine} m²");
            table.Cell().Row(10).Column(1).ShowIf(PayLoadResult.ZugangA).PaddingLeft(10).Text(text =>
            {
                text.Span("Fläche Zugang A*");
                if (PayLoadResult.NutzflaecheZugangA == 0)
                    text.Span(" Tiefe < 100");
            });
            table.Cell().Row(10).Column(2).ShowIf(PayLoadResult.ZugangA).AlignRight().Text($"{PayLoadResult.NutzflaecheZugangA} m²");

            table.Cell().Row(11).Column(1).ShowIf(PayLoadResult.ZugangB).PaddingLeft(10).Text(text =>
            {
                text.Span("Fläche Zugang B*");
                if (PayLoadResult.NutzflaecheZugangB == 0)
                    text.Span(" Tiefe < 100");
            });
            table.Cell().Row(11).Column(2).ShowIf(PayLoadResult.ZugangB).AlignRight().Text($"{PayLoadResult.NutzflaecheZugangB} m²");
            table.Cell().Row(12).Column(1).ShowIf(PayLoadResult.ZugangC).PaddingLeft(10).Text(text =>
            {
                text.Span("Fläche Zugang C*");
                if (PayLoadResult.NutzflaecheZugangC == 0)
                    text.Span(" Tiefe < 100");
            });
            table.Cell().Row(12).Column(2).ShowIf(PayLoadResult.ZugangC).AlignRight().Text($"{PayLoadResult.NutzflaecheZugangC} m²");
            table.Cell().Row(13).Column(1).ShowIf(PayLoadResult.ZugangD).PaddingLeft(10).Text(text =>
            {
                text.Span("Fläche Zugang D*");
                if (PayLoadResult.NutzflaecheZugangD == 0)
                    text.Span(" Tiefe < 100");
            });
            table.Cell().Row(13).Column(2).ShowIf(PayLoadResult.ZugangD).AlignRight().Text($"{PayLoadResult.NutzflaecheZugangD} m²");
            table.Cell().Row(14).Column(1).PaddingLeft(10).Text("Fahrkorbfläche Gesamt");
            table.Cell().Row(14).Column(2).AlignRight().Text($"{PayLoadResult.NutzflaecheGesamt} m²");
        });
    }

    void TablePersonData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.ConstantColumn(25, Unit.Millimetre);
            });
            table.Cell().Row(1).Column(1).ColumnSpan(2).PaddingLeft(10).Text("Nutzfläche des Fahrkorbs für Personenaufzüge").FontSize(fontSizeXS).Bold();
            table.Cell().Row(2).Column(1).PaddingLeft(10).Text("minimale Nennlast (Tabelle 6)");
            table.Cell().Row(2).Column(2).AlignRight().Text($"{PayLoadResult.NennLastTabelle6} kg");
            table.Cell().Row(3).Column(1).ColumnSpan(2).PaddingLeft(10).Text("reduzierte Nutzfläche (für hydraulisch angetriebene Lastenaufzüge)").FontSize(fontSizeXS).Bold();
            table.Cell().Row(4).Column(1).PaddingLeft(10).Text("minimale Nennlast (Tabelle 7)");
            table.Cell().Row(4).Column(2).AlignRight().Text($"{PayLoadResult.NennLastTabelle7} kg");
            table.Cell().Row(5).Column(1).PaddingLeft(10).Background(PayLoadResult.PayloadAllowed ? successfulColor : errorColor).Text("Nennlast (gewählt)").Bold();
            table.Cell().Row(5).Column(2).Background(PayLoadResult.PayloadAllowed ? successfulColor : errorColor).AlignRight().Text($"{ParameterDictionary["var_Q"].Value} kg").Bold();
            table.Cell().Row(6).Column(1).ColumnSpan(2).AlignCenter().Text(text =>
            {
                if (PayLoadResult.PayloadAllowed)
                {
                    text.Line("Nennlast enspricht der EN81:20!").Bold().FontColor(successfulColor);
                }
                else
                {
                    text.Line("Nennlast enspricht nicht der EN81:20!").Bold().FontColor(errorColor);
                }
            });
            table.Cell().Row(7).Column(1).ColumnSpan(2).PaddingTop(15).PaddingBottom(0).Text("Anzahl der Personen (5.4.2.3)").FontSize(fontSizeL).Bold();
            table.Cell().Row(8).Column(1).ColumnSpan(2).PaddingLeft(10).Text("Anzahl der Personen berechnet (Nutzlast / 75)").FontSize(fontSizeXS).Bold();
            table.Cell().Row(9).Column(1).PaddingLeft(10).Text("Anzahl Personen");
            table.Cell().Row(9).Column(2).AlignRight().Text($"{PayLoadResult.Personen75kg} Per.");
            table.Cell().Row(10).Column(1).ColumnSpan(2).PaddingLeft(10).Text("Anzahl der Personen und kleinste Nutzfläche des Fahrkorbs").FontSize(fontSizeXS).Bold();
            table.Cell().Row(11).Column(1).PaddingLeft(10).Text("Anzahl Personen (Tabelle 8)");
            table.Cell().Row(11).Column(2).AlignRight().Text($"{PayLoadResult.PersonenFlaeche} Per.");
            table.Cell().Row(12).Column(1).PaddingLeft(10).Background(secondaryColor).Text("Anzahl Personen").Bold();
            table.Cell().Row(12).Column(2).Background(secondaryColor).AlignRight().Text($"{PayLoadResult.PersonenBerechnet} Per.").Bold();
        });
    }
}
