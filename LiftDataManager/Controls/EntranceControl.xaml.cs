using System.Windows.Input;
using Cogs.Collections;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace LiftDataManager.Controls;

public sealed partial class EntranceControl : UserControl
{
    public string NewMainEntrance
    {
        get; private set;
    }

    public EntranceControl()
    {
        InitializeComponent();
    }

    public ObservableDictionary<string, Parameter> ItemSource
    {
        get => (ObservableDictionary<string, Parameter>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register("ItemSource", typeof(ObservableDictionary<string, Parameter>), typeof(EntranceControl), new PropertyMetadata(null));

    public ICommand SetHauptzugangCommand
    {
        get => (ICommand)GetValue(SetHauptzugangCommandProperty);
        set => SetValue(SetHauptzugangCommandProperty, value);
    }

    public static readonly DependencyProperty SetHauptzugangCommandProperty =
        DependencyProperty.Register("SetHauptzugangCommand", typeof(ICommand), typeof(CommandBar), new PropertyMetadata(null));

    public ICommand ResetHauptzugangCommand
    {
        get => (ICommand)GetValue(ResetHauptzugangCommandProperty);
        set => SetValue(ResetHauptzugangCommandProperty, value);
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
        set => SetValue(DisplayNameHauptzugangProperty, value);
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

            var borderBox = (TextBox)FindName("txtBox_" + ItemSource["var_Haupthaltestelle"].Value.Replace("ZG_", ""));
            borderBox.BorderThickness = new Thickness(2);
            object accentColor;
            var accentColorFound = Application.Current.Resources.ThemeDictionaries.TryGetValue("SystemAccentColor", out accentColor);
            if (accentColorFound)
            {
                borderBox.BorderBrush = new SolidColorBrush((Color)accentColor);
            }

        }
    }

    //Workaround broken binding usercontrol

    public bool? ZugangA
    {
        get
        {
            var value = ItemSource["var_ZUGANSSTELLEN_A"].Value;
            ZugangA = ConvertStingToBool(value);
            return (bool)GetValue(ZugangAProperty);
        }
        set
        {
            ItemSource["var_ZUGANSSTELLEN_A"].Value = ConvertBoolToString(value);
            SetValue(ZugangAProperty, value);
        }
    }

    public static readonly DependencyProperty ZugangAProperty =
        DependencyProperty.Register("ZugangA", typeof(bool), typeof(EntranceControl), new PropertyMetadata(false));

    public bool? ZugangB
    {
        get
        {
            var value = ItemSource["var_ZUGANSSTELLEN_B"].Value;
            ZugangB = ConvertStingToBool(value);
            return (bool)GetValue(ZugangBProperty);
        }
        set
        {
            ItemSource["var_ZUGANSSTELLEN_B"].Value = ConvertBoolToString(value);
            SetValue(ZugangBProperty, value);
        }
    }

    public static readonly DependencyProperty ZugangBProperty =
        DependencyProperty.Register("ZugangB", typeof(bool), typeof(EntranceControl), new PropertyMetadata(false));

    public bool? ZugangC
    {
        get
        {
            var value = ItemSource["var_ZUGANSSTELLEN_C"].Value;
            ZugangC = ConvertStingToBool(value);
            return (bool)GetValue(ZugangCProperty);
        }
        set
        {
            ItemSource["var_ZUGANSSTELLEN_C"].Value = ConvertBoolToString(value);
            SetValue(ZugangCProperty, value);
        }
    }

    public static readonly DependencyProperty ZugangCProperty =
        DependencyProperty.Register("ZugangC", typeof(bool), typeof(EntranceControl), new PropertyMetadata(false));

    public bool? ZugangD
    {
        get
        {
            var value = ItemSource["var_ZUGANSSTELLEN_D"].Value;
            ZugangD = ConvertStingToBool(value);
            return (bool)GetValue(ZugangDProperty);
        }
        set
        {
            ItemSource["var_ZUGANSSTELLEN_D"].Value = ConvertBoolToString(value);
            SetValue(ZugangDProperty, value);
        }
    }

    public static readonly DependencyProperty ZugangDProperty =
        DependencyProperty.Register("ZugangD", typeof(bool), typeof(EntranceControl), new PropertyMetadata(false));



    private bool? ConvertStingToBool(string value)
    {
        if (value == null || value.GetType() != typeof(string) || string.IsNullOrWhiteSpace((string)value))
        {
            return null;
        }

        if (string.Equals((string)value, "true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        else if (string.Equals((string)value, "false", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        else
        {
            Debug.WriteLine($"string: {value} could not be converted to a bool");
            return null;
        }
    }


    private string ConvertBoolToString(bool? value)
    {
        if (value == null || value.GetType() != typeof(bool))
        {
            return string.Empty;
        }

        return ((bool)value) ? "true" : "false";
    }


}
