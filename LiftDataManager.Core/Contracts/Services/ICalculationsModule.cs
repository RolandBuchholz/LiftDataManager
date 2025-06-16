using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.Models.CalculationResultsModels;
using LiftDataManager.Core.Models.ComponentModels;

namespace LiftDataManager.Core.Contracts.Services;
public interface ICalculationsModule
{
    Dictionary<int, TableRow<int, double>> Table6 { get; }
    Dictionary<int, TableRow<int, double>> Table7 { get; }
    Dictionary<int, TableRow<int, double>> Table8 { get; }

    /// <summary>
    /// reset all values
    /// </summary>
    /// <returns>Task</returns>
    Task ResetAsync();

    int GetPersonenCarArea(double area);

    double GetLoadFromTable(double area, string tableName);

    bool ValdidateLiftLoad(double load, double area, string cargotyp, string drivesystem);

    double GetCarFrameWeight(ObservableDictionary<string, Parameter> parameterDictionary);

    int GetMaxFuse(string? inverter);

    string GetDriveTyp(string? driveSystem, int driveSuspension);

    string GetDriveControl(string? driveTyp);

    string GetLiftTyp(string? liftTyp);

    string GetSoftStartTyp(string? liftcontroltyp, bool isRopeLift);

    string GetDrivePosition(string? drivePos);

    bool IsRopeLift(SelectionValue? carTyp);

    string GetDistanceBetweenDoors(ObservableDictionary<string, Parameter> parameterDictionary, string orientation);

    int GetNumberOfCardoors(ObservableDictionary<string, Parameter> parameterDictionary);

    int GetRammingProtectionRows(ObservableDictionary<string, Parameter> parameterDictionary, string? rammingProtectionTyp);

    CarFrameType? GetCarFrameTyp(ObservableDictionary<string, Parameter> parameterDictionary);

    CarVentilationResult GetCarVentilationCalculation(ObservableDictionary<string, Parameter> parameterDictionary);

    PayLoadResult GetPayLoadCalculation(ObservableDictionary<string, Parameter> parameterDictionary);

    CarWeightResult GetCarWeightCalculation(ObservableDictionary<string, Parameter> parameterDictionary);

    SafetyGearResult GetSafetyGearCalculation(ObservableDictionary<string, Parameter> parameterDictionary, bool counterweightSafetyGear);

    List<LiftSafetyComponent> GetLiftSafetyComponents(ObservableDictionary<string, Parameter> parameterDictionary);

    List<LiftSafetyComponent> GetUCMPComponents(ObservableDictionary<string, Parameter> parameterDictionary);

    BufferCalculationData GetBufferCalculationData(ObservableDictionary<string, Parameter> parameterDictionary, string parameterName, int eulerCase, bool bufferUnderCounterweight);

    string GetBufferDetails(string buffertyp, double liftSpeed);

    int GetmaxBufferStoke(string? buffertyp);

    double GetCurrentBufferForce(ObservableDictionary<string, Parameter> parameterDictionary, string bufferParameterName);

    bool ValidateBufferRange(string buffertyp, double liftSpeed, double bufferLoad);

    (double, double) GetMirrorWidth(ObservableDictionary<string, Parameter> parameterDictionary, string wallSide, int index);

    (double, double) GetMirrorHeight(ObservableDictionary<string, Parameter> parameterDictionary, string wallSide, int index);

    double GetSkirtingBoardHeightByName(ObservableDictionary<string, Parameter> parameterDictionary);

    double GetRammingProtectionHeightByName(ObservableDictionary<string, Parameter> parameterDictionary);

    double GetHandrailDiameterByName(string handrailName);

    string GetGuideRailSurface(SelectionValue? guideRail, SelectionValue? guidetyp);

    void SetPayLoadResult(ObservableDictionary<string, Parameter> parameterDictionary, int personenBerechnet, double nutzflaecheGesamt);

    bool IsOverspeedGovernorWeightRequired(SelectionValue? overSpeedGovernor);

}
