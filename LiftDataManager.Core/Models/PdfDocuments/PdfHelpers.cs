using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LiftDataManager.Core.Models.PdfDocuments;
public static class PdfHelpers
{
    static IContainer Cell(this IContainer container, bool background)
    {
        return container
            .Border(0.5f)
            .BorderColor(Colors.Grey.Lighten1)
            .Background(background ? Colors.Grey.Lighten4 : Colors.White)
            .Padding(5);
    }

    public static IContainer ValueCell(this IContainer container)
    {
        return container.Cell(false);
    }

    public static IContainer LabelCell(this IContainer container)
    {
        return container.Cell(true);
    }
}
