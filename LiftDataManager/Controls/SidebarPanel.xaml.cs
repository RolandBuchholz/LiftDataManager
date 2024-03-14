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
            InfoCenterEntrysView.Filter = value switch
            {
                0 => x => x != null,
                1 => x => ((InfoCenterEntry)x).State.Value == 1,
                2 => x => x != null,
                _ => x => x != null,
            };
        }
    }

    private int selectedIndexInfoCenterTyp;
    public int SelectedIndexInfoCenterTyp
    {
        get { return selectedIndexInfoCenterTyp; }
        set
        {
            selectedIndexInfoCenterTyp = value;
            InfoCenterEntrysView.Filter = value switch
            {
                0 => x => x != null,
                1 => x => ((InfoCenterEntry)x).State.Value == 1,
                2 => x => ((InfoCenterEntry)x).State.Value == 2,
                3 => x => ((InfoCenterEntry)x).State.Value == 3,
                4 => x => ((InfoCenterEntry)x).State.Value == 4,
                5 => x => ((InfoCenterEntry)x).State.Value == 5,
                _ => x => x != null,
            };
        }
    }

    public ObservableCollection<InfoCenterEntry> InfoCenterEntrys
    {
        get { return (ObservableCollection<InfoCenterEntry>)GetValue(InfoCenterEntrysProperty); }
        set
        {
            SetValue(InfoCenterEntrysProperty, value);
            InfoCenterEntrysView = new AdvancedCollectionView(value, true);
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

    private void ClearEntrys_Click(object sender, RoutedEventArgs e)
    {
        InfoCenterEntrys.Clear();
    }
}
