using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class SchachtDetailViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>, IRecipient<RefreshModelStateMessage>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;
    public ObservableCollection<string?> OpeningDirections { get; set; }

    public SchachtDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                                  ISettingService settingService, ILogger<DataViewModelBase> baseLogger, ParameterContext parametercontext, ICalculationsModule calculationsModuleService) :
                                  base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        _calculationsModuleService = calculationsModuleService;
        _parametercontext = parametercontext;
        OpeningDirections =
        [
            "einseitig öffnend",
            "einseitig öffnend (rechts)",
            "einseitig öffnend (links)"
        ];
    }

    private readonly string[] shaftDesignParameter = [ "var_ZUGANSSTELLEN_A", "var_ZUGANSSTELLEN_B",
                                                       "var_ZUGANSSTELLEN_C", "var_ZUGANSSTELLEN_D",
                                                       "var_MauerOeffnungBreiteA", "var_MauerOeffnungAbstandA",
                                                       "var_MauerOeffnungBreiteB", "var_MauerOeffnungAbstandB",
                                                       "var_MauerOeffnungBreiteC", "var_MauerOeffnungAbstandC",
                                                       "var_MauerOeffnungBreiteD", "var_MauerOeffnungAbstandD",
                                                       "var_Bausatzlage", "var_Spiegelbildlich", "var_MassD","var_Versatz_Y", "var_Versatz_X",
                                                       "var_KBI", "var_KTI",
                                                       "var_AbstandKabineA", "var_AbstandKabineD",
                                                       "var_L1","var_L2","var_L3","var_L4",
                                                       "var_TB","var_TB_B","var_TB_C","var_TB_D",
                                                       "var_TuerEinbau","var_TuerEinbauB","var_TuerEinbauC","var_TuerEinbauD",
                                                       "var_Tueroeffnung","var_Tueroeffnung_B","var_Tueroeffnung_C","var_Tueroeffnung_D" ];

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null||
            !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }

        if (message.PropertyName == "var_SB" || message.PropertyName == "var_ST")
        {
            SetViewBoxDimensions();
            SetWallOpeningOffsets();
        };

        if (shaftDesignParameter.Contains(message.PropertyName))
        {
            SetWallOpeningOffsets();
            SetCarFrameOffset(message.PropertyName);
            RefreshView();
        };
        SetInfoSidebarPanelText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex));
    }

    private float _scale;
    private float _stokeWith;

    public CarFrameType? CarFrameTyp { get; set; }

    public GuideRails? CarframeGuideRail { get; set; }

    public GuideRails? CounterWeightGuideRail { get; set; }

    [ObservableProperty]
    public partial double ViewBoxWidth { get; set; }

    [ObservableProperty]
    public partial double ViewBoxHeight { get; set; }

    [ObservableProperty]
    public partial double ShaftWidth { get; set; }

    [ObservableProperty]
    public partial double ShaftDepth { get; set; }

    [ObservableProperty]
    public partial double CarDistanceWallA { get; set; }

    [ObservableProperty]
    public partial double CarDistanceWallB { get; set; }

    [ObservableProperty]
    public partial double CarDistanceWallC { get; set; }

    [ObservableProperty]
    public partial double CarDistanceWallD { get; set; }

    public bool CanEditOpeningDirectionEntranceA => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_A") &&
                                                    ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung")).StartsWith("einseitig");
    public bool CanEditOpeningDirectionEntranceB => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_B") &&
                                                    ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_B")).StartsWith("einseitig");
    public bool CanEditOpeningDirectionEntranceC => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_C") &&
                                                    ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_C")).StartsWith("einseitig");
    public bool CanEditOpeningDirectionEntranceD => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_D") &&
                                                    ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_D")).StartsWith("einseitig");

    [ObservableProperty]
    public partial double OffsetWallOpeningShaftDoorA { get; set; }

    [ObservableProperty]
    public partial double OffsetWallOpeningShaftDoorB { get; set; }

    [ObservableProperty]
    public partial double OffsetWallOpeningShaftDoorC { get; set; }

    [ObservableProperty]
    public partial double OffsetWallOpeningShaftDoorD { get; set; }

    [ObservableProperty]
    public partial bool OpeningDirectionNotSelected { get; set; }

    [ObservableProperty]
    public partial bool ShowCarFrameOffsetInfoVertikal { get; set; }

    [ObservableProperty]
    public partial bool ShowCarFrameOffsetInfoHorizontal { get; set; }

    [ObservableProperty]
    public partial bool IsCarFrameOffsetXEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsFrameToCarOffsetEnabled { get; set; }

    [ObservableProperty]
    public partial string? OpeningDirectionA { get; set; }
    partial void OnOpeningDirectionAChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary["var_Tueroeffnung"], value);
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public partial string? OpeningDirectionB { get; set; }
    partial void OnOpeningDirectionBChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary["var_Tueroeffnung_B"], value);
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public partial string? OpeningDirectionC { get; set; }
    partial void OnOpeningDirectionCChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary["var_Tueroeffnung_C"], value);
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    public partial string? OpeningDirectionD { get; set; }
    partial void OnOpeningDirectionDChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            LiftParameterHelper.UpdateParameterDropDownListValue(ParameterDictionary["var_Tueroeffnung_D"], value);
            CheckIsOpeningDirectionSelected();
        }
    }

    private void CheckIsOpeningDirectionSelected()
    {
        OpeningDirectionNotSelected = (!string.IsNullOrWhiteSpace(OpeningDirectionA) && string.Equals(OpeningDirectionA, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(OpeningDirectionB) && string.Equals(OpeningDirectionB, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(OpeningDirectionC) && string.Equals(OpeningDirectionC, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(OpeningDirectionD) && string.Equals(OpeningDirectionD, "einseitig öffnend"));
    }

    private SKXamlCanvas? _xamlCanvas;

    [RelayCommand]
    private void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        SKImageInfo info = e.Info;
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        canvas.Clear();
        canvas.Translate(info.Width / 2, info.Height / 2);
        canvas.Scale(_scale);
        DrawShaftWall(canvas);
        DrawEntranceWall(canvas);
        DrawLiftCar(canvas);
        DrawCarFrame(canvas);
    }

    [RelayCommand]
    private void OnLoadedSKXamlCanvas(SKXamlCanvas xamlCanvas)
    {
        _xamlCanvas = xamlCanvas;
    }

    private void DrawShaftWall(SKCanvas canvas)
    {
        SKPathEffect diagLinesPath = SKPathEffect.Create2DLine(_stokeWith,
        SkiaSharpHelpers.Multiply(SKMatrix.CreateScale(50f, 50f), SKMatrix.CreateRotationDegrees(45f)));

        SKRect shaftWallOutSide = new()
        {
            Size = new SKSize((float)(ShaftWidth + 500), (float)(ShaftDepth + 500)),
            Location = new SKPoint(-(float)((ShaftWidth + 500) / 2), -(float)((ShaftDepth + 500) / 2))
        };

        SKRect shaftWallHatch = new()
        {
            Size = shaftWallOutSide.Size,
            Location = shaftWallOutSide.Location
        };

        shaftWallHatch.Inflate(50f, 50f);

        SKRect shaftWallInSide = new()
        {
            Size = new SKSize((float)ShaftWidth, (float)ShaftDepth),
            Location = new SKPoint(-(float)(ShaftWidth / 2), -(float)(ShaftDepth / 2))
        };

        using var paintWallSolid = new SKPaint
        {
            Color = SKColors.DimGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        using var paintHatch = new SKPaint
        {
            Color = SKColors.DarkRed,
            PathEffect = diagLinesPath,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
        using var paintHatchOutline = new SKPaint
        {
            Color = SKColors.DarkRed,
            IsAntialias = true,
            StrokeWidth = _stokeWith * 2,
            Style = SKPaintStyle.Stroke
        };
        using var paintShaft = new SKPaint
        {
            Color = SKColors.Gray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        canvas.DrawRect(shaftWallOutSide, paintWallSolid);
        canvas.DrawRect(shaftWallOutSide, paintHatchOutline);
        canvas.Save();
        canvas.ClipRect(shaftWallOutSide);
        canvas.DrawRect(shaftWallHatch, paintHatch);
        canvas.Restore();
        canvas.DrawRect(shaftWallInSide, paintShaft);
        canvas.DrawRect(shaftWallInSide, paintHatchOutline);

    }

    private void DrawEntranceWall(SKCanvas canvas)
    {
        string[] entrances = ["A", "B", "C", "D"];

        foreach (var entrance in entrances)
        {
            if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_ZUGANSSTELLEN_{entrance}"))
                continue;
            float wallOpeningWidth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_MauerOeffnungBreite{entrance}");
            if (wallOpeningWidth == 0)
                continue;
            float wallOpeningLeft = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_MauerOeffnungAbstand{entrance}");

            using var paintShaftEntrace = new SKPaint
            {
                Color = SKColors.LightSlateGray,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            using var paintOutline = new SKPaint
            {
                Color = SKColors.DarkRed,
                IsAntialias = true,
                StrokeWidth = _stokeWith * 2,
                Style = SKPaintStyle.Stroke
            };
            using var paintText = new SKPaint
            {
                Color = SKColors.DarkRed,
                IsAntialias = true,
            };

            using var font = new SKFont
            {
                Size = 200
            };

            float width = 0f;
            float height = 0f;
            float posX = 0f;
            float posY = 0f;

            switch (entrance)
            {
                case "A":
                    width = wallOpeningWidth;
                    height = 250f;
                    posX = -(float)ShaftWidth / 2 + wallOpeningLeft;
                    posY = (float)ShaftDepth / 2;
                    break;
                case "B":
                    width = 250f;
                    height = wallOpeningWidth;
                    posX = (float)ShaftWidth / 2;
                    posY = (float)ShaftDepth / 2 - wallOpeningWidth - wallOpeningLeft;
                    break;
                case "C":
                    width = wallOpeningWidth;
                    height = -250f;
                    posX = (float)ShaftWidth / 2 - wallOpeningWidth - wallOpeningLeft;
                    posY = -(float)ShaftDepth / 2;
                    break;
                case "D":
                    width = -250;
                    height = wallOpeningWidth;
                    posX = -(float)ShaftWidth / 2;
                    posY = -(float)ShaftDepth / 2 + wallOpeningLeft;
                    break;
                default:
                    continue;
            }

            SKRect entranceRect = new()
            {
                Size = new SKSize(width, height),
                Location = new SKPoint(posX, posY)
            };

            SKRect textRect = new();
            font.MeasureText(entrance, out textRect);

            canvas.DrawRect(entranceRect, paintShaftEntrace);
            canvas.DrawRect(entranceRect, paintOutline);
            canvas.DrawText(entrance, entranceRect.MidX, entranceRect.MidY - textRect.MidY, SKTextAlign.Center, font, paintText);
        }
    }

    private void DrawLiftCar(SKCanvas canvas)
    {
        double carWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KBI");
        double carDepth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KTI");
        double carWallA = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_AbstandKabineA");
        double carWallD = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_AbstandKabineD");

        float carR1 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_R1");
        float carR2 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_R2");
        float carR3 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_R3");
        float carR4 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_R4");

        float carDoorMountingA = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_TuerEinbau");
        float carDoorMountingB = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_TuerEinbauB");
        float carDoorMountingC = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_TuerEinbauC");
        float carDoorMountingD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_TuerEinbauD");

        float carDoorWidthA = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_TB");
        float carDoorWidthB = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_TB_B");
        float carDoorWidthC = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_TB_C");
        float carDoorWidthD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_TB_D");

        if (carWidth == 0 || carDepth == 0)
            return;
        CarDistanceWallA = Math.Round(carWallA - carDepth / 2, 2);
        CarDistanceWallD = Math.Round(carWallD - carWidth / 2, 2);
        CarDistanceWallB = Math.Round((float)ShaftWidth - (CarDistanceWallD + carWidth), 2);
        CarDistanceWallC = Math.Round((float)ShaftDepth - (CarDistanceWallA + carDepth), 2);

        SKPath midLineCarHorizontal = new();
        midLineCarHorizontal.MoveTo(-(float)(ShaftWidth + 600) / 2, (float)ShaftDepth / 2 - (float)carWallA);
        midLineCarHorizontal.LineTo((float)(ShaftWidth + 600) / 2, (float)ShaftDepth / 2 - (float)carWallA);

        SKPath midLineCarVertical = new();
        midLineCarVertical.MoveTo(-(float)ShaftWidth / 2 + (float)carWallD, (float)(ShaftDepth + 600) / 2);
        midLineCarVertical.LineTo(-(float)ShaftWidth / 2 + (float)carWallD, -(float)(ShaftDepth + 600) / 2);

        SKPath liftCar = new();
        liftCar.MoveTo(-(float)carWidth / 2, (float)carDepth / 2);
        if (carDoorMountingD != 0 && carDoorWidthD != 0)
        {
            liftCar.LineTo(-(float)carWidth / 2, (float)carDepth / 2 - (float)carR4);
            liftCar.RLineTo(-carDoorMountingD, 0);
            liftCar.RLineTo(0, -carDoorWidthD);
            liftCar.RLineTo(carDoorMountingD, 0);
        }
        liftCar.LineTo(-(float)carWidth / 2, -(float)carDepth / 2);
        if (carDoorMountingC != 0 && carDoorWidthC != 0)
        {
            liftCar.RLineTo(carR2, 0);
            liftCar.RLineTo(0, -carDoorMountingC);
            liftCar.RLineTo(carDoorWidthC, 0);
            liftCar.RLineTo(0, carDoorMountingC);
        }
        liftCar.LineTo((float)carWidth / 2, -(float)carDepth / 2);
        if (carDoorMountingB != 0 && carDoorWidthB != 0)
        {
            liftCar.RLineTo(0, carR3);
            liftCar.RLineTo(carDoorMountingB, 0);
            liftCar.RLineTo(0, carDoorWidthB);
            liftCar.RLineTo(-carDoorMountingB, 0);
        }
        liftCar.LineTo((float)carWidth / 2, (float)carDepth / 2);
        if (carDoorMountingA != 0 && carDoorWidthA != 0)
        {
            liftCar.RLineTo(-carR1, 0);
            liftCar.RLineTo(0, carDoorMountingA);
            liftCar.RLineTo(-carDoorWidthA, 0);
            liftCar.RLineTo(0, -carDoorMountingA);
        }
        liftCar.Close();

        liftCar.Transform(SKMatrix.CreateTranslation(midLineCarVertical.LastPoint.X, midLineCarHorizontal.LastPoint.Y));

        using var paintCar = new SKPaint
        {
            Color = SKColors.MediumPurple,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
        };

        using var paintOutline = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            StrokeWidth = _stokeWith * 1.5f,
            Style = SKPaintStyle.Stroke
        };
        canvas.DrawPath(liftCar, paintCar);
        canvas.DrawPath(liftCar, paintOutline);

        using var paintCarMidLine = new SKPaint
        {
            Color = SKColors.Purple,
            PathEffect = SKPathEffect.CreateDash([250, 50, 20, 50], 1),
            IsAntialias = true,
            StrokeWidth = _stokeWith * 1.5f,
            Style = SKPaintStyle.Stroke

        };
        canvas.DrawPath(midLineCarHorizontal, paintCarMidLine);
        canvas.DrawPath(midLineCarVertical, paintCarMidLine);

        //LiftDoors
        string[] entrances = ["A", "B", "C", "D"];

        foreach (var entrance in entrances)
        {
            if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_ZUGANSSTELLEN_{entrance}"))
            {
                continue;
            }

            float carDoorWidth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, entrance == "A" ? "var_TB"
                                                                                                                       : $"var_TB_{entrance}");
            string doorOpening = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, entrance == "A" ? "var_Tueroeffnung"
                                                                                                                        : $"var_Tueroeffnung_{entrance}");
            float crossbarDepth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_KabTuerKaempferBreite{entrance}");
            string carDoorDescription = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, $"var_CarDoorDescription{entrance}");
            string shaftDoorDescription = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, $"var_ShaftDoorDescription{entrance}");
            var carDoor = _parametercontext.Set<CarDoor>().FirstOrDefault(x => x.Name == carDoorDescription);
            var shaftDoor = _parametercontext.Set<ShaftDoor>().FirstOrDefault(x => x.Name == shaftDoorDescription);

            if (string.IsNullOrWhiteSpace(doorOpening) || carDoorWidth <= 0)
            {
                continue;
            }

            float doorPositionX = 0;
            float doorPositionY = 0;

            switch (entrance)
            {
                case "A":
                    doorPositionX = midLineCarVertical.Bounds.MidX + (float)carWidth * 0.5f - carR1 - carDoorWidthA * 0.5f;
                    doorPositionY = midLineCarHorizontal.Bounds.MidY + (float)carDepth * 0.5f + carDoorMountingA;
                    break;
                case "B":
                    doorPositionX = midLineCarVertical.Bounds.MidX + (float)carWidth * 0.5f + carDoorMountingB;
                    doorPositionY = midLineCarHorizontal.Bounds.MidY - (float)carDepth * 0.5f + carR3 + carDoorWidthB * 0.5f;
                    break;
                case "C":
                    doorPositionX = midLineCarVertical.Bounds.MidX - (float)carWidth * 0.5f + carR2 + carDoorWidthC * 0.5f;
                    doorPositionY = midLineCarHorizontal.Bounds.MidY - (float)carDepth * 0.5f - carDoorMountingC;
                    break;
                case "D":
                    doorPositionX = midLineCarVertical.Bounds.MidX - (float)carWidth * 0.5f - carDoorMountingD;
                    doorPositionY = midLineCarHorizontal.Bounds.MidY + (float)carDepth * 0.5f - carR4 - carDoorWidthD * 0.5f;
                    break;
                default:
                    return;
            }

            (SKPath, SKPath)? carDoorPath = null;
            if (carDoor is not null)
            {
                var carDoorOpeningDirection = string.Empty;

                if (carDoor.LiftDoorOpeningDirectionId == 3)
                {
                    carDoorOpeningDirection = "zentral";
                }
                else if (carDoor.LiftDoorOpeningDirectionId == 2)
                {
                    carDoorOpeningDirection = doorOpening switch
                    {
                        "einseitig öffnend (rechts)" => "rechts",
                        "einseitig öffnend (links)" => "links",
                        _ => string.Empty
                    };
                }

                if (string.IsNullOrWhiteSpace(carDoorOpeningDirection))
                {
                    carDoorPath = SkiaSharpHelpers.CreateCarDoor(doorPositionX, doorPositionY, carDoor, entrance, carDoorWidth, carDoorOpeningDirection, crossbarDepth);
                }
            }

            (SKPath, SKPath)? shaftDoorPath = null;

            if (shaftDoor is not null)
            {
                var shaftDoorOpeningDirection = string.Empty;

                if (shaftDoor.LiftDoorOpeningDirectionId == 3)
                {
                    shaftDoorOpeningDirection = "zentral";
                }
                else if (shaftDoor.LiftDoorOpeningDirectionId == 2)
                {
                    shaftDoorOpeningDirection = doorOpening switch
                    {
                        "einseitig öffnend (rechts)" => "rechts",
                        "einseitig öffnend (links)" => "links",
                        _ => string.Empty
                    };
                }

                //TODO improve installationType advanced door Selection 
                string liftDoortyp = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tuertyp");

                if (string.IsNullOrWhiteSpace(shaftDoorOpeningDirection))
                {
                    shaftDoorPath = SkiaSharpHelpers.CreateShaftDoor(doorPositionX, doorPositionY, shaftDoor, entrance, carDoorWidth, shaftDoorOpeningDirection, liftDoortyp);
                }
            }

            using var paintDoor = new SKPaint
            {
                Color = SKColors.MediumSlateBlue,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawPath(carDoorPath?.Item1, paintDoor);
            canvas.DrawPath(carDoorPath?.Item1, paintOutline);
            canvas.DrawPath(shaftDoorPath?.Item1, paintDoor);
            canvas.DrawPath(shaftDoorPath?.Item1, paintOutline);

            using var paintDoorPanels = new SKPaint
            {
                Color = SKColors.BlueViolet,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawPath(carDoorPath?.Item2, paintDoorPanels);
            canvas.DrawPath(carDoorPath?.Item2, paintOutline);
            canvas.DrawPath(shaftDoorPath?.Item2, paintDoorPanels);
            canvas.DrawPath(shaftDoorPath?.Item2, paintOutline);
        }
    }

    private void DrawCarFrame(SKCanvas canvas)
    {
        float carDimensionD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_MassD");
        float carWallA = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_AbstandKabineA");
        float carWallD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_AbstandKabineD");
        float carFrameOffsetY = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Versatz_Y");
        float carFrameOffsetX = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Versatz_X");
        float carframeDGB = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Stichmass");
        float counterWeightDGB = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Stichmass_GGW");
        float frameOffsetY = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Versatz_Stichmass_Y");
        float frameCounterWeightOffset = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Versatz_Gegengewicht_Stichmass_parallel");
        float counterWeightOffset = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Versatz_Gegengewicht_Stichmass");
        string carFramePosition = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bausatzlage");
        bool carFrameMirrorImage = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_Spiegelbildlich");
        float counterWeightWidth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Gegengewicht_Einlagenbreite");
        float counterWeightDepth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Gegengewicht_Einlagentiefe");

        var carFrameTyp = _calculationsModuleService.GetCarFrameTyp(ParameterDictionary);
        if (carFrameTyp == null)
        {
            return;
        }

        var cantiLever = carFrameTyp.CarFrameBaseType != null && (carFrameTyp.CarFrameBaseTypeId == 1 || carFrameTyp.CarFrameBaseTypeId == 4);

        SKPoint startPointDGB = new();
        SKPoint endPointDGB = new();
        SKPoint startPointMidDGB = new();
        SKPoint endPointMidDGB = new();

        SKPoint carFrameRailLeft = new();
        SKPoint carFrameRailRight = new();
        SKPoint cwtRailLeft = new();
        SKPoint cwtRailRight = new();

        float carFrameRailLeftRotation = 0f;
        float carFrameRailRightRotation = 0f;
        float cwtRailLeftRotation = 0f;
        float cwtRailRightRotation = 0f;
        if (cantiLever)
        {
            switch (carFramePosition)
            {
                case "A":
                    return;
                case "B":
                    startPointDGB.X = (float)ShaftWidth / 2 - carDimensionD;
                    startPointDGB.Y = (float)(ShaftDepth + 600) / 2;
                    endPointDGB.X = (float)ShaftWidth / 2 - carDimensionD;
                    endPointDGB.Y = -(float)(ShaftDepth + 600) / 2;
                    startPointMidDGB.X = startPointDGB.X - 100f;
                    endPointMidDGB.X = endPointDGB.X + 100f;
                    startPointMidDGB.Y = (float)ShaftDepth / 2 - carWallA - carFrameOffsetY;
                    endPointMidDGB.Y = (float)ShaftDepth / 2 - carWallA - carFrameOffsetY;

                    //GuideRails
                    carFrameRailLeftRotation = 90f;
                    carFrameRailRightRotation = 270f;
                    carFrameRailLeft.X = startPointDGB.X;
                    carFrameRailRight.X = startPointDGB.X;
                    carFrameRailLeft.Y = startPointMidDGB.Y - carframeDGB * 0.5f;
                    carFrameRailRight.Y = startPointMidDGB.Y + carframeDGB * 0.5f;
                    cwtRailLeftRotation = 90f;
                    cwtRailRightRotation = 270f;
                    cwtRailLeft.X = startPointDGB.X + frameCounterWeightOffset;
                    cwtRailRight.X = startPointDGB.X + frameCounterWeightOffset;
                    cwtRailLeft.Y = startPointMidDGB.Y - counterWeightDGB * 0.5f;
                    cwtRailRight.Y = startPointMidDGB.Y + counterWeightDGB * 0.5f;
                    break;
                case "C":
                    startPointDGB.X = -(float)(ShaftWidth + 600) / 2;
                    startPointDGB.Y = -(float)ShaftDepth / 2 + carDimensionD;
                    endPointDGB.X = (float)(ShaftWidth + 600) / 2;
                    endPointDGB.Y = -(float)ShaftDepth / 2 + carDimensionD;
                    startPointMidDGB.Y = startPointDGB.Y - 100f;
                    endPointMidDGB.Y = endPointDGB.Y + 100f;
                    startPointMidDGB.X = -(float)ShaftWidth / 2 + carWallD + carFrameOffsetY;
                    endPointMidDGB.X = -(float)ShaftWidth / 2 + carWallD + carFrameOffsetY;

                    //GuideRails
                    carFrameRailLeftRotation = 0f;
                    carFrameRailRightRotation = 180f;
                    carFrameRailLeft.X = startPointMidDGB.X - carframeDGB * 0.5f;
                    carFrameRailRight.X = startPointMidDGB.X + carframeDGB * 0.5f;
                    carFrameRailLeft.Y = startPointDGB.Y;
                    carFrameRailRight.Y = startPointDGB.Y;
                    cwtRailLeftRotation = 0f;
                    cwtRailRightRotation = 180f;
                    cwtRailLeft.X = startPointMidDGB.X - counterWeightDGB * 0.5f;
                    cwtRailRight.X = startPointMidDGB.X + counterWeightDGB * 0.5f;
                    cwtRailLeft.Y = carFrameRailLeft.Y - frameCounterWeightOffset;
                    cwtRailRight.Y = carFrameRailLeft.Y - frameCounterWeightOffset;
                    break;
                case "D":
                    startPointDGB.X = -(float)ShaftWidth / 2 + carDimensionD;
                    startPointDGB.Y = (float)(ShaftDepth + 600) / 2;
                    endPointDGB.X = -(float)ShaftWidth / 2 + carDimensionD;
                    endPointDGB.Y = -(float)(ShaftDepth + 600) / 2;
                    startPointMidDGB.X = startPointDGB.X - 100f;
                    endPointMidDGB.X = endPointDGB.X + 100f;
                    startPointMidDGB.Y = (float)ShaftDepth / 2 - carWallA - carFrameOffsetY;
                    endPointMidDGB.Y = (float)ShaftDepth / 2 - carWallA - carFrameOffsetY;

                    //GuideRails
                    carFrameRailLeftRotation = 90f;
                    carFrameRailRightRotation = 270f;
                    carFrameRailLeft.X = startPointDGB.X;
                    carFrameRailRight.X = startPointDGB.X;
                    carFrameRailLeft.Y = startPointMidDGB.Y - carframeDGB * 0.5f;
                    carFrameRailRight.Y = startPointMidDGB.Y + carframeDGB * 0.5f;
                    cwtRailLeftRotation = 90f;
                    cwtRailRightRotation = 270f;
                    cwtRailLeft.X = startPointDGB.X - frameCounterWeightOffset;
                    cwtRailRight.X = startPointDGB.X - frameCounterWeightOffset;
                    cwtRailLeft.Y = startPointMidDGB.Y - counterWeightDGB * 0.5f;
                    cwtRailRight.Y = startPointMidDGB.Y + counterWeightDGB * 0.5f;
                    break;
                default:
                    return;
            }
        }
        else
        {
            startPointDGB.X = -(float)(ShaftWidth + 600) / 2;
            endPointDGB.X = (float)(ShaftWidth + 600) / 2;
            if (carFrameMirrorImage)
            {
                counterWeightOffset *= -1;
                frameOffsetY *= -1;
            }
            startPointDGB.Y = (float)ShaftDepth / 2 - carDimensionD;
            endPointDGB.Y = (float)ShaftDepth / 2 - carDimensionD;
            startPointMidDGB.X = -(float)ShaftWidth / 2 + carWallD + carFrameOffsetX;
            endPointMidDGB.X = -(float)ShaftWidth / 2 + carWallD + carFrameOffsetX;

            switch (carFramePosition)
            {
                case "A":
                    return;
                case "B" or "D":
                    startPointMidDGB.Y = startPointDGB.Y - 100f + frameOffsetY;
                    endPointMidDGB.Y = endPointDGB.Y + 100f + frameOffsetY;
                    break;
                case "C":
                    startPointMidDGB.Y = startPointDGB.Y - 100f;
                    endPointMidDGB.Y = endPointDGB.Y + 100f;
                    break;
                default:
                    return;
            }

            //GuideRails
            carFrameRailLeft.X = startPointMidDGB.X - carframeDGB * 0.5f;
            carFrameRailRight.X = startPointMidDGB.X + carframeDGB * 0.5f;
            carFrameRailLeft.Y = startPointDGB.Y;
            carFrameRailRight.Y = startPointDGB.Y;
            carFrameRailLeftRotation = 0f;
            carFrameRailRightRotation = 180f;

            if (carFramePosition == "C")
            {
                cwtRailLeftRotation = 0f;
                cwtRailRightRotation = 180f;
                cwtRailLeft.X = startPointMidDGB.X - counterWeightDGB * 0.5f;
                cwtRailRight.X = startPointMidDGB.X + counterWeightDGB * 0.5f;
                cwtRailLeft.Y = carFrameRailLeft.Y - frameCounterWeightOffset;
                cwtRailRight.Y = carFrameRailLeft.Y - frameCounterWeightOffset;
            }
            else
            {
                cwtRailLeftRotation = 90f;
                cwtRailRightRotation = 270f;
                cwtRailLeft.X = carFramePosition == "D" ? carFrameRailLeft.X - frameCounterWeightOffset
                                                        : carFrameRailRight.X + frameCounterWeightOffset;
                cwtRailRight.X = carFramePosition == "D" ? carFrameRailLeft.X - frameCounterWeightOffset
                                                         : carFrameRailRight.X + frameCounterWeightOffset;
                cwtRailLeft.Y = startPointDGB.Y - counterWeightOffset - counterWeightDGB * 0.5f;
                cwtRailRight.Y = startPointDGB.Y - counterWeightOffset + counterWeightDGB * 0.5f;
            }
        }

        using var paintCarFrameMidLine = new SKPaint
        {
            Color = SKColors.RoyalBlue,
            PathEffect = SKPathEffect.CreateDash([250, 50, 20, 50], 1),
            IsAntialias = true,
            StrokeWidth = _stokeWith * 1.5f,
            Style = SKPaintStyle.Stroke
        };

        SKPath midLineCarFrameDGB = new();
        midLineCarFrameDGB.MoveTo(startPointDGB);
        midLineCarFrameDGB.LineTo(endPointDGB);
        canvas.DrawPath(midLineCarFrameDGB, paintCarFrameMidLine);

        SKPath midLineCarFrame = new();
        midLineCarFrame.MoveTo(startPointMidDGB);
        midLineCarFrame.LineTo(endPointMidDGB);
        canvas.DrawPath(midLineCarFrame, paintCarFrameMidLine);

        if (!cantiLever && frameOffsetY != 0)
        {
            SKPath midLineCarFrameHorizonal = new();
            midLineCarFrameHorizonal.MoveTo(startPointMidDGB.X - 250f, midLineCarFrame.Bounds.MidY);
            midLineCarFrameHorizonal.LineTo(endPointMidDGB.X + 250f, midLineCarFrame.Bounds.MidY);
            canvas.DrawPath(midLineCarFrameHorizonal, paintCarFrameMidLine);
        }

        if (CarframeGuideRail is not null)
        {
            using var paintStrokeSmall = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                IsStroke = true,
                StrokeWidth = _stokeWith,
                Style = SKPaintStyle.Stroke
            };
            using var paintCarGuideRail = new SKPaint
            {
                Color = SKColors.SlateBlue,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
            };

            SKPath guideRailFrameLeft = SkiaSharpHelpers.CreateGuideRail(carFrameRailLeft.X, carFrameRailLeft.Y, CarframeGuideRail, carFrameRailLeftRotation);
            canvas.DrawPath(guideRailFrameLeft, paintCarGuideRail);
            canvas.DrawPath(guideRailFrameLeft, paintStrokeSmall);
            SKPath guideRailFrameRight = SkiaSharpHelpers.CreateGuideRail(carFrameRailRight.X, carFrameRailRight.Y, CarframeGuideRail, carFrameRailRightRotation);
            canvas.DrawPath(guideRailFrameRight, paintCarGuideRail);
            canvas.DrawPath(guideRailFrameRight, paintStrokeSmall);

            if (CounterWeightGuideRail is not null)
            {
                using var paintCounterWeightGuideRail = new SKPaint
                {
                    Color = SKColors.BlueViolet,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                };
                SKPath guideRailCWTLeft = SkiaSharpHelpers.CreateGuideRail(cwtRailLeft.X, cwtRailLeft.Y, CounterWeightGuideRail, cwtRailLeftRotation);
                canvas.DrawPath(guideRailCWTLeft, paintCarGuideRail);
                canvas.DrawPath(guideRailCWTLeft, paintStrokeSmall);
                SKPath guideRailCWTRight = SkiaSharpHelpers.CreateGuideRail(cwtRailRight.X, cwtRailRight.Y, CounterWeightGuideRail, cwtRailRightRotation);
                canvas.DrawPath(guideRailCWTRight, paintCarGuideRail);
                canvas.DrawPath(guideRailCWTRight, paintStrokeSmall);

                //CounterWeight
                if (counterWeightWidth > 0 && counterWeightDepth > 0)
                {
                    SKRect counterWeightFilling = new()
                    {
                        Size = carFramePosition == "A" || carFramePosition == "C" ? new SKSize(counterWeightWidth, counterWeightDepth)
                                                                                  : new SKSize(counterWeightDepth, counterWeightWidth)
                    };
                    counterWeightFilling.Offset(-counterWeightFilling.MidX, -counterWeightFilling.MidY);
                    counterWeightFilling.Offset((cwtRailLeft.X + cwtRailRight.X) * 0.5f, (cwtRailLeft.Y + cwtRailRight.Y) * 0.5f);
                    canvas.DrawRect(counterWeightFilling, paintCarGuideRail);
                    canvas.DrawRect(counterWeightFilling, paintStrokeSmall);
                }
            }
        }
    }

    public void RefreshView()
    {
        _xamlCanvas?.Invalidate();
    }

    private void SetViewBoxDimensions()
    {
        ShaftWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_SB");
        ShaftDepth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_ST");

        _scale = (float)(1200 / (ShaftWidth + ShaftDepth));
        _stokeWith = 1 / _scale;
        if (ShaftWidth > 0 && ShaftDepth > 0)
        {
            ViewBoxWidth = (ShaftWidth + 800) * _scale;
            ViewBoxHeight = (ShaftDepth + 800) * _scale;
        }
    }

    private void SetWallOpeningOffsets()
    {
        OffsetWallOpeningShaftDoorA = CalculateWallOpeningOffsets("A");
        OffsetWallOpeningShaftDoorB = CalculateWallOpeningOffsets("B");
        OffsetWallOpeningShaftDoorC = CalculateWallOpeningOffsets("C");
        OffsetWallOpeningShaftDoorD = CalculateWallOpeningOffsets("D");
    }

    private double CalculateWallOpeningOffsets(string entrance)
    {
        if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_ZUGANSSTELLEN_{entrance}"))
        {
            return 0;
        }

        double wallOpeningdistanceLeftSide = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, $"var_MauerOeffnungAbstand{entrance}");
        double wallOpeningWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, $"var_MauerOeffnungBreite{entrance}");
        double doorWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, entrance == "A" ? "var_TB" : $"var_TB_{entrance}");
        double entranceLeftSide;

        switch (entrance)
        {
            case "A" or "C":
                double carWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KBI");
                double carDistanceWallD = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_AbstandKabineD");
                entranceLeftSide = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, entrance == "A" ? "var_L1" : "var_L2");

                return entrance == "A" ? Math.Round((wallOpeningdistanceLeftSide + wallOpeningWidth / 2) - (carDistanceWallD - carWidth / 2 + entranceLeftSide + doorWidth / 2), 2) :
                                         Math.Round((ShaftWidth - (wallOpeningdistanceLeftSide + wallOpeningWidth / 2)) - (carDistanceWallD + carWidth / 2 - entranceLeftSide - doorWidth / 2), 2);
            case "B" or "D":
                double carDepth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KTI");
                double carDistanceWallA = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_AbstandKabineA");
                entranceLeftSide = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, entrance == "B" ? "var_L3" : "var_L4");

                return entrance == "B" ? Math.Round((wallOpeningdistanceLeftSide + wallOpeningWidth / 2) - (carDistanceWallA - carDepth / 2 + entranceLeftSide + doorWidth / 2), 2) :
                                         Math.Round((ShaftDepth - (wallOpeningdistanceLeftSide + wallOpeningWidth / 2)) - (carDistanceWallA + carDepth / 2 - entranceLeftSide - doorWidth / 2), 2);
            default:
                return 0;
        }
    }

    private void SetDefaultCarPosition()
    {
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_AbstandKabineA"].Value))
        {
            ParameterDictionary["var_AbstandKabineA"].AutoUpdateParameterValue((ShaftDepth / 2).ToString());
        }
        if (string.IsNullOrWhiteSpace(ParameterDictionary["var_AbstandKabineD"].Value))
        {
            ParameterDictionary["var_AbstandKabineD"].AutoUpdateParameterValue((ShaftWidth / 2).ToString());
        }
    }

    private void SetCarFrameOffset(string? parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return;
        }

        if (string.Equals(parameterName, "setup") || string.Equals(parameterName, "var_Versatz_Y") || string.Equals(parameterName, "var_MassD") ||
            string.Equals(parameterName, "var_Bausatzlage") || string.Equals(parameterName, "var_Spiegelbildlich") || string.Equals(parameterName, "var_AbstandKabineA"))
        {
            double carFrameOffsetY = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Versatz_Stichmass_Y");
            double carOffsetY = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Versatz_Y");
            string carFramePosition = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bausatzlage");
            bool carFrameMirrorImage = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_Spiegelbildlich");

            CarFrameTyp = _calculationsModuleService.GetCarFrameTyp(ParameterDictionary);
            if (CarFrameTyp is null)
                return;

            if (CarFrameTyp.CarFrameBaseTypeId == 1 || CarFrameTyp.CarFrameBaseTypeId == 4)
            {
                ParameterDictionary["var_Versatz_Stichmass_Y"].AutoUpdateParameterValue("0");
                switch (carFramePosition)
                {
                    case "A" or "C":
                        ShowCarFrameOffsetInfoVertikal = false;
                        ShowCarFrameOffsetInfoHorizontal = carFrameOffsetY != 0;
                        break;
                    case "B" or "D":
                        ShowCarFrameOffsetInfoHorizontal = false;
                        ShowCarFrameOffsetInfoVertikal = carFrameOffsetY != 0;
                        break;
                    default:
                        break;
                }
                IsCarFrameOffsetXEnabled = false;
                IsFrameToCarOffsetEnabled = true;
            }
            else
            {
                if (carFrameMirrorImage)
                {
                    carFrameOffsetY *= -1;
                }
                double carOffsetWallA = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_AbstandKabineA");
                if (string.Equals(parameterName, "var_MassD"))
                {
                    double carFrameOffsetWallA = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_MassD");
                    ParameterDictionary["var_Versatz_Y"].AutoUpdateParameterValue((carFrameOffsetWallA - carOffsetWallA - carFrameOffsetY).ToString());
                }
                else
                {
                    ParameterDictionary["var_MassD"].AutoUpdateParameterValue((carOffsetWallA + carFrameOffsetY + carOffsetY).ToString());
                }
                ShowCarFrameOffsetInfoHorizontal = false;
                ShowCarFrameOffsetInfoVertikal = carOffsetY != 0;
                IsCarFrameOffsetXEnabled = true;
                IsFrameToCarOffsetEnabled = false;
            }
        }
    }

    private async Task UpdateShaftDataAsync(int delay)
    {
        await Task.Delay(delay);
        OpeningDirectionA = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung");
        OpeningDirectionB = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_B");
        OpeningDirectionC = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_C");
        OpeningDirectionD = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_D");
        CheckIsOpeningDirectionSelected();
        SetWallOpeningOffsets();
        SetDefaultCarPosition();
        SetCarFrameOffset("setup");
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_FuehrungsschieneFahrkorb"].Value))
        {
            CarframeGuideRail = _parametercontext.Set<GuideRails>().FirstOrDefault(x => x.Name.Contains(ParameterDictionary["var_FuehrungsschieneFahrkorb"].Value!));
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_FuehrungsschieneGegengewicht"].Value))
        {
            CounterWeightGuideRail = _parametercontext.Set<GuideRails>().FirstOrDefault(x => x.Name.Contains(ParameterDictionary["var_FuehrungsschieneGegengewicht"].Value!));
        }
        LiftParameterHelper.SetDefaultCarFrameData(ParameterDictionary, CarFrameTyp);
        RefreshView();
    }

    [RelayCommand]
    private static void GoToSchachtViewModel()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(SchachtPage));
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
        {
            SetViewBoxDimensions();
            UpdateShaftDataAsync(500).SafeFireAndForget();
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}
