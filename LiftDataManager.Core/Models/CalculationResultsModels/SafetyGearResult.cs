namespace LiftDataManager.Core.Models.CalculationResultsModels;

public class SafetyGearResult
{
    public string? CarRailSurface { get; set; }
    public string? Lubrication { get; set; }
    public string? AllowedRailHeads { get; set; }
    public bool RailHeadAllowed { get; set; }
    public int MinLoad { get; set; }
    public int MaxLoad { get; set; }
    public bool PipeRuptureValve { get; set; }
}
