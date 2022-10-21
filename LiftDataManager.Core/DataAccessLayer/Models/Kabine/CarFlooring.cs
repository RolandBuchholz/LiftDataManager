namespace LiftDataManager.Core.DataAccessLayer.Models.Kabine;

public class CarFlooring : BaseEntity
{
    public string? Name { get; set; }
    public double? Thickness { get; set; }
    public double? WeightPerSquareMeter { get; set; }
    public bool SpecialSheet { get; set; }
}
