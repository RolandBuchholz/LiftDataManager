using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Models.PdfDocuments;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;

namespace PDFTests.Services.DocumentGeneration;

public class KabinengewichtDocument : PdfBaseDocument
{
    private readonly ICalculationsModule _calculationsModuleService;
    public CarWeightResult CarWeightResult = new();

    public KabinengewichtDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService)
    {
        _calculationsModuleService = calculationsModuleService;
        ParameterDictionary = parameterDictionary;
        CarWeightResult = _calculationsModuleService.GetCarWeightCalculation(parameterDictionary);
        Title = "Kabinengewicht";
    }

    protected override void Content(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Height(100).Padding(10, Unit.Millimetre).Background(Colors.Blue.Lighten1).Text("KabinengewichtDocument");
        });
    }
}
