namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class BufferPropProfile : BaseEntity
{
    public double AreaOfProfile { get; set; }
    public double MomentOfInertiaX { get; set; }
    public double MomentOfInertiaY { get; set; }
    public double RadiusOfInertiaX { get; set; }
    public double RadiusOfInertiaY { get; set; }
    public double CenterOfGravityAxisX { get; set; }
    public double CenterOfGravityAxisY { get; set; }
}
