namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class GuideRails : SelectionEntity
{
    public bool UsageAsCarRail { get; set; }
    public bool UsageAsCwtRail { get; set; }
    public bool Machined { get; set; }
    public double RailHead { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public double Area { get; set; }
    public double MomentOfInertiaX { get; set; }
    public double MomentOfInertiaY { get; set; }
    public double ModulusOfResistanceX { get; set; }
    public double ModulusOfResistanceY { get; set; }
    public double FlangeC { get; set; }
    public double RadiusOfInertiaX { get; set; }
    public double RadiusOfInertiaY { get; set; }
    public double ThicknessF { get; set; }
    public required string ForgedClips { get; set; }
    public double ForgedClipsForce { get; set; }
    public required string SlidingClips { get; set; }
    public double SlidingClipsForce { get; set; }
    public double Weight { get; set; }
    public string? Material { get; set; }
}
