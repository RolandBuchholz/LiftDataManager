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

    private void OnLoadCarDesignControl(object sender, RoutedEventArgs e)
    {
        ItemSource["var_KTI"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_KBI"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_L1"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_L2"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_L3"].PropertyChanged += CarDesign_PropertyChanged;
        ItemSource["var_L4"].PropertyChanged += CarDesign_PropertyChanged;
    }

    private void OnUnLoadCarDesignControl(object sender, RoutedEventArgs e)
    {
        ItemSource["var_KTI"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_KBI"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_L1"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_L2"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_L3"].PropertyChanged -= CarDesign_PropertyChanged;
        ItemSource["var_L4"].PropertyChanged -= CarDesign_PropertyChanged;
    }

    public double Kabinenbreite => !string.IsNullOrWhiteSpace(ItemSource["var_KBI"].Value) ? Convert.ToDouble(ItemSource["var_KBI"].Value, CultureInfo.CurrentCulture) : 0;
    public double Kabinentiefe => !string.IsNullOrWhiteSpace(ItemSource["var_KTI"].Value) ? Convert.ToDouble(ItemSource["var_KTI"].Value, CultureInfo.CurrentCulture) : 0;

    public double TuerbreiteA => !string.IsNullOrWhiteSpace(ItemSource["var_TB"].Value) ? Convert.ToDouble(ItemSource["var_TB"].Value, CultureInfo.CurrentCulture) : 0;
    public double TuerbreiteB => !string.IsNullOrWhiteSpace(ItemSource["var_TB_B"].Value) ? Convert.ToDouble(ItemSource["var_TB_B"].Value, CultureInfo.CurrentCulture) : 0;
    public double TuerbreiteC => !string.IsNullOrWhiteSpace(ItemSource["var_TB_C"].Value) ? Convert.ToDouble(ItemSource["var_TB_C"].Value, CultureInfo.CurrentCulture) : 0;
    public double TuerbreiteD => !string.IsNullOrWhiteSpace(ItemSource["var_TB_C"].Value) ? Convert.ToDouble(ItemSource["var_TB_C"].Value, CultureInfo.CurrentCulture) : 0;

    public double HalsL1 => !string.IsNullOrWhiteSpace(ItemSource["var_L1"].Value) ? Convert.ToDouble(ItemSource["var_L1"].Value, CultureInfo.CurrentCulture) : 0;
    public double HalsL2 => !string.IsNullOrWhiteSpace(ItemSource["var_L2"].Value) ? Convert.ToDouble(ItemSource["var_L2"].Value, CultureInfo.CurrentCulture) : 0;
    public double HalsL3 => !string.IsNullOrWhiteSpace(ItemSource["var_L3"].Value) ? Convert.ToDouble(ItemSource["var_L3"].Value, CultureInfo.CurrentCulture) : 0;
    public double HalsL4 => !string.IsNullOrWhiteSpace(ItemSource["var_L4"].Value) ? Convert.ToDouble(ItemSource["var_L4"].Value, CultureInfo.CurrentCulture) : 0;

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


    public ObservableDictionary<string, Parameter> ItemSource
    {
        get => (ObservableDictionary<string, Parameter>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register("ItemSource", typeof(ObservableDictionary<string, Parameter>), typeof(CarDesignControl), new PropertyMetadata(null));

    private void CarDesignGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateCarDesign();
    }

    private void CarDesign_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
