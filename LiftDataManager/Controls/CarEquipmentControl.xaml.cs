using Cogs.Collections;
using SkiaSharp;
using SkiaSharp.Views.Windows;

namespace LiftDataManager.Controls;
public sealed partial class CarEquipmentControl : UserControl
{
    public CarEquipmentControl()
    {
        InitializeComponent();
        Loaded += CarEquipmentControl_Loaded;
    }

    private void CarEquipmentControl_Loaded(object sender, RoutedEventArgs e)
    {
        KT = 2100;
        KB = 1100;
        KH = 2100;
        ViewBoxWidth = Side == CarSide.A || Side == CarSide.C ? KB / 2 : KT / 2;
        ViewBoxHeight = KH / 2;
    }

    public double ViewBoxWidth
    {
        get { return (double)GetValue(ViewBoxWidthProperty); }
        set { SetValue(ViewBoxWidthProperty, value); }
    }

    public static readonly DependencyProperty ViewBoxWidthProperty =
        DependencyProperty.Register("ViewBoxWidth", typeof(double), typeof(CarEquipmentControl), new PropertyMetadata(0.0));

    public double ViewBoxHeight
    {
        get { return (double)GetValue(ViewBoxHeightProperty); }
        set { SetValue(ViewBoxHeightProperty, value); }
    }

    public static readonly DependencyProperty ViewBoxHeightProperty =
        DependencyProperty.Register("ViewBoxHeight", typeof(double), typeof(CarEquipmentControl), new PropertyMetadata(0.0));

    public double KB { get; set; }
    public double KT { get; set; }
    public double KH { get; set; }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear();
        DrawWall(canvas);
        //DrawEntrace(canvas);
    }

    public CarSide Side
    {
        get => (CarSide)GetValue(SideProperty);
        set => SetValue(SideProperty, value);
    }

    public static readonly DependencyProperty SideProperty =
        DependencyProperty.Register(nameof(Side), typeof(CarSide), typeof(CarEquipmentControl), new PropertyMetadata(CarSide.A));

    public ObservableDictionary<string, Parameter> ItemSource
    {
        get => (ObservableDictionary<string, Parameter>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register(nameof(ItemSource), typeof(ObservableDictionary<string, Parameter>), typeof(EntranceControl), new PropertyMetadata(null));

    private void RefreshView()
    {
        xamlCanvas.Invalidate();
    }

    private void DrawWall(SKCanvas canvas)
    {
        var width = Side == CarSide.A || Side == CarSide.C ? (float)KB  : (float)KH;

        using var paint = new SKPaint
        {
            Color = SKColors.LightGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRect(0, 0, width, (float)KH, paint);
    }
}
