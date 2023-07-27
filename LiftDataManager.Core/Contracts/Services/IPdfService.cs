using Cogs.Collections;

namespace LiftDataManager.Core.Contracts.Services;

public interface IPdfService
{
    bool MakeSinglePdfDocument(string pdfModel, ObservableDictionary<string, Parameter> ParamterDictionary, string? path, bool showPdf, bool lowPrintColor, bool lowHighlightColor);

    bool MakeDefaultSetofPdfDocuments(ObservableDictionary<string, Parameter> ParamterDictionary, string? path);
}
