namespace LiftDataManager.Core.Models;

public class RopeCalculationData
{
    public required string RopeDescription { get; set; }
    public int NumberOfRopes { get; set; }
    public double RopeDiameter { get; set; }
    public int MinimumBreakingStrength { get; set; }
    public int WireStrength { get; set; }
    public double RopeWeight { get; set; }
    public int MaximumNumberOfRopes { get; set; }
    public double RopeLength { get; set; }
}
