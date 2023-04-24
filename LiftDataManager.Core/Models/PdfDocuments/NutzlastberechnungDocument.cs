﻿using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Models.PdfDocuments;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;

namespace PDFTests.Services.DocumentGeneration;

public class NutzlastberechnungDocument : PdfBaseDocument
{
    private readonly ICalculationsModule _calculationsModuleService;
    public PayLoadResult PayLoadResult = new();

    public NutzlastberechnungDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService)
    {
        _calculationsModuleService = calculationsModuleService;
        ParameterDictionary = parameterDictionary;
        PayLoadResult = _calculationsModuleService.GetPayLoadCalculation(parameterDictionary);
        Title = "Nutzfläche des Fahrkorbs";
    }

    protected override void Content(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Height(100).Padding(10, Unit.Millimetre).Background(Colors.Blue.Lighten1).Text("NutzlastberechnungDocument");
        });
    }
}
