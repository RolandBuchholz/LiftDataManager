namespace LiftDataManager.Views
{
    public sealed partial class KabineDetailControlPanelPage : Page
    {
        public KabineDetailControlPanelViewModel ViewModel
        {
            get;
        }

        public KabineDetailControlPanelPage()
        {
            ViewModel = App.GetService<KabineDetailControlPanelViewModel>();
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
