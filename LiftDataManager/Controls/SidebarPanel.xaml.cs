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
    }

    public CollectionViewSource ViewSource
    {
        get => (CollectionViewSource)GetValue(ViewSourceProperty);
        set => SetValue(ViewSourceProperty, value);
    }

    public static readonly DependencyProperty ViewSourceProperty =
        DependencyProperty.Register(nameof(ViewSource), typeof(CollectionViewSource), typeof(SidebarPanel), new PropertyMetadata(null));

    public ObservableCollection<InfoCenterEntry> InfoCenterEntrys
    {
        get { return (ObservableCollection<InfoCenterEntry>)GetValue(InfoCenterEntrysProperty); }
        set { SetValue(InfoCenterEntrysProperty, value); }
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
