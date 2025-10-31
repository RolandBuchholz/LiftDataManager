using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;

namespace LiftDataManager.Core.Contracts.Services;

public interface IPdfService
{
    bool MakeSinglePdfDocument(string pdfModel, ObservableDictionary<string, Parameter> ParameterDictionary, string? path, bool showPdf, bool lowPrintColor, bool lowHighlightColor);

    bool GenerateSafetyComponentsReport(ObservableDictionary<string, Parameter> ParameterDictionary, LiftCommission? liftCommission);

    bool MakeDefaultSetofPdfDocuments(ObservableDictionary<string, Parameter> ParameterDictionary, string? path);
}
