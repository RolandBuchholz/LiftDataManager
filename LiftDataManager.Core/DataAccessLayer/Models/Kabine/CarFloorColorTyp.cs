namespace LiftDataManager.Core.DataAccessLayer.Models.Kabine;

public class CarFloorColorTyp : SelectionEntity
{
    public int CarFloorId { get; set; }
    public CarFlooring? CarFlooring { get; set; }
    public string? Image { get; set; }
}
