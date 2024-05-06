namespace LiftDataManager.Core.Models;

public class BufferCalculationData
{
    public required string ProfilDescription { get; set; }
    public int NumberOfBuffer { get; set; }
    public required int BufferPillarLength { get; set; }
    public required int BucklingLength { get; set; }
    public required string ProfilMaterial { get; set; }
    public required int EulerCase { get; set; }
    public double Area { get; set; }
    public double MomentOfInertiaX { get; set; }
    public double MomentOfInertiaY { get; set; }
    public double RadiusOfInertiaX { get; set; }
    public double RadiusOfInertiaY { get; set; }
    public double CenterOfGravityAxisY { get; set; }
    public double CenterOfGravityAxisX { get; set; }
    public bool ReducedSafetyRoomBufferUnderCounterweight { get; set; }
}
