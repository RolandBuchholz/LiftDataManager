using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace LiftDataManager.Controls
{
    public sealed partial class CommandBar : UserControl
    {
        public CommandBar()
        {
            InitializeComponent();
        }

        public string FilterValue
        {
            get { return (string)GetValue(FilterValueProperty); }
            set { SetValue(FilterValueProperty, value); }
        }

        public static readonly DependencyProperty FilterValueProperty =
            DependencyProperty.Register("FilterValue", typeof(string), typeof(CommandBar), new PropertyMetadata(string.Empty));

        public string GroupingValue
        {
            get { return (string)GetValue(GroupingValueProperty); }
            set { SetValue(GroupingValueProperty, value); }
        }

        public static readonly DependencyProperty GroupingValueProperty =
            DependencyProperty.Register("GroupingValue", typeof(string), typeof(CommandBar), new PropertyMetadata(string.Empty));

        public string SearchInput
        {
            get { return (string)GetValue(SearchInputProperty); }
            set { SetValue(SearchInputProperty, value); }
        }

        public static readonly DependencyProperty SearchInputProperty =
            DependencyProperty.Register("SearchInput", typeof(string), typeof(CommandBar), new PropertyMetadata(string.Empty));

        public bool IsUnsavedParametersSelected
        {
            get { return (bool)GetValue(IsUnsavedParametersSelectedProperty); }
            set { SetValue(IsUnsavedParametersSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsUnsavedParametersSelectedProperty =
            DependencyProperty.Register("IsUnsavedParametersSelected", typeof(bool), typeof(CommandBar), new PropertyMetadata(false));

        public ICommand SaveAllCommand
        {
            get { return (ICommand)GetValue(SaveAllCommandProperty); }
            set { SetValue(SaveAllCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveAllCommandProperty =
            DependencyProperty.Register("SaveAllCommand", typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));

        public ICommand ShowUnsavedParametersCommand
        {
            get { return (ICommand)GetValue(ShowUnsavedParametersCommandProperty); }
            set { SetValue(ShowUnsavedParametersCommandProperty, value); }
        }

        public static readonly DependencyProperty ShowUnsavedParametersCommandProperty =
            DependencyProperty.Register("ShowUnsavedParametersCommand", typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));

        public ICommand ShowAllParametersCommand
        {
            get { return (ICommand)GetValue(ShowAllParametersCommandProperty); }
            set { SetValue(ShowAllParametersCommandProperty, value); }
        }

        public static readonly DependencyProperty ShowAllParametersCommandProperty =
            DependencyProperty.Register("ShowAllParametersCommand", typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));

        public ICommand SetParameterFilterCommand
        {
            get { return (ICommand)GetValue(SetParameterFilterCommandProperty); }
            set { SetValue(SetParameterFilterCommandProperty, value); }
        }

        public static readonly DependencyProperty SetParameterFilterCommandProperty =
            DependencyProperty.Register("SetParameterFilterCommand", typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));

        public ICommand GroupParameterCommand
        {
            get { return (ICommand)GetValue(GroupParameterCommandProperty); }
            set { SetValue(GroupParameterCommandProperty, value); }
        }

        public static readonly DependencyProperty GroupParameterCommandProperty =
            DependencyProperty.Register("GroupParameterCommand", typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));
    }
}
