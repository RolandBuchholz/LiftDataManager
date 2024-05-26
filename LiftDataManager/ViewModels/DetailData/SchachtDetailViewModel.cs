using CommunityToolkit.Mvvm.Messaging.Messages;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System.Collections.ObjectModel;

namespace LiftDataManager.ViewModels;

public partial class SchachtDetailViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    private readonly ICalculationsModule _calculationsModuleService;
    public ObservableCollection<string?> OpeningDirections { get; set; }


    public SchachtDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ICalculationsModule calculationsModuleService) :
     base(parameterDataService, dialogService, infoCenterService)
    {
        _calculationsModuleService = calculationsModuleService;
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
                                                       "var_Bausatzlage", "var_MassD",
                                                       "var_KBI", "var_KTI",
                                                       "var_AbstandKabineA", "var_AbstandKabineD",
                                                       "var_L1","var_L2","var_L3","var_L4",
                                                       "var_TB","var_TB_B","var_TB_C","var_TB_D",
                                                       "var_TuerEinbau","var_TuerEinbauB","var_TuerEinbauC","var_TuerEinbauD",
                                                       "var_Tueroeffnung","var_Tueroeffnung_B","var_Tueroeffnung_C","var_Tueroeffnung_D" ];

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;
        if (message.PropertyName == "var_SB" || message.PropertyName == "var_ST")
        {
            SetViewBoxDimensions();
        };

        if (shaftDesignParameter.Contains(message.PropertyName))
        {
            SetWallOpeningOffsets();
            RefreshView();
        };
        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    [ObservableProperty]
    private double viewBoxWidth;

    [ObservableProperty]
    private double viewBoxHeight;

    [ObservableProperty]
    private double shaftWidth;

    [ObservableProperty]
    private double shaftDepth;

    [ObservableProperty]
    private float carDistanceWallA;

    [ObservableProperty]
    private float carDistanceWallB;

    [ObservableProperty]
    private float carDistanceWallC;

    [ObservableProperty]
    private float carDistanceWallD;

    public bool CanEditOpeningDirectionEntranceA => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_A") &&
                                                    ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung")).StartsWith("einseitig");
    public bool CanEditOpeningDirectionEntranceB => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_B") &&
                                                    ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_B")).StartsWith("einseitig");
    public bool CanEditOpeningDirectionEntranceC => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_C") &&
                                                    ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_C")).StartsWith("einseitig");
    public bool CanEditOpeningDirectionEntranceD => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_D") &&
                                                    ((string)LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_D")).StartsWith("einseitig");

    [ObservableProperty]
    private double offsetWallOpeningShaftDoorA;

    [ObservableProperty]
    private double offsetWallOpeningShaftDoorB;

    [ObservableProperty]
    private double offsetWallOpeningShaftDoorC;

    [ObservableProperty]
    private double offsetWallOpeningShaftDoorD;

    [ObservableProperty]
    private bool openingDirectionNotSelected;

    [ObservableProperty]
    private string? openingDirectionA;
    partial void OnOpeningDirectionAChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            ParameterDictionary!["var_Tueroeffnung"].DropDownListValue = value;
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    private string? openingDirectionB;
    partial void OnOpeningDirectionBChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            ParameterDictionary!["var_Tueroeffnung_B"].DropDownListValue = value;
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    private string? openingDirectionC;
    partial void OnOpeningDirectionCChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            ParameterDictionary!["var_Tueroeffnung_C"].DropDownListValue = value;
            CheckIsOpeningDirectionSelected();
        }
    }

    [ObservableProperty]
    private string? openingDirectionD;
    partial void OnOpeningDirectionDChanged(string? value)
    {
        if (ParameterDictionary is not null)
        {
            ParameterDictionary!["var_Tueroeffnung_D"].DropDownListValue = value;
            CheckIsOpeningDirectionSelected();
        }
    }

    private void CheckIsOpeningDirectionSelected()
    {
        OpeningDirectionNotSelected = (!string.IsNullOrWhiteSpace(openingDirectionA) && string.Equals(openingDirectionA, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(openingDirectionB) && string.Equals(openingDirectionB, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(openingDirectionC) && string.Equals(openingDirectionC, "einseitig öffnend")) ||
                                      (!string.IsNullOrWhiteSpace(openingDirectionD) && string.Equals(openingDirectionD, "einseitig öffnend"));
    }

    private float _stokeWith;

    private SKXamlCanvas? _xamlCanvas;

    [RelayCommand]
    private void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        SKImageInfo info = e.Info;
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        canvas.Clear();
        canvas.Translate(info.Width / 2, info.Height / 2);
        _stokeWith = ((float)shaftWidth + (float)shaftDepth) / 600f;
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
        SkiaSharpHelpers.Multiply(SKMatrix.CreateScale(100f, 100f), SKMatrix.CreateRotationDegrees(45f)));

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

        shaftWallHatch.Inflate(100f, 100f);

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
        string[] entrances = new[] { "A", "B", "C", "D" };

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
                TextAlign = SKTextAlign.Center,
                TextSize = 200,
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
                    posX = -(float)shaftWidth / 2 + wallOpeningLeft;
                    posY = (float)shaftDepth / 2;
                    break;
                case "B":
                    width = 250f;
                    height = wallOpeningWidth;
                    posX = (float)shaftWidth / 2;
                    posY = (float)shaftDepth / 2 - wallOpeningWidth - wallOpeningLeft;
                    break;
                case "C":
                    width = wallOpeningWidth;
                    height = -250f;
                    posX = (float)shaftWidth / 2 - wallOpeningWidth - wallOpeningLeft;
                    posY = -(float)shaftDepth / 2;
                    break;
                case "D":
                    width = -250;
                    height = wallOpeningWidth;
                    posX = -(float)shaftWidth / 2;
                    posY = -(float)shaftDepth / 2 + wallOpeningLeft;
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
            paintText.MeasureText(entrance, ref textRect);

            canvas.DrawRect(entranceRect, paintShaftEntrace);
            canvas.DrawRect(entranceRect, paintOutline);
            canvas.DrawText(entrance, entranceRect.MidX, entranceRect.MidY - textRect.MidY, paintText);
        }
    }

    private void DrawLiftCar(SKCanvas canvas)
    {
        float carWidth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_KBI");
        float carDepth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_KTI");
        float carWallA = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_AbstandKabineA");
        float carWallD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_AbstandKabineD");

        float carR1 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_R1");
        float carR2 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_R2");
        float carR3 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_R3");
        float carR4 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_R4");

        float carDoorMountingA = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TuerEinbau");
        float carDoorMountingB = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TuerEinbauB");
        float carDoorMountingC = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TuerEinbauC");
        float carDoorMountingD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TuerEinbauD");

        float carDoorWidthA = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TB");
        float carDoorWidthB = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TB_B");
        float carDoorWidthC = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TB_C");
        float carDoorWidthD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TB_D");

        if (carWidth == 0 || carDepth == 0)
            return;
        CarDistanceWallA = carWallA - carDepth / 2;
        CarDistanceWallD = carWallD - carWidth / 2;
        CarDistanceWallB = (float)shaftWidth - (CarDistanceWallD + carWidth);
        CarDistanceWallC = (float)shaftDepth - (CarDistanceWallA + carDepth);

        SKPath midLineCarHorizontal = new();
        midLineCarHorizontal.MoveTo(-(float)viewBoxWidth / 2, (float)shaftDepth / 2 - carWallA);
        midLineCarHorizontal.LineTo((float)viewBoxWidth / 2, (float)shaftDepth / 2 - carWallA);

        SKPath midLineCarVertical = new();
        midLineCarVertical.MoveTo(-(float)shaftWidth / 2 + carWallD, (float)viewBoxHeight / 2);
        midLineCarVertical.LineTo(-(float)shaftWidth / 2 + carWallD, -(float)viewBoxHeight / 2);

        SKPath liftCar = new();
        liftCar.MoveTo(-carWidth / 2, carDepth / 2);
        if (carDoorMountingD != 0 && carDoorWidthD != 0)
        {
            liftCar.LineTo(-carWidth / 2, carDepth / 2 - carR4);
            liftCar.RLineTo(-carDoorMountingD, 0);
            liftCar.RLineTo(0, -carDoorWidthD);
            liftCar.RLineTo(carDoorMountingD, 0);
        }
        liftCar.LineTo(-carWidth / 2, -carDepth / 2);
        if (carDoorMountingC != 0 && carDoorWidthC != 0)
        {
            liftCar.RLineTo(carR2, 0);
            liftCar.RLineTo(0, -carDoorMountingC);
            liftCar.RLineTo(carDoorWidthC, 0);
            liftCar.RLineTo(0, carDoorMountingC);
        }
        liftCar.LineTo(carWidth / 2, -carDepth / 2);
        if (carDoorMountingB != 0 && carDoorWidthB != 0)
        {
            liftCar.RLineTo(0, carR3);
            liftCar.RLineTo(carDoorMountingB, 0);
            liftCar.RLineTo(0, carDoorWidthB);
            liftCar.RLineTo(-carDoorMountingB, 0);
        }
        liftCar.LineTo(carWidth / 2, carDepth / 2);
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

        using var paintCarStoke = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            StrokeWidth = _stokeWith * 1.5f,
            Style = SKPaintStyle.Stroke
        };
        canvas.DrawPath(liftCar, paintCar);
        canvas.DrawPath(liftCar, paintCarStoke);

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
    }

    private void DrawCarFrame(SKCanvas canvas)
    {
        float carDimensionD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_MassD");
        string carFramePosition = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bausatzlage");

        var carFrameTyp = _calculationsModuleService.GetCarFrameTyp(ParameterDictionary);
        if (carFrameTyp == null)
        {
            return;
        }

        using var paintCarFrameMidLine = new SKPaint
        {
            Color = SKColors.RoyalBlue,
            PathEffect = SKPathEffect.CreateDash([250, 50, 20, 50], 1),
            IsAntialias = true,
            StrokeWidth = _stokeWith * 1.5f,
            Style = SKPaintStyle.Stroke
        };

        SKPoint startPoint = new();
        SKPoint endPoint = new();

        if (carFrameTyp.CarFrameBaseType != null && carFrameTyp.CarFrameBaseType.Name == "Rucksack")
        {
            switch (carFramePosition)
            {
                case "A":
                    startPoint.X = -(float)viewBoxWidth / 2;
                    startPoint.Y = (float)shaftDepth / 2 - carDimensionD;
                    endPoint.X = (float)viewBoxWidth / 2;
                    endPoint.Y = (float)shaftDepth / 2 - carDimensionD;
                    break;
                case "B":
                    startPoint.X = (float)shaftWidth / 2 - carDimensionD;
                    startPoint.Y = (float)viewBoxHeight / 2;
                    endPoint.X = (float)shaftWidth / 2 - carDimensionD;
                    endPoint.Y = -(float)viewBoxHeight / 2;
                    break;
                case "C":
                    startPoint.X = -(float)viewBoxWidth / 2;
                    startPoint.Y = -(float)shaftDepth / 2 + carDimensionD;
                    endPoint.X = (float)viewBoxWidth / 2;
                    endPoint.Y = -(float)shaftDepth / 2 + carDimensionD;
                    break;
                case "D":
                    startPoint.X = -(float)shaftWidth / 2 + carDimensionD;
                    startPoint.Y = (float)viewBoxHeight / 2;
                    endPoint.X = -(float)shaftWidth / 2 + carDimensionD;
                    endPoint.Y = -(float)viewBoxHeight / 2;
                    break;
                default:
                    startPoint.X = -(float)shaftWidth / 2 + carDimensionD;
                    startPoint.Y = (float)viewBoxHeight / 2;
                    endPoint.X = -(float)shaftWidth / 2 + carDimensionD;
                    endPoint.Y = -(float)viewBoxHeight / 2;
                    break;
            }
        }
        else
        {
            startPoint.X = -(float)viewBoxWidth / 2;
            startPoint.Y = (float)shaftDepth / 2 - carDimensionD;
            endPoint.X = (float)viewBoxWidth / 2;
            endPoint.Y = (float)shaftDepth / 2 - carDimensionD;
        }
        SKPath midLineCarFrame = new();
        midLineCarFrame.MoveTo(startPoint);
        midLineCarFrame.LineTo(endPoint);

        canvas.DrawPath(midLineCarFrame, paintCarFrameMidLine);
    }

    public void RefreshView()
    {
        _xamlCanvas?.Invalidate();
    }

    private void SetViewBoxDimensions()
    {
        ShaftWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_SB");
        ShaftDepth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_ST");
        if (ShaftWidth > 0 && ShaftDepth > 0)
        {
            ViewBoxWidth = ShaftWidth + 800;
            ViewBoxHeight = ShaftDepth + 800;
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

                return entrance == "A" ? (wallOpeningdistanceLeftSide + wallOpeningWidth / 2) - (carDistanceWallD - carWidth / 2 + entranceLeftSide + doorWidth / 2) :
                                         (ShaftWidth - (wallOpeningdistanceLeftSide + wallOpeningWidth / 2)) - (carDistanceWallD + carWidth / 2 - entranceLeftSide - doorWidth / 2);
            case "B" or "D":
                double carDepth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KTI");
                double carDistanceWallA = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_AbstandKabineA");
                entranceLeftSide = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, entrance == "B" ? "var_L3" : "var_L4");

                return entrance == "B" ? (wallOpeningdistanceLeftSide + wallOpeningWidth / 2) - (carDistanceWallA - carDepth / 2 + entranceLeftSide + doorWidth / 2) :
                                         (ShaftDepth - (wallOpeningdistanceLeftSide + wallOpeningWidth / 2)) - (carDistanceWallA + carDepth / 2 - entranceLeftSide - doorWidth / 2);
            default:
                return 0;
        }
    }

    [RelayCommand]
    private static void GoToSchachtViewModel()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(SchachtPage));
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            SetViewBoxDimensions();
            OpeningDirectionA = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung");
            OpeningDirectionB = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_B");
            OpeningDirectionC = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_C");
            OpeningDirectionD = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Tueroeffnung_D");
            CheckIsOpeningDirectionSelected();
            SetWallOpeningOffsets();
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}
