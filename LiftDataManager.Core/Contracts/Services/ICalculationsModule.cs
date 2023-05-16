using Cogs.Collections;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Models.ComponentModels;

namespace LiftDataManager.Core.Contracts.Services;
public interface ICalculationsModule
{
    Dictionary<int, TableRow<int, double>> Table6 { get; }
    Dictionary<int, TableRow<int, double>> Table7 { get; }
    Dictionary<int, TableRow<int, double>> Table8 { get; }

    int GetPersonenCarArea(double area);

    double GetLoadFromTable(double area, string tableName);

    bool ValdidateLiftLoad(double load, double area, string cargotyp, string drivesystem);

    double GetCarFrameWeight(ObservableDictionary<string, Parameter>? parameterDictionary);

    CarVentilationResult GetCarVentilationCalculation(ObservableDictionary<string, Parameter>? parameterDictionary);

    PayLoadResult GetPayLoadCalculation(ObservableDictionary<string, Parameter>? parameterDictionary);

    CarWeightResult GetCarWeightCalculation(ObservableDictionary<string, Parameter>? parameterDictionary);

    void SetPayLoadResult(ObservableDictionary<string, Parameter> parameterDictionary, int personenBerechnet, double nutzflaecheGesamt);
}
