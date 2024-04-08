namespace LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

public class BufferPropProfile : BaseEntity
{
    public double AreaOfProfile { get; set; }
    public double MomentOfInertiaX { get; set; }
    public double MomentOfInertiaY { get; set; }
}
