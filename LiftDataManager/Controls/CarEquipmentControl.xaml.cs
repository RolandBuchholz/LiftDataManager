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
        CarDepth = LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_KTI");
        CarWidth = LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_KBI");
        CarHeightRaw = LiftParameterHelper.GetLiftParameterValue<double>(ItemSource, "var_KHRoh");
        _scale = 0.25f;
        ViewBoxWidth = Side == CarSide.A || Side == CarSide.C ? CarWidth * _scale : CarDepth * _scale;
        ViewBoxHeight = CarHeightRaw * _scale;
    }

    float _scale;
    public double CarWidth { get; set; }
    public double CarDepth { get; set; }
    public double CarHeightRaw { get; set; }

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

    public Dictionary<string, float> CarEquipmentDataBaseData
    {
        get { return (Dictionary<string, float>)GetValue(CarEquipmentDataBaseDataProperty); }
        set { SetValue(CarEquipmentDataBaseDataProperty, value); }
    }

    public static readonly DependencyProperty CarEquipmentDataBaseDataProperty =
        DependencyProperty.Register(nameof(CarEquipmentDataBaseData), typeof(Dictionary<string, float>), typeof(EntranceControl), new PropertyMetadata(null));

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        //SKImageInfo info = e.Info;
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear();
        canvas.Scale(_scale);
        DrawWall(canvas);
        DrawSkirtingBoard(canvas);
        DrawMirror(canvas);
        DrawHandrail(canvas);
        DrawDivisionBar(canvas);
        DrawRammingProtection(canvas);
        DrawCarDoor(canvas);
    }

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
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, $"var_ZUGANSSTELLEN_{Side}"))
            return;

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
                using (SKPaint paintText = new())
                {
                    paintText.Style = SKPaintStyle.Fill;
                    paintText.IsAntialias = true;
                    paintText.Color = SKColors.IndianRed;
                    paintText.TextAlign = SKTextAlign.Center;
                    paintText.FakeBoldText = true;
                    paintText.TextSize = 200;

                    for (int i = 0; i < doorPanelCount; i++)
                    {
                        canvas.DrawText("?", doorDistanceR + panelWidth / 2 + panelWidth * i, (float)CarHeightRaw - doorHeight / 1.5f, paintText);
                    }
                }
                break;
            case "einseitig öffnend (rechts)":
                for (int i = 0; i < doorPanelCount; i++)
                {
                    for (int j = 0; j < doorPanelCount - i; j++)
                    {
                        float startOffset = doorPanelCount - i * 75;
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
        float skirtingHeight = CarEquipmentDataBaseData["SkirtingBoardHeight"];
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
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, $"var_Spiegel{Side}"))
            return;
        List<string> mirrors = [];

        if (LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, "var_SpiegelA"))
            mirrors.Add("A");
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, "var_SpiegelB"))
            mirrors.Add("B");
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, "var_SpiegelC"))
            mirrors.Add("C");
        if (LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, "var_SpiegelD"))
            mirrors.Add("D");

        var indexOfMirror = mirrors.IndexOf(Side.ToString());
        float mirrorWidth = indexOfMirror == 0 ? LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_BreiteSpiegel")
                                               : LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, $"var_BreiteSpiegel{indexOfMirror + 1}");
        float mirrorHeight = indexOfMirror == 0 ? LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_HoeheSpiegel")
                                                : LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, $"var_HoeheSpiegel{indexOfMirror + 1}");
        float spacingCeiling = indexOfMirror == 0 ? LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_AbstandSpiegelDecke")
                                                  : LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, $"var_AbstandSpiegelDecke{indexOfMirror + 1}");
        float spacingLeftWall = indexOfMirror == 0 ? LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_AbstandSpiegelvonLinks")
                                                   : LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, $"var_AbstandSpiegelvonLinks{indexOfMirror + 1}");
        SKRect mirror = new()
        {
            Size = new SKSize(mirrorWidth, mirrorHeight),
            Location = new SKPoint(spacingLeftWall, spacingCeiling)
        };

        using var paint = new SKPaint
        {
            Shader = SKShader.CreateLinearGradient(
                                new SKPoint(mirror.Left, mirror.Top),
                                new SKPoint(mirror.Right, mirror.Bottom),
                                [SKColors.LightBlue, SKColors.FloralWhite],
                                [0, 1],
                                SKShaderTileMode.Repeat),
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
        canvas.DrawRect(mirror, paint);
        canvas.DrawRect(mirror, paintStrokeSmall);
    }

    private void DrawHandrail(SKCanvas canvas)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, $"var_Handlauf{Side}"))
            return;
        float handrailHeightFFB = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_HoeheHandlauf");
        float width = Side == CarSide.A || Side == CarSide.C ? (float)CarWidth : (float)CarDepth;
        float handrailHeight = CarEquipmentDataBaseData["HandrailHeight"];

        using var paint = new SKPaint
        {
            Color = SKColors.DarkGray,
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
        canvas.DrawRect(10f, (float)CarHeightRaw - handrailHeightFFB, width - 20f, handrailHeight, paint);
        canvas.DrawRect(10f, (float)CarHeightRaw - handrailHeightFFB, width - 20f, handrailHeight, paintStrokeSmall);
    }

    private void DrawRammingProtection(SKCanvas canvas)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, $"var_Rammschutz{Side}"))
            return;
        float rammingProtectionHeightFFB1 = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_HoeheRammschutz");
        float rammingProtectionHeightFFB2 = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_HoeheRammschutz2");
        float rammingProtectionHeightFFB3 = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_HoeheRammschutz3");

        float width = Side == CarSide.A || Side == CarSide.C ? (float)CarWidth : (float)CarDepth;
        float rammingProtectionHeight = CarEquipmentDataBaseData["RammingProtectionHeight"];

        using var paint = new SKPaint
        {
            Color = SKColors.DarkGray,
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
        if (rammingProtectionHeightFFB1 > 0)
        {
            canvas.DrawRect(10f, (float)CarHeightRaw - rammingProtectionHeightFFB1, width - 20f, rammingProtectionHeight, paint);
            canvas.DrawRect(10f, (float)CarHeightRaw - rammingProtectionHeightFFB1, width - 20f, rammingProtectionHeight, paintStrokeSmall);
        }
        if (rammingProtectionHeightFFB2 > 0)
        {
            canvas.DrawRect(10f, (float)CarHeightRaw - rammingProtectionHeightFFB2, width - 20f, rammingProtectionHeight, paint);
            canvas.DrawRect(10f, (float)CarHeightRaw - rammingProtectionHeightFFB2, width - 20f, rammingProtectionHeight, paintStrokeSmall);
        }
        if (rammingProtectionHeightFFB3 > 0)
        {
            canvas.DrawRect(10f, (float)CarHeightRaw - rammingProtectionHeightFFB3, width - 20f, rammingProtectionHeight, paint);
            canvas.DrawRect(10f, (float)CarHeightRaw - rammingProtectionHeightFFB3, width - 20f, rammingProtectionHeight, paintStrokeSmall);
        }
    }

    private void DrawDivisionBar(SKCanvas canvas)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ItemSource, $"var_Teilungsleiste{Side}"))
            return;
        float divisionBarHeightFFB = LiftParameterHelper.GetLiftParameterValue<float>(ItemSource, "var_TeilungsleisteOKFF");
        float width = Side == CarSide.A || Side == CarSide.C ? (float)CarWidth : (float)CarDepth;
        float divisionBarHeight = 40f;

        using var paint = new SKPaint
        {
            Color = SKColors.DarkGray,
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
        canvas.DrawRect(5f, (float)CarHeightRaw - divisionBarHeightFFB, width - 10f, divisionBarHeight, paint);
        canvas.DrawRect(5f, (float)CarHeightRaw - divisionBarHeightFFB, width - 10f, divisionBarHeight, paintStrokeSmall);
    }
}
