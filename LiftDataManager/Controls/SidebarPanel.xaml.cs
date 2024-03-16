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
        Unloaded += SidebarPanel_Unloaded;
    }

    private void SidebarPanel_Unloaded(object sender, RoutedEventArgs e)
    {
        InfoCenterEntrys.CollectionChanged -= InfoCenterEntrys_CollectionChanged;
    }

    public CollectionViewSource InfoCenterEntrysView { get; set; }

    private int _maxEntryCount = 30;

    private int selectedIndexQuantity;
    public int SelectedIndexQuantity
    {
        get { return selectedIndexQuantity; }
        set 
        {
            selectedIndexQuantity = value;
            _maxEntryCount = value switch
            {
                0 => 30,
                1 => 50,
                2 => int.MaxValue,
                _ => 30,
            };
            FilterInfoCenterEntrys(SelectedIndexInfoCenterTyp);
        }
    }

    private int selectedIndexInfoCenterTyp;
    public int SelectedIndexInfoCenterTyp
    {
        get { return selectedIndexInfoCenterTyp; }
        set
        {
            selectedIndexInfoCenterTyp = value;
            FilterInfoCenterEntrys(SelectedIndexInfoCenterTyp);
        }
    }

    public ObservableCollection<InfoCenterEntry> InfoCenterEntrys
    {
        get { return (ObservableCollection<InfoCenterEntry>)GetValue(InfoCenterEntrysProperty); }
        set
        {
            SetValue(InfoCenterEntrysProperty, value);
            InfoCenterEntrys.CollectionChanged += InfoCenterEntrys_CollectionChanged;
            InfoCenterEntrysView = new CollectionViewSource() { IsSourceGrouped = false};
            FilterInfoCenterEntrys(SelectedIndexInfoCenterTyp);
        }
    }

    private void InfoCenterEntrys_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (sender != null && sender is ObservableCollection<InfoCenterEntry>)
        {
            btn_ClearEntrys.IsEnabled = ((ObservableCollection<InfoCenterEntry>)sender).Count > 0;
            FilterInfoCenterEntrys(SelectedIndexInfoCenterTyp);
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
        set 
        {
            SetValue(InfoCenterIsOpenProperty, value);
            FilterInfoCenterEntrys(SelectedIndexInfoCenterTyp);
        }
    }

    public static readonly DependencyProperty InfoCenterIsOpenProperty =
        DependencyProperty.Register(nameof(InfoCenterIsOpen), typeof(bool), typeof(SidebarPanel), new PropertyMetadata(false));

    private void ClearEntrys_Click(object sender, RoutedEventArgs e)
    {
        InfoCenterEntrys.Clear();
    }

    private void FilterInfoCenterEntrys(int infoCenterTyp)
    {
        if (!InfoCenterIsOpen) return;

        if (infoCenterTyp < 1 || infoCenterTyp > 6)
        {
            InfoCenterEntrysView.Source = InfoCenterEntrys.OrderByDescending(x => x.TimeStamp).Take(_maxEntryCount);
        }
        else
        {
            InfoCenterEntrysView.Source = InfoCenterEntrys.OrderByDescending(x => x.TimeStamp).Where(y => y.State == infoCenterTyp).Take(_maxEntryCount);
        }
    }
}
