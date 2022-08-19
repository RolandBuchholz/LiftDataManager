using System.ComponentModel;
using Cogs.Collections;

namespace LiftDataManager.Controls;

public sealed partial class CarDesignControl : UserControl
{
    public CarDesignControl()
    {
        InitializeComponent();
        Loaded += OnLoadCarDesignControl;
        Unloaded += OnUnLoadCarDesignControl;
    }

    private void OnLoadCarDesignControl(object? sender, RoutedEventArgs e)
    {
        ItemSource["var_KTI"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_KBI"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_L1"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_L2"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_L3"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_L4"].PropertyChanged += CarDesign_PropertyChanged;
    }

    private void OnUnLoadCarDesignControl(object? sender, RoutedEventArgs e)
    {
        ItemSource["var_KTI"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_KBI"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_L1"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_L2"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_L3"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_L4"].PropertyChanged -= CarDesign_PropertyChanged;
    }

    public double Kabinenbreite => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_KBI");
    public double Kabinentiefe => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_KTI");

    public double TuerbreiteA => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_TB");
    public double TuerbreiteB => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_TB_B");
    public double TuerbreiteC => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_TB_C");
    public double TuerbreiteD => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_TB_D");

    public double HalsL1 => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_L1");
    public double HalsL2 => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_L2");
    public double HalsL3 => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_L3");
    public double HalsL4 => LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_L4");


    public double DisplayWidthCar
    {
        get => (double)GetValue(DisplayWidthCarProperty);
        set => SetValue(DisplayWidthCarProperty, value);
    }

    public static readonly DependencyProperty DisplayWidthCarProperty =
        DependencyProperty.Register("DisplayWidthCar", typeof(double), typeof(CarDesignControl), new PropertyMetadata((double)250));

    public double DisplayHeightCar
    {
        get => (double)GetValue(DisplayHeightCarProperty);
        set => SetValue(DisplayHeightCarProperty, value);
    }

    public static readonly DependencyProperty DisplayHeightCarProperty =
        DependencyProperty.Register("DisplayHeightCar", typeof(double), typeof(CarDesignControl), new PropertyMetadata((double)250));

    public bool ShowEntranceButtons
    {
        get => (bool)GetValue(ShowEntranceButtonsProperty);
        set => SetValue(ShowEntranceButtonsProperty, value);
    }

    public static readonly DependencyProperty ShowEntranceButtonsProperty =
        DependencyProperty.Register("ShowEntranceButtons", typeof(bool), typeof(CarDesignControl), new PropertyMetadata(false));

    public ObservableDictionary<string, Parameter> ItemSource
    {
        get => (ObservableDictionary<string, Parameter>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register("ItemSource", typeof(ObservableDictionary<string, Parameter>), typeof(CarDesignControl), new PropertyMetadata(null));

    private void CarDesignGrid_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateCarDesign();
    }

    private void CarDesign_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        UpdateCarDesign();
    }

    private void UpdateCarDesign()
    {
        if (Kabinenbreite > 300 && Kabinentiefe > 300)
        {
            var scalingfactorWidth = (CarDesignGrid.ActualWidth - 115) / Kabinenbreite;
            var scalingfactorHeight = (CarDesignGrid.ActualHeight - 115) / Kabinentiefe;
            var scalingfactor = scalingfactorWidth < scalingfactorHeight ? scalingfactorWidth : scalingfactorHeight;

            DisplayWidthCar = Kabinenbreite * scalingfactor;
            DisplayHeightCar = Kabinentiefe * scalingfactor;

            BorderA.Width = TuerbreiteA > 0 ? TuerbreiteA * scalingfactor : 180;
            BorderB.Height = TuerbreiteB > 0 ? TuerbreiteB * scalingfactor : 180;
            BorderC.Width = TuerbreiteC > 0 ? TuerbreiteC * scalingfactor : 180;
            BorderD.Height = TuerbreiteD > 0 ? TuerbreiteD * scalingfactor : 180;

            BorderA.Margin = new Thickness(HalsL1 * scalingfactor, 0, 0, 0);
            BorderB.Margin = new Thickness(0, 0, 0, HalsL3 * scalingfactor);
            BorderC.Margin = new Thickness(0, 0, HalsL2 * scalingfactor, 0);
            BorderD.Margin = new Thickness(0, HalsL4 * scalingfactor, 0, 0);
        }
        else
        {
            DisplayWidthCar = 250;
            DisplayHeightCar = 250;
        }
    }
}
