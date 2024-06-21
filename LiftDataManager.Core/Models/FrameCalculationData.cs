using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.Models;

public class FrameCalculationData
{
    public GuideRails? CarframeRail { get; set; }
    public GuideRails? CounterweightRail { get; set; }
    public double MaxRailBracketSpacing { get; set; }
    public double DistanceGuideShoesCarframe { get; set; }
    public double DistanceGuideShoesCounterweight { get; set; }
    public double CarCenterOfMassX { get; set; }
    public double CarCenterOfMassY { get; set; }
    public double AdditionalRailForceCarframe { get; set; }
    public double AdditionalRailForceCounterweight { get; set; }
    public double CounterweightDepth { get; set; }
    public double CounterweightWidth { get; set; }
    public double OffsetCounterweightSuspensionCenter { get; set; }
    public int CarframeBracketClipCount { get; set; }
    public int CounterweightBracketClipCount { get; set; }
    public bool HasSlidingClips { get; set; }
    public double BuildingDeflectionX { get; set; }
    public double BuildingDeflectionY { get; set; }
}