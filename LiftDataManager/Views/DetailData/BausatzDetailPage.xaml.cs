namespace LiftDataManager.Views;

public sealed partial class BausatzDetailPage : Page
{
    public BausatzDetailViewModel ViewModel
    {
        get;
    }

    public BausatzDetailPage()
    {
        ViewModel = App.GetService<BausatzDetailViewModel>();
        InitializeComponent();
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //var detailPage = e.AddedItems.FirstOrDefault();
        //if (detailPage is null)
        //    return;
        //if (detailPage is not PivotItem || ((PivotItem)detailPage).Name != "CarEquipment")
        //    return;

        //CarEquipmentControlA.RefreshView();
        //CarEquipmentControlB.RefreshView();
        //CarEquipmentControlC.RefreshView();
        //CarEquipmentControlD.RefreshView();
    }
}
