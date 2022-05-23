using LiftDataManager.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Views
{
    public sealed partial class ListenansichtDetailControl : UserControl
    {
        public Parameter ListDetailsMenuItem
        {
            get { return GetValue(ListDetailsMenuItemProperty) as Parameter; }
            set { SetValue(ListDetailsMenuItemProperty, value); }
        }
        public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(Parameter), typeof(ListenansichtDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

        public ListenansichtDetailControl()
        {
            InitializeComponent();
        }
        private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListenansichtDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
