namespace LiftDataManager.Controls;
public sealed partial class SpezifikationsNumberControl : UserControl
{
    public SpezifikationsNumberControl()
    {
        InitializeComponent();

        SpezifikationTyps = Enum.GetValues(typeof(SpezifikationTyp)).Cast<SpezifikationTyp>().Take(3).ToList();
        Years = new List<int> { 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
        Months = Enum.GetValues(typeof(MonthGerman)).Cast<MonthGerman>().ToList();
    }
    public List<SpezifikationTyp> SpezifikationTyps { get; set; }
    public List<int> Years { get; set; }

    public List<MonthGerman> Months { get; set; }


}
