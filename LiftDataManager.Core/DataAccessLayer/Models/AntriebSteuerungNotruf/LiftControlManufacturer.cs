namespace LiftDataManager.Core.DataAccessLayer.Models.AntriebSteuerungNotruf;

public class LiftControlManufacturer : SelectionEntity
{
    public int DetectionDistance { get; set; }
    public int DetectionDistanceSIL3 { get; set; }
    public int DeadTime { get; set; }
    public int DeadTimeZAsbc4 { get; set; }
    public int DeadTimeSIL3 { get; set; }
    public double Speeddetector { get; set; }
    public double SpeeddetectorSIL3 { get; set; }
}
