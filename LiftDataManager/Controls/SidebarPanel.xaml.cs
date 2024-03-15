using CommunityToolkit.WinUI.Collections;
using System.Collections.ObjectModel;

namespace LiftDataManager.Controls;

public sealed partial class SidebarPanel : UserControl
{
    public QuickLinksViewModel ViewModel
    {
        get;
    }
    public SidebarPanel()
    {
        ViewModel = App.GetService<QuickLinksViewModel>();
        InitializeComponent();
        InfoCenterEntrysView ??= new();
    }

    public AdvancedCollectionView InfoCenterEntrysView { get; set; }

    private int selectedIndexQuantity;
    public int SelectedIndexQuantity
    {
        get { return selectedIndexQuantity; }
        set 
        {
            selectedIndexQuantity = value;

            //InfoCenterEntrysView.Source.RemoveAt(2);
            //InfoCenterEntrysView.Filter = _ => true;
            //InfoCenterEntrysView.Source = InfoCenterEntrys;
            //InfoCenterEntrysView = new AdvancedCollectionView(InfoCenterEntrys, true);
            //InfoCenterEntrysView.SortDescriptions.Add(new SortDescription("TimeStamp", SortDirection.Descending));

        }
    }

    private int selectedIndexInfoCenterTyp;
    public int SelectedIndexInfoCenterTyp
    {
        get { return selectedIndexInfoCenterTyp; }
        set
        {
            selectedIndexInfoCenterTyp = value;
            InfoCenterEntrysView.Filter = FilterInfoCenterEntrys(value);
        }
    }

    public ObservableCollection<InfoCenterEntry> InfoCenterEntrys
    {
        get { return (ObservableCollection<InfoCenterEntry>)GetValue(InfoCenterEntrysProperty); }
        set
        {
            SetValue(InfoCenterEntrysProperty, value);
            InfoCenterEntrysView = new AdvancedCollectionView(value, true);
            InfoCenterEntrysView.SortDescriptions.Add(new SortDescription("TimeStamp", SortDirection.Descending));
        }
    }

    public static readonly DependencyProperty InfoCenterEntrysProperty =
        DependencyProperty.Register(nameof(InfoCenterEntrys), typeof(ObservableCollection<InfoCenterEntry>), typeof(SidebarPanel), new PropertyMetadata(default));

    public bool CanTextClear
    {
        get => (bool)GetValue(CanTextClearProperty);
        set => SetValue(CanTextClearProperty, value);
    }

    public static readonly DependencyProperty CanTextClearProperty =
        DependencyProperty.Register(nameof(CanTextClear), typeof(bool), typeof(SidebarPanel), new PropertyMetadata(false));

    public bool ShowQuickLinks
    {
        get
        {
            ViewModel.CheckCanOpenFiles();
            return (bool)GetValue(ShowQuickLinksProperty);
        }
        set => SetValue(ShowQuickLinksProperty, value);
    }

    public static readonly DependencyProperty ShowQuickLinksProperty =
        DependencyProperty.Register(nameof(ShowQuickLinks), typeof(bool), typeof(SidebarPanel), new PropertyMetadata(false));

    public bool InfoCenterIsOpen
    {
        get { return (bool)GetValue(InfoCenterIsOpenProperty); }
        set { SetValue(InfoCenterIsOpenProperty, value); }
    }

    public static readonly DependencyProperty InfoCenterIsOpenProperty =
        DependencyProperty.Register(nameof(InfoCenterIsOpen), typeof(bool), typeof(SidebarPanel), new PropertyMetadata(false));

    private void ClearEntrys_Click(object sender, RoutedEventArgs e)
    {
        InfoCenterEntrys.Clear();
    }

    private static Predicate<object> FilterInfoCenterEntrys(int index)
    {
        return (index < 1 || index > 6) ? _ => true : x => ((InfoCenterEntry)x).State.Value == index;
    }
}
