namespace LiftDataManager.Core.DataAccessLayer.Models.Kabine;

public class CarFloorProfile : BaseEntity
{
    public string? Name { get; set; }
    public double? WeightPerMeter { get; set; }
    public double? Height { get; set; }
}
