using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Windows;
using SkiaSharp;
using System.Text.Json;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Input;

namespace LiftDataManager.ViewModels;

public partial class BausatzDetailRailBracketViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    private readonly ILogger<BausatzDetailViewModel> _logger;

    public ObservableCollection<Parameter> ListOfCustomRailBracketDistances { get; set; } = [];
    public ObservableCollection<Parameter> ActiveCustomRailBracketDistances { get; set; } = [];
    public List<double> RailBracketDistances { get; set; } = [];

    public BausatzDetailRailBracketViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService, ParameterContext parametercontext, ICalculationsModule calculationsModuleService, ILogger<BausatzDetailViewModel> logger) :
         base(parameterDataService, dialogService, infoCenterService)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        _logger = logger;
    }

    private readonly string[] shaftDesignParameter = [ "var_Schienenlaenge", "var_Hilfsschienenlaenge", "var_SchienenteilungFK", "var_SchienenteilungGGW",
                                                       "var_Startschiene", "var_HilfsschienenlaengeStartstueck", "var_maxSchienenLaenge",
                                                       "var_B1", "var_B2" ];

    private readonly string[] railBracketDistancesParameter = [ "var_B2_1", "var_B2_2", "var_B2_3" ,"var_B2_4","var_B2_5",
                                                                "var_B2_6", "var_B2_7", "var_B2_8" ,"var_B2_9","var_B2_10",
                                                                "var_B2_11", "var_B2_12", "var_B2_13" ,"var_B2_14","var_B2_15",
                                                                "var_B2_16", "var_B2_17", "var_B2_18" ,"var_B2_19" ];

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null)
            return;
        if (!(message.Sender.GetType() == typeof(Parameter)))
            return;
        if (message.PropertyName == "var_SG" || message.PropertyName == "var_FH" || message.PropertyName == "var_SK")
        {
            SetViewBoxDimensions();
        };

        if (shaftDesignParameter.Contains(message.PropertyName))
        {
            CalculateDimensions();
            RefreshView();
        };

        if (railBracketDistancesParameter.Contains(message.PropertyName))
        {
            OrderListOfRailBrackets(message.NewValue, message.PropertyName);
            CalculateDimensions();
            RefreshView();
        };

        SetInfoSidebarPanelText(message);
        _ = SetModelStateAsync();
    }

    private float _scale;
    private float _stokeWith;

    private SKXamlCanvas? _xamlCanvas;

    public CarFrameType? CarFrameTyp { get; set; }

    [ObservableProperty]
    private double viewBoxWidth;

    [ObservableProperty]
    private double viewBoxHeight;

    [ObservableProperty]
    private double shaftDepth;

    [ObservableProperty]
    private double shaftPit;

    [ObservableProperty]
    private double shaftTravel;
    partial void OnShaftTravelChanged(double value)
    {
        ParameterDictionary["var_FH"].Value = (value / 1000).ToString();
    }

    [ObservableProperty]
    private double shaftHeadroom;

    [ObservableProperty]
    private double shaftHeight;

    [ObservableProperty]
    private string cWTRailName = "Führungsschienen Gegengewicht";

    [ObservableProperty]
    private string cWTRailNameShaftCeilling = "Führungsschienen Gegengewicht";

    [ObservableProperty]
    private bool errorStartRail;

    [ObservableProperty]
    private bool errorStandardRail;

    [ObservableProperty]
    private bool errorTotalRailLength;

    [ObservableProperty]
    private bool errorDistanceGuideRailBracket;

    [ObservableProperty]
    private bool errorTotalDistanceGuideRailBrackets;

    [ObservableProperty]
    private double distanceCarRailShaftCeiling;

    [ObservableProperty]
    private double distanceCWTRailShaftCeiling;

    [ObservableProperty]
    private double carRailBracketDistanceLeft;

    [ObservableProperty]
    private double cwtRailBracketDistanceLeft;

    [ObservableProperty]
    private int railBracketLevelCount;

    [ObservableProperty]
    private double maxRailBracketSpacing;

    [ObservableProperty]
    private double distanceCarRailjoint;

    [ObservableProperty]
    private double distanceCwtRailjoint;

    [ObservableProperty]
    private bool customRailBracketSpacing;
    partial void OnCustomRailBracketSpacingChanged(bool oldValue, bool newValue)
    {
        if (oldValue && !newValue)
        {
            foreach (string railBracket in railBracketDistancesParameter)
            {
                  ParameterDictionary[railBracket].Value = "0";
            }
        }
        if (newValue)
        {
            CanAddCustomRailBracketDistance = ActiveCustomRailBracketDistances.Count < 19;
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveCustomRailBracketDistanceCommand))]
    private bool canRemoveCustomRailBracketDistance;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCustomRailBracketDistanceCommand))]
    private bool canAddCustomRailBracketDistance;

    private void DrawShaftWall(SKCanvas canvas)
    {
        SKPathEffect diagLinesPath = SKPathEffect.Create2DLine(_stokeWith,
        SkiaSharpHelpers.Multiply(SKMatrix.CreateScale(100f, 100f), SKMatrix.CreateRotationDegrees(45f)));

        SKRect shaftWallOutSide = new()
        {
            Size = new SKSize((float)(ShaftDepth + 500), (float)(ShaftHeight + 500)),
            Location = new SKPoint(-(float)((ShaftDepth + 500) / 2), -(float)((ShaftHeight + 500) / 2))
        };

        SKRect shaftWallHatch = new()
        {
            Size = shaftWallOutSide.Size,
            Location = shaftWallOutSide.Location
        };

        shaftWallHatch.Inflate(100f, 100f);

        SKRect shaftWallInSide = new()
        {
            Size = new SKSize((float)ShaftDepth , (float)ShaftHeight),
            Location = new SKPoint(-(float)(ShaftDepth / 2), -(float)(ShaftHeight / 2))
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
            StrokeWidth = 2 * _stokeWith,
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
        //string[] entrances = new[] { "A", "B", "C", "D" };

        //foreach (var entrance in entrances)
        //{
        //    if (!LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, $"var_ZUGANSSTELLEN_{entrance}"))
        //        continue;
        //    float wallOpeningWidth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_MauerOeffnungBreite{entrance}");
        //    if (wallOpeningWidth == 0)
        //        continue;
        //    float wallOpeningLeft = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_MauerOeffnungAbstand{entrance}");

        //    using var paintShaftEntrace = new SKPaint
        //    {
        //        Color = SKColors.LightSlateGray,
        //        IsAntialias = true,
        //        Style = SKPaintStyle.Fill
        //    };
        //    using var paintOutline = new SKPaint
        //    {
        //        Color = SKColors.DarkRed,
        //        IsAntialias = true,
        //        StrokeWidth = _stokeWith * 2,
        //        Style = SKPaintStyle.Stroke
        //    };
        //    using var paintText = new SKPaint
        //    {
        //        Color = SKColors.DarkRed,
        //        IsAntialias = true,
        //        TextAlign = SKTextAlign.Center,
        //        TextSize = 200,
        //    };

        //    float width = 0f;
        //    float height = 0f;
        //    float posX = 0f;
        //    float posY = 0f;

        //    switch (entrance)
        //    {
        //        case "A":
        //            width = wallOpeningWidth;
        //            height = 250f;
        //            posX = -(float)shaftWidth / 2 + wallOpeningLeft;
        //            posY = (float)shaftDepth / 2;
        //            break;
        //        case "B":
        //            width = 250f;
        //            height = wallOpeningWidth;
        //            posX = (float)shaftWidth / 2;
        //            posY = (float)shaftDepth / 2 - wallOpeningWidth - wallOpeningLeft;
        //            break;
        //        case "C":
        //            width = wallOpeningWidth;
        //            height = -250f;
        //            posX = (float)shaftWidth / 2 - wallOpeningWidth - wallOpeningLeft;
        //            posY = -(float)shaftDepth / 2;
        //            break;
        //        case "D":
        //            width = -250;
        //            height = wallOpeningWidth;
        //            posX = -(float)shaftWidth / 2;
        //            posY = -(float)shaftDepth / 2 + wallOpeningLeft;
        //            break;
        //        default:
        //            continue;
        //    }

        //    SKRect entranceRect = new()
        //    {
        //        Size = new SKSize(width, height),
        //        Location = new SKPoint(posX, posY)
        //    };

        //    SKRect textRect = new();
        //    paintText.MeasureText(entrance, ref textRect);

        //    canvas.DrawRect(entranceRect, paintShaftEntrace);
        //    canvas.DrawRect(entranceRect, paintOutline);
        //    canvas.DrawText(entrance, entranceRect.MidX, entranceRect.MidY - textRect.MidY, paintText);
        //}
    }

    private void DrawLiftCar(SKCanvas canvas)
    {
        //float carWidth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_KBI");
        //float carDepth = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_KTI");
        //float carWallA = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_AbstandKabineA");
        //float carWallD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_AbstandKabineD");

        //float carR1 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_R1");
        //float carR2 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_R2");
        //float carR3 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_R3");
        //float carR4 = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_R4");

        //float carDoorMountingA = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TuerEinbau");
        //float carDoorMountingB = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TuerEinbauB");
        //float carDoorMountingC = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TuerEinbauC");
        //float carDoorMountingD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TuerEinbauD");

        //float carDoorWidthA = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TB");
        //float carDoorWidthB = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TB_B");
        //float carDoorWidthC = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TB_C");
        //float carDoorWidthD = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, $"var_TB_D");

        //if (carWidth == 0 || carDepth == 0)
        //    return;
        //CarDistanceWallA = carWallA - carDepth / 2;
        //CarDistanceWallD = carWallD - carWidth / 2;
        //CarDistanceWallB = (float)shaftWidth - (CarDistanceWallD + carWidth);
        //CarDistanceWallC = (float)shaftDepth - (CarDistanceWallA + carDepth);

        //SKPath midLineCarHorizontal = new();
        //midLineCarHorizontal.MoveTo(-(float)viewBoxWidth / 2, (float)shaftDepth / 2 - carWallA);
        //midLineCarHorizontal.LineTo((float)viewBoxWidth / 2, (float)shaftDepth / 2 - carWallA);

        //SKPath midLineCarVertical = new();
        //midLineCarVertical.MoveTo(-(float)shaftWidth / 2 + carWallD, (float)viewBoxHeight / 2);
        //midLineCarVertical.LineTo(-(float)shaftWidth / 2 + carWallD, -(float)viewBoxHeight / 2);

        //SKPath liftCar = new();
        //liftCar.MoveTo(-carWidth / 2, carDepth / 2);
        //if (carDoorMountingD != 0 && carDoorWidthD != 0)
        //{
        //    liftCar.LineTo(-carWidth / 2, carDepth / 2 - carR4);
        //    liftCar.RLineTo(-carDoorMountingD, 0);
        //    liftCar.RLineTo(0, -carDoorWidthD);
        //    liftCar.RLineTo(carDoorMountingD, 0);
        //}
        //liftCar.LineTo(-carWidth / 2, -carDepth / 2);
        //if (carDoorMountingC != 0 && carDoorWidthC != 0)
        //{
        //    liftCar.RLineTo(carR2, 0);
        //    liftCar.RLineTo(0, -carDoorMountingC);
        //    liftCar.RLineTo(carDoorWidthC, 0);
        //    liftCar.RLineTo(0, carDoorMountingC);
        //}
        //liftCar.LineTo(carWidth / 2, -carDepth / 2);
        //if (carDoorMountingB != 0 && carDoorWidthB != 0)
        //{
        //    liftCar.RLineTo(0, carR3);
        //    liftCar.RLineTo(carDoorMountingB, 0);
        //    liftCar.RLineTo(0, carDoorWidthB);
        //    liftCar.RLineTo(-carDoorMountingB, 0);
        //}
        //liftCar.LineTo(carWidth / 2, carDepth / 2);
        //if (carDoorMountingA != 0 && carDoorWidthA != 0)
        //{
        //    liftCar.RLineTo(-carR1, 0);
        //    liftCar.RLineTo(0, carDoorMountingA);
        //    liftCar.RLineTo(-carDoorWidthA, 0);
        //    liftCar.RLineTo(0, -carDoorMountingA);
        //}
        //liftCar.Close();

        //liftCar.Transform(SKMatrix.CreateTranslation(midLineCarVertical.LastPoint.X, midLineCarHorizontal.LastPoint.Y));

        //using var paintCar = new SKPaint
        //{
        //    Color = SKColors.MediumPurple,
        //    IsAntialias = true,
        //    Style = SKPaintStyle.Fill,
        //};

        //using var paintCarStoke = new SKPaint
        //{
        //    Color = SKColors.Black,
        //    IsAntialias = true,
        //    StrokeWidth = _stokeWith * 1.5f,
        //    Style = SKPaintStyle.Stroke
        //};
        //canvas.DrawPath(liftCar, paintCar);
        //canvas.DrawPath(liftCar, paintCarStoke);

        //using var paintCarMidLine = new SKPaint
        //{
        //    Color = SKColors.DarkGreen,
        //    PathEffect = SKPathEffect.CreateDash(new float[] { 250, 50, 20, 50 }, 1),
        //    IsAntialias = true,
        //    StrokeWidth = _stokeWith * 1.5f,
        //    Style = SKPaintStyle.Stroke

        //};
        //canvas.DrawPath(midLineCarHorizontal, paintCarMidLine);
        //canvas.DrawPath(midLineCarVertical, paintCarMidLine);
    }

    public void RefreshView()
    {
        _xamlCanvas?.Invalidate();
    }

    private void CalculateDimensions()
    {
        double firstRailBracket = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_B1");
        double equallyBracketSpacing = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_B2");
        double carRailLength = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Schienenlaenge");
        double cwtRailLength = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Hilfsschienenlaenge");
        double startCarRailLength = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Startschiene");
        double startCwtRailLength = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_HilfsschienenlaengeStartstueck");
        double maxRailLength = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_maxSchienenLaenge");

        //DataBase

        double carRailSplit = 5000;
        var carGuideRailLength = _parametercontext.Set<GuideRailLength>().FirstOrDefault(x => x.Name == ParameterDictionary["var_SchienenteilungFK"].Value);
        if (carGuideRailLength is not null)
        {
            carRailSplit = carGuideRailLength.RailLength;
        }

        double cwtRailSplit = 5000;
        var cwtGuideRailLength = _parametercontext.Set<GuideRailLength>().FirstOrDefault(x => x.Name == ParameterDictionary["var_SchienenteilungGGW"].Value);
        if (cwtGuideRailLength is not null)
        {
            cwtRailSplit = cwtGuideRailLength.RailLength;
        }

        //calculations
        DistanceCarRailShaftCeiling = ShaftHeight - carRailLength;
        DistanceCWTRailShaftCeiling = ShaftHeight - cwtRailLength;

        var carRailData = CalculateRailData(carRailLength, startCarRailLength, carRailSplit, maxRailLength);
        var cwtRailData = CalculateRailData(cwtRailLength, startCwtRailLength, cwtRailSplit, maxRailLength);

        ParameterDictionary["var_Anzahl_5m_Schienen"].AutoUpdateParameterValue(carRailData.Item1.ToString());
        ParameterDictionary["var_Endschiene"].AutoUpdateParameterValue(carRailData.Item2.ToString());
        ParameterDictionary["var_AnzahlHilfsschienenlaengeStueck"].AutoUpdateParameterValue(cwtRailData.Item1.ToString());
        ParameterDictionary["var_HilfsschienenlaengeEndstueck"].AutoUpdateParameterValue(cwtRailData.Item2.ToString());

        RailBracketDistances.Clear();
        foreach (var parameter in ActiveCustomRailBracketDistances)
        {
            if (double.TryParse(parameter.Value, out double distance))
            {
                RailBracketDistances.Add(distance);
            }
        }

        MaxRailBracketSpacing = CustomRailBracketSpacing ? RailBracketDistances.Count > 0 ? RailBracketDistances.Max() : 0
                                                         : LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_B2");

        if (CustomRailBracketSpacing)
        {
            RailBracketLevelCount = RailBracketDistances.Count + 1;
            double customrailbracketdistance = 0;
            foreach (var railbracket in RailBracketDistances)
            {
                customrailbracketdistance += railbracket;
            }
            CarRailBracketDistanceLeft = carRailLength - firstRailBracket - customrailbracketdistance;
            CwtRailBracketDistanceLeft = cwtRailLength - firstRailBracket - customrailbracketdistance;
        }
        else
        {
            if (carRailLength < 0)
            {
                RailBracketLevelCount = carRailLength >= cwtRailLength ? (int)Math.Floor((carRailLength - firstRailBracket) / equallyBracketSpacing)
                                                       : (int)Math.Floor((cwtRailLength - firstRailBracket) / equallyBracketSpacing);
                CarRailBracketDistanceLeft = carRailLength - firstRailBracket - RailBracketLevelCount * equallyBracketSpacing;
                CwtRailBracketDistanceLeft = cwtRailLength - firstRailBracket - RailBracketLevelCount * equallyBracketSpacing;
            }
            else
            {
                RailBracketLevelCount = 0;
                CarRailBracketDistanceLeft = 0;
                CwtRailBracketDistanceLeft = 0;
            }
        }

        //errors
        ErrorStartRail = startCarRailLength > maxRailLength || startCwtRailLength > maxRailLength;
        ErrorStandardRail = carRailSplit > maxRailLength || cwtRailSplit > maxRailLength;
        ErrorTotalRailLength = DistanceCarRailShaftCeiling < 0 || DistanceCWTRailShaftCeiling < 0;
        ErrorDistanceGuideRailBracket = DistanceCarRailjoint < 200 || DistanceCwtRailjoint < 200;
        ErrorTotalDistanceGuideRailBrackets = CarRailBracketDistanceLeft < 0 && CwtRailBracketDistanceLeft < 0;
    }

    private static (double, double) CalculateRailData(double railLength, double startRailLength, double railSplit, double maxRailLength)
    {
        double railCount = 0;
        double endRailLength = 0;

        if (railSplit == 0 || railLength == 0 || startRailLength == 0)
        {
            return (railCount, endRailLength);
        }

        railCount = Math.Floor((railLength - startRailLength) / railSplit);

        if ((railLength - startRailLength) % railSplit == 0)
        {
            railCount --;
        }

        if ((railLength - startRailLength) % railSplit == 0)
        {
            railCount --;
        }

        endRailLength = railLength - railCount * railSplit - startRailLength;

        if (maxRailLength >= endRailLength + railSplit)
        {
            railCount --;
            endRailLength = railLength - railCount * railSplit - startRailLength;
        }

        return (railCount, endRailLength);
    }

    private void SetViewBoxDimensions()
    {
        ShaftPit = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_SG");
        ShaftTravel = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_FH") * 1000;
        ShaftHeadroom = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_SK");
        ShaftHeight = ShaftPit + ShaftTravel + ShaftHeadroom;
        double realShaftDepth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_ST");
        double realShaftWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_SB");
        string framePosition = LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Bausatzlage");
        ShaftDepth = framePosition == "A" || framePosition == "C" ? realShaftWidth : realShaftDepth;

        _scale = (float)(1000 / ShaftHeight);
        _stokeWith = 1 / _scale;

        if (ShaftHeight > 0 && ShaftDepth > 0)
        {
            ViewBoxWidth = (ShaftDepth + 800) * _scale;
            ViewBoxHeight = (ShaftHeight + 800) * _scale;
        }
        CalculateDimensions();
    }

    private void GetCarFrameTyp()
    {
        CarFrameTyp = _calculationsModuleService.GetCarFrameTyp(ParameterDictionary);
        if (CarFrameTyp is not null)
        {
            CWTRailName = CarFrameTyp.DriveTypeId == 2 ? "Führungsschienen Joch" : "Führungsschienen Gegengewicht";
            CWTRailNameShaftCeilling = CarFrameTyp.DriveTypeId == 2 ? "Jochschiene Schachtdecke" : "Gegengewichtsschiene Schachtdecke";
        }
    }

    private void FillListOfRailBrackets()
    {
        foreach (string railBracket in railBracketDistancesParameter)
        {
            ListOfCustomRailBracketDistances.Add(ParameterDictionary[railBracket]);
        }
        if (CustomRailBracketSpacing)
        {
            OrderListOfRailBrackets("0", null);
        }
    }

    private void OrderListOfRailBrackets(string value, string? name)
    {
        if (value == "0" || name is null || string.IsNullOrWhiteSpace(value))
        {
            ActiveCustomRailBracketDistances.Clear();
            RailBracketDistances.Clear();
            var tempRailBracketDistances = new List<string?>();
            foreach (string railBracket in railBracketDistancesParameter)
            {
                var railBracketDistance = ParameterDictionary[railBracket].Value;
                if (!string.IsNullOrWhiteSpace(railBracketDistance) && !string.Equals(railBracketDistance, "0"))
                {
                    tempRailBracketDistances.Add(railBracketDistance);
                }
            }

            for (int i = 0; i < ListOfCustomRailBracketDistances.Count; i++)
            {
                if (tempRailBracketDistances.Count > i)
                {
                    ListOfCustomRailBracketDistances[i].Value = tempRailBracketDistances[i];
                    ActiveCustomRailBracketDistances.Add(ListOfCustomRailBracketDistances[i]);
                }
                else
                {
                    ListOfCustomRailBracketDistances[i].AutoUpdateParameterValue("0");
                }
            }
        }
        else
        {
           if (!ActiveCustomRailBracketDistances.Any(p => p.Name == name))
           {
                var railBracket = ListOfCustomRailBracketDistances.FirstOrDefault(p => p.Name == name);
                if (railBracket is not null && !string.IsNullOrWhiteSpace(value))
                {
                    railBracket.Value = value;
                    ActiveCustomRailBracketDistances.Add(railBracket);
                }
           }
        }

        CanRemoveCustomRailBracketDistance = ActiveCustomRailBracketDistances.Count > 0;
        CanAddCustomRailBracketDistance = ActiveCustomRailBracketDistances.Count < 19;
    }

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
        //DrawEntranceWall(canvas);
        //DrawLiftCar(canvas);
    }

    [RelayCommand]
    private void OnLoadedSKXamlCanvas(SKXamlCanvas xamlCanvas)
    {
        _xamlCanvas = xamlCanvas;
    }

    [RelayCommand(CanExecute = nameof(CanAddCustomRailBracketDistance))]
    private void AddCustomRailBracketDistance()
    {
        var parameter = ActiveCustomRailBracketDistances.LastOrDefault();
        if (parameter is not null)
        {
            var tempDistance = parameter.Value;
            var index = ActiveCustomRailBracketDistances.IndexOf(parameter);
            ListOfCustomRailBracketDistances[index + 1].Value = tempDistance;
        }
        else
        {
            var firstDistance = ParameterDictionary["var_B2"].Value;
            if (!string.IsNullOrWhiteSpace(firstDistance) && !string.Equals(firstDistance,"0"))
            {
                ListOfCustomRailBracketDistances[0].Value = firstDistance;
            }
            else
            {
                ListOfCustomRailBracketDistances[0].Value = "1650";
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanRemoveCustomRailBracketDistance))]
    private void RemoveCustomRailBracketDistance()
    {
        var parameter = ActiveCustomRailBracketDistances.LastOrDefault();
        if (parameter is not null)
        {
            parameter.Value = "0";
        }
    }

    [RelayCommand]
    private void KeyDownAddNewRailBracketDistance(object sender)
    {
        var listView = sender as ListView;
        if (listView is not null)
        {
            if (CanAddCustomRailBracketDistance && listView.SelectedIndex + 1 == ActiveCustomRailBracketDistances.Count)
            {
                AddCustomRailBracketDistanceCommand.Execute(null);
            }

            FindNextElementOptions fneo = new() { SearchRoot = listView.XamlRoot.Content };
            _ = FocusManager.TryMoveFocus(FocusNavigationDirection.Next, fneo);
            _ = FocusManager.TryMoveFocus(FocusNavigationDirection.Next, fneo);
        }
    }

    [RelayCommand]
    private static void GoToBausatzViewModel()
    {
        LiftParameterNavigationHelper.NavigateToPage(typeof(BausatzPage));
    }

    [ObservableProperty]
    private PivotItem? selectedPivotItem;
    partial void OnSelectedPivotItemChanged(PivotItem? value)
    {
        if (value?.Tag != null)
        {
            var pageType = Application.Current.GetType().Assembly.GetType($"LiftDataManager.Views.{value.Tag}");
            if (pageType != null)
            {

                LiftParameterNavigationHelper.NavigatePivotItem(pageType);
            }
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            GetCarFrameTyp();
            SetViewBoxDimensions();
            CustomRailBracketSpacing = !string.IsNullOrWhiteSpace(ParameterDictionary["var_B2_1"].Value) && 
                                       !string.Equals(ParameterDictionary["var_B2_1"].Value, "0");
            FillListOfRailBrackets();
        }   
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}