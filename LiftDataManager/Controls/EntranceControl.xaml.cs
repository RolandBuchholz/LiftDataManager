using Cogs.Collections;
using CommunityToolkit.WinUI.UI;
using LiftDataManager.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

namespace LiftDataManager.Controls
{
    public sealed partial class EntranceControl : UserControl
    {
        public string NewMainEntrance { get; private set; }

        public EntranceControl()
        {
            InitializeComponent();
        }

        public ObservableDictionary<string, Parameter> ItemSource
        {
            get { return (ObservableDictionary<string, Parameter>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(ObservableDictionary<string, Parameter>), typeof(EntranceControl), new PropertyMetadata(null));

        public ICommand SetHauptzugangCommand
        {
            get { return (ICommand)GetValue(SetHauptzugangCommandProperty); }
            set { SetValue(SetHauptzugangCommandProperty, value); }
        }

        public static readonly DependencyProperty SetHauptzugangCommandProperty =
            DependencyProperty.Register("SetHauptzugangCommand", typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));

        public ICommand ResetHauptzugangCommand
        {
            get { return (ICommand)GetValue(ResetHauptzugangCommandProperty); }
            set { SetValue(ResetHauptzugangCommandProperty, value); }
        }

        public static readonly DependencyProperty ResetHauptzugangCommandProperty =
            DependencyProperty.Register("ResetHauptzugangCommand", typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));

        public string DisplayNameHauptzugang
        {
            get
            {
                SetBorderHauptZugang();
                 return (string)GetValue(DisplayNameHauptzugangProperty);
            }
            set
            {
                SetValue(DisplayNameHauptzugangProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayNameHauptzugangProperty =
            DependencyProperty.Register("DisplayNameHauptzugang", typeof(string), typeof(CommandBar), new PropertyMetadata(string.Empty));

        private void Textbox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);

            e.Handled = true;
        }

        private void SetBorderHauptZugang()
        {
            if (ItemSource["var_Haupthaltestelle"].Value is null) return;

            NewMainEntrance = ItemSource["var_Haupthaltestelle"].Value;

            var allEntranceBoxes = grd_Entrance.FindChildren().Where(f => f.Name.StartsWith("txtBox_"));

            foreach (var entranceBox in allEntranceBoxes)
            {
                if (entranceBox is TextBox)
                {
                    ((TextBox)entranceBox).BorderThickness = new Thickness(0);
                }
            }

            if (NewMainEntrance != "NV")
            {
                TextBox borderBox = (TextBox)FindName("txtBox_" + ItemSource["var_Haupthaltestelle"].Value.Replace("ZG_", ""));
                borderBox.BorderThickness = new Thickness(2);
  
            }
        }
    }
}
