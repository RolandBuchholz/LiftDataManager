using LiftDataManager.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace LiftDataManager.Views
{
    public sealed partial class ParameterDetailControl : UserControl
    {
        public ParameterDetailControl()
        {
            InitializeComponent();
        }

        public Parameter ListDetailsMenuItem
        {
            get { return GetValue(ListDetailsMenuItemProperty) as Parameter; }
            set { SetValue(ListDetailsMenuItemProperty, value); }
        }
        public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(Parameter), typeof(ParameterDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(ParameterDetailControl), new PropertyMetadata(null));


        private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ParameterDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
