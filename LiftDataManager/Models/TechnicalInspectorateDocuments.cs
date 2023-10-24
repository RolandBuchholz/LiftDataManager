namespace LiftDataManager.Models
{
    public class TechnicalInspectorateDocuments
    {
        public enum Month
        {
            Januar = 1,
            Februar,
            März,
            April,
            Mai,
            Juni,
            Juli,
            August,
            September,
            Oktober,
            November,
            Dezember
        }

        public int ManufactureYear { get; set; }
        public int YearOfConstruction { get; set; }
        public Month MonthOfConstruction { get; set; }
    }
}
