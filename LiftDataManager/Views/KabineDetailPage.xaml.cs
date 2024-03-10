namespace LiftDataManager.Views;

public sealed partial class KabineDetailPage : Page
{
    public KabineDetailViewModel ViewModel
    {
        get;
    }

    public KabineDetailPage()
    {
        ViewModel = App.GetService<KabineDetailViewModel>();
        InitializeComponent();
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var detailPage = e.AddedItems.FirstOrDefault();
        if (detailPage is null)
            return;
        if (detailPage is not PivotItem || ((PivotItem)detailPage).Name != "CarEquipment")
            return;

        CarEquipmentControlA.RefreshView();
        CarEquipmentControlB.RefreshView();
        CarEquipmentControlC.RefreshView();
        CarEquipmentControlD.RefreshView();
    }
}
