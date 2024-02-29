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
        CarDepth = LiftParameterHelper.GetLiftParameterValue<double>(ItemSource,"var_KTI");
        CarWidth = LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_KBI");
        CarHeightRaw = LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_KHRoh");
        ViewBoxWidth = Side == CarSide.A || Side == CarSide.C ? CarWidth : CarDepth;
        ViewBoxHeight = CarHeightRaw;
    }

    public double ViewBoxWidth
    {
        get { return (double)GetValue(ViewBoxWidthProperty); }
        set { SetValue(ViewBoxWidthProperty, value); }
    }

    public static readonly DependencyProperty ViewBoxWidthProperty =
        DependencyProperty.Register(nameof(ViewBoxWidth), typeof(double), typeof(CarEquipmentControl), new PropertyMetadata(0.0));

    public double ViewBoxHeight
    {
        get { return (double)GetValue(ViewBoxHeightProperty); }
        set { SetValue(ViewBoxHeightProperty, value); }
    }

    public static readonly DependencyProperty ViewBoxHeightProperty =
        DependencyProperty.Register(nameof(ViewBoxHeight), typeof(double), typeof(CarEquipmentControl), new PropertyMetadata(0.0));

    public double CarWidth { get; set; }
    public double CarDepth { get; set; }
    public double CarHeightRaw { get; set; }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear();
        DrawWall(canvas);
        DrawSkirtingBoard(canvas);
        DrawCarDoor(canvas);
        DrawMirror(canvas);
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

    public void RefreshView()
    {
        xamlCanvas.Invalidate();
    }
    private void DrawWall(SKCanvas canvas)
    {
        var width = Side == CarSide.A || Side == CarSide.C ? (float)CarWidth : (float)CarDepth;

        using var paint = new SKPaint
        {
            Color = SKColors.LightGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRect(0, 0, width, (float)CarHeightRaw, paint);
    }
    private void DrawCarDoor(SKCanvas canvas)
    {
       if (!LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, $"var_ZUGANSSTELLEN_{Side}")) return;

        float doorHeight = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, Side.ToString() == "A" ? "var_TH" : $"var_TH_{Side}");
        float doorWidth = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, Side.ToString() == "A" ? "var_TB" : $"var_TB_{Side}");
        string dooropening = LiftParameterHelper.GetLiftParameterValue<string>(ItemSource, Side.ToString() == "A" ? "var_Tueroeffnung" : $"var_Tueroeffnung_{Side}");
        var doorDistanceRString = Side switch
        {
            CarSide.A => "var_R1",
            CarSide.B => "var_R3",
            CarSide.C => "var_R2",
            CarSide.D => "var_R4",
            _ => "var_R1",
        };
        float doorDistanceR = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, doorDistanceRString);
        int doorPanelCount = LiftParameterHelper.GetLiftParameterValue<int>(ItemSource, Side.ToString() == "A" ? "var_AnzahlTuerfluegel" : $"var_AnzahlTuerfluegel_{Side}");
        float panelWidth = doorWidth / doorPanelCount;

        using var paint = new SKPaint
        {
            Color = SKColors.DarkGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
        };
        using var paintStroke = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            IsStroke = true,
            StrokeWidth = 20,
            Style = SKPaintStyle.Stroke
        };
        using var paintStrokeSmall = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            IsStroke = true,
            StrokeWidth = 10,
            Style = SKPaintStyle.Stroke
        };
        using var paintGreen = new SKPaint
        {
            Color = SKColors.GreenYellow,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
        };
        canvas.DrawRect(doorDistanceR, (float)CarHeightRaw, doorWidth, -doorHeight, paint);
        canvas.DrawRect(doorDistanceR, (float)CarHeightRaw, doorWidth, -doorHeight, paintStroke);

        if (doorPanelCount > 1)
        {
            for (int i = 1; i < doorPanelCount; i++)
            {
                canvas.DrawLine(doorDistanceR + panelWidth * i, (float)CarHeightRaw, doorDistanceR + panelWidth * i, (float)CarHeightRaw - doorHeight, paintStrokeSmall);
            }
        }

        var arrow = new SKPath();

        switch (dooropening)
        {
            case "einseitig öffnend":
                using (SKPaint paintText = new ())
                {
                    paintText.Style = SKPaintStyle.Fill;
                    paintText.IsAntialias = true;
                    paintText.Color = SKColors.IndianRed;
                    paintText.TextAlign = SKTextAlign.Center;
                    paintText.FakeBoldText = true;
                    paintText.TextSize = 200;

                    for (int i = 0; i < doorPanelCount; i++)
                    {
                        canvas.DrawText("?", doorDistanceR + panelWidth/2 + panelWidth * i, (float)CarHeightRaw - doorHeight/1.5f, paintText);
                    }   
                }
                break;
            case "einseitig öffnend (rechts)":
                for (int i = 0; i < doorPanelCount; i++)
                {
                    for (int j = 0; j < doorPanelCount-i; j++)
                    {
                        float startOffset = doorPanelCount-i*75;
                        float arrowOffset = startOffset - j * 150;
                        arrow = SkiaSharpHelpers.CreateArrow(doorDistanceR - panelWidth / 2 + doorPanelCount * panelWidth - panelWidth * i, (float)CarHeightRaw - doorHeight / 1.5f + arrowOffset, 180);
                        canvas.DrawPath(arrow, paintGreen);
                        canvas.DrawPath(arrow, paintStrokeSmall);
                    }
                }
                break;
            case "einseitig öffnend (links)":
                for (int i = 0; i < doorPanelCount; i++)
                {
                    for (int j = 0; j < doorPanelCount - i; j++)
                    {
                        float startOffset = doorPanelCount - i * 75;
                        float arrowOffset = startOffset - j * 150;
                        arrow = SkiaSharpHelpers.CreateArrow(doorDistanceR + panelWidth / 2 + panelWidth * i, (float)CarHeightRaw - doorHeight / 1.5f + arrowOffset, 0);
                        canvas.DrawPath(arrow, paintGreen);
                        canvas.DrawPath(arrow, paintStrokeSmall);
                    }
                }
                break;
            case "zentral öffnend":
                for (int i = 0; i < doorPanelCount / 2; i++)
                {
                    for (int j = 0; j < doorPanelCount / 2 - i; j++)
                    {
                        float startOffset = doorPanelCount - i * 75;
                        float arrowOffset = startOffset - j * 150;
                        arrow = SkiaSharpHelpers.CreateArrow(doorDistanceR + doorWidth / 2 + panelWidth / 2 + panelWidth * i, (float)CarHeightRaw - doorHeight / 1.5f + arrowOffset, 0);
                        canvas.DrawPath(arrow, paintGreen);
                        canvas.DrawPath(arrow, paintStrokeSmall);
                    }
                }
                for (int i = 0; i < doorPanelCount / 2; i++)
                {
                    for (int j = 0; j < doorPanelCount / 2 - i; j++)
                    {
                        float startOffset = doorPanelCount - i * 75;
                        float arrowOffset = startOffset - j * 150;
                        arrow = SkiaSharpHelpers.CreateArrow(doorDistanceR - doorWidth / 2 - panelWidth / 2 + doorPanelCount * panelWidth - panelWidth * i, (float)CarHeightRaw - doorHeight / 1.5f + arrowOffset, 180);
                        canvas.DrawPath(arrow, paintGreen);
                        canvas.DrawPath(arrow, paintStrokeSmall);
                    }
                }
                break;
            default:
                break;
        }
    }
    private void DrawSkirtingBoard(SKCanvas canvas)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, $"var_Sockelleiste{Side}"))
            return;
        float skirtingHeightFFB = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_SockelleisteOKFF");
        float width = Side == CarSide.A || Side == CarSide.C ? (float)CarWidth : (float)CarDepth;
        string skirting = LiftParameterHelper.GetLiftParameterValue<string>(ItemSource, "var_Sockelleiste");
        if (string.IsNullOrWhiteSpace(skirting))
            return;
        var skirtingHeightString = skirting.Replace("V2A","").Replace("V4A","").Split("x").FirstOrDefault()?.Trim();
        if (!float.TryParse(skirtingHeightString, out float skirtingHeight))
            return;

        using var paint = new SKPaint
        {
            Color = SKColors.DimGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
        };
        using var paintStrokeSmall = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            IsStroke = true,
            StrokeWidth = 10,
            Style = SKPaintStyle.Stroke
        };
        canvas.DrawRect(0, (float)CarHeightRaw - skirtingHeightFFB, width, skirtingHeight, paint);
        canvas.DrawRect(0, (float)CarHeightRaw - skirtingHeightFFB, width, skirtingHeight, paintStrokeSmall);
    }

    private void DrawMirror(SKCanvas canvas)
    {
        //if (!LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, $"var_Sockelleiste{Side}"))
        //    return;
        //float skirtingHeightFFB = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_SockelleisteOKFF");
        //float width = Side == CarSide.A || Side == CarSide.C ? (float)CarWidth : (float)CarDepth;
        //string skirting = LiftParameterHelper.GetLiftParameterValue<string>(ItemSource, "var_Sockelleiste");
        //if (string.IsNullOrWhiteSpace(skirting))
        //    return;
        //var skirtingHeightString = skirting.Replace("V2A", "").Replace("V4A", "").Split("x").FirstOrDefault()?.Trim();
        //if (!float.TryParse(skirtingHeightString, out float skirtingHeight))
        //    return;

        //using var paint = new SKPaint
        //{
        //    Color = SKColors.DimGray,
        //    IsAntialias = true,
        //    Style = SKPaintStyle.Fill,
        //};
        //using var paintStrokeSmall = new SKPaint
        //{
        //    Color = SKColors.Black,
        //    IsAntialias = true,
        //    IsStroke = true,
        //    StrokeWidth = 10,
        //    Style = SKPaintStyle.Stroke
        //};
        //canvas.DrawRect(0, (float)CarHeightRaw - skirtingHeightFFB, width, skirtingHeight, paint);
        //canvas.DrawRect(0, (float)CarHeightRaw - skirtingHeightFFB, width, skirtingHeight, paintStrokeSmall);
    }
}
