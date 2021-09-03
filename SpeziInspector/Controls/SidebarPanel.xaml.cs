using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SpeziInspector.Controls
{
    public sealed partial class SidebarPanel : UserControl
    {
        public SidebarPanel()
        {
            InitializeComponent();
        }

        public string InfoText
        {
            get { return (string)GetValue(InfoTextProperty); }
            set 
            {
                SetValue(InfoTextProperty, value);
                InfoTextScroller.UpdateLayout();
                var scrollViewerHeight = InfoTextScroller.ScrollableHeight;
                InfoTextScroller.ChangeView(null, scrollViewerHeight, null);
                CanTextClear = InfoText.Length > 45;
            }
        }

        public static readonly DependencyProperty InfoTextProperty =
            DependencyProperty.Register("InfoText", typeof(string), typeof(SidebarPanel), new PropertyMetadata(string.Empty));

        public bool CanTextClear
        {
            get { return (bool)GetValue(CanTextClearProperty); }
            set { SetValue(CanTextClearProperty, value); }
        }

        public static readonly DependencyProperty CanTextClearProperty =
            DependencyProperty.Register("CanTextClear", typeof(bool), typeof(SidebarPanel), new PropertyMetadata(false));

        private void btn_ClearInfoText_Click(object sender, RoutedEventArgs e)
        {
            InfoText = "Info Sidebar Panel Text gelöscht\n----------\n";
            CanTextClear = false;
        }
    }
}
