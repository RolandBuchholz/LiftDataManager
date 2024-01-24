using LiftDataManager.Core.Models.ComponentModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LiftDataManager.Core.Models.PdfDocuments;
public class TableENComponent : PdfBaseDocument, IComponent
{
    private readonly List<TableRow<int, double>> tableEN;

    public TableENComponent(List<TableRow<int, double>> table, bool lowColor, bool lowHighlightColor)
    {
        tableEN = table;
        SetPdfStyle(lowColor, lowHighlightColor);
    }

    public void Compose(IContainer container)
    {
        var firstItem = tableEN.First().FirstValue;
        var tableName = firstItem switch
        {
            1 => "table8",
            100 => "table6",
            400 => "table7",
            _ => throw new NotImplementedException()
        };

        string header = string.Empty;
        string footer = string.Empty;

        var headerColumn = tableName == "" ? "Personen" : "Nennlast";
        var headerValue = "Fläche";

        switch (tableName)
        {
            case "table6":
                header = "Tabelle6 - Nutzfläche des Fahrkorbs für Personenaufzüge";
                footer = "wenn Nutzfläche größer 5.0 m² dann zusätzlich 100 kg pro 0.16 m²";
                break;
            case "table7":
                header = "Tabelle7 - reduzierte Nutzfläche für hydraulische Lastenaufzüge";
                footer = "wenn Nutzfläche größer 5.04 m² dann zusätzlich 100 kg pro 0.40 m²";
                break;
            case "table8":
                header = "Tabelle8 - Personenanzahl bezogen auf die Nutzfläche";
                footer = "wenn Nutzfläche größer 3.13 m² dann eine zusätzliche Person pro 0.115 m²";
                break;
        }

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Cell().Row(1).Column(1).ColumnSpan(4).Padding(0.5f).Background(primaryVariantColor).AlignCenter().Text(header).FontSize(fontSizeS).FontColor(onPrimaryColor);
            table.Cell().Row(2).Column(1).Padding(0.5f).Background(primaryColor).AlignCenter().Text(headerColumn).FontSize(fontSizeXS).FontColor(onSecondaryColor);
            table.Cell().Row(2).Column(2).Padding(0.5f).Background(primaryColor).AlignCenter().Text(headerValue).FontSize(fontSizeXS).FontColor(onSecondaryColor);
            table.Cell().Row(2).Column(3).Padding(0.5f).Background(primaryColor).AlignCenter().Text(headerColumn).FontSize(fontSizeXS).FontColor(onSecondaryColor);
            table.Cell().Row(2).Column(4).Padding(0.5f).Background(primaryColor).AlignCenter().Text(headerValue).FontSize(fontSizeXS).FontColor(onSecondaryColor);

            for (int i = 0; i < tableEN.Count / 2; i++)
            {
                table.Cell().Padding(0.5f).Background(tableEN[i].IsSelected ? highlightColor : secondaryColor).AlignCenter().Text($"{tableEN[i].FirstValue} {tableEN[i].FirstUnit}").FontSize(fontSizeXS);
                table.Cell().Padding(0.5f).Background(tableEN[i].IsSelected ? highlightColor : secondaryColor).AlignCenter().Text($"{tableEN[i].SecondValue} {tableEN[i].SecondUnit}").FontSize(fontSizeXS);
                table.Cell().Padding(0.5f).Background(tableEN[i + tableEN.Count / 2].IsSelected ? highlightColor : secondaryColor).AlignCenter().Text($"{tableEN[i + tableEN.Count / 2].FirstValue} {tableEN[i + tableEN.Count / 2].FirstUnit}").FontSize(fontSizeXS);
                table.Cell().Padding(0.5f).Background(tableEN[i + tableEN.Count / 2].IsSelected ? highlightColor : secondaryColor).AlignCenter().Text($"{tableEN[i + tableEN.Count / 2].SecondValue} {tableEN[i + tableEN.Count / 2].SecondUnit}").FontSize(fontSizeXS);
            }

            if (tableEN.Count % 2 != 0)
            {
                table.Cell().Padding(0.5f).Background(tableEN[tableEN.Count - 1].IsSelected ? highlightColor : secondaryColor).AlignCenter().Text($"{tableEN[tableEN.Count - 1].FirstValue} {tableEN[tableEN.Count - 1].FirstUnit}").FontSize(fontSizeXS);
                table.Cell().Padding(0.5f).Background(tableEN[tableEN.Count - 1].IsSelected ? highlightColor : secondaryColor).AlignCenter().Text($"{tableEN[tableEN.Count - 1].SecondValue} {tableEN[tableEN.Count - 1].SecondUnit}").FontSize(fontSizeXS);
            }

            table.Cell().ColumnSpan(4).Padding(0.5f).Background(primaryColor).PaddingLeft(5).Text(footer).FontSize(8).FontColor(onPrimaryColor);
        });
    }
}