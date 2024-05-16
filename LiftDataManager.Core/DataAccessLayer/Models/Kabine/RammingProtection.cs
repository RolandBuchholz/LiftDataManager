namespace LiftDataManager.Core.DataAccessLayer.Models.Kabine;

public class RammingProtection : BaseEntity
{
    public double WeightPerMeter { get; set; }
    public int NumberOfRows { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
}
