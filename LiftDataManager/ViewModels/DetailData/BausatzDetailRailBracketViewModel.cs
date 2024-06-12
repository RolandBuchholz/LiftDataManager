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
    private readonly float _shaftPitOffset = 400f;
    private double _carRailSplit = 5000;
    private double _cwtRailSplit = 5000;

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
            Location = new SKPoint(-(float)((ShaftDepth + 500) / 2), -(float)((ShaftHeight + _shaftPitOffset + 250f)))
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
            Location = new SKPoint(-(float)(ShaftDepth / 2), -(float)(ShaftHeight + _shaftPitOffset))
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

    private void DrawGuideRails(SKCanvas canvas)
    {
        if (CarFrameTyp == null)
        {
            return;
        }

        float carRailLength = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Schienenlaenge");
        float cwtRailLength = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Hilfsschienenlaenge");

        if (carRailLength == 0 && cwtRailLength == 0)
        {
            return;
        }

        float carRailWidth = 80f;
        float cwtRailWidth = 50f;
        float posY = -_shaftPitOffset;
        float carRailLeftPosX = 0f;
        float carRailRightPosX = 0f;
        float cwtRailLeftPosX = 0f;
        float cwtRailLRightPosX = 0f;

        float cwtDgb = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Stichmass_GGW");
        
        if (CarFrameTyp.CarFrameBaseTypeId == 1)
        {
            carRailLeftPosX = -500f;
            carRailRightPosX = 500f;
            cwtRailLeftPosX = -350f;
            cwtRailLRightPosX = 350f;

            float carDgb = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Stichmass");

            if (carDgb != 0)
            {
                carRailLeftPosX = -carDgb / 2;
                carRailRightPosX = carDgb / 2;
            }

            if (cwtDgb != 0)
            {
                cwtRailLeftPosX = -cwtDgb / 2;
                cwtRailLRightPosX = cwtDgb / 2;
            }
        }
        else
        {
            float carDistanceWall = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_MassD");
            carRailLeftPosX = 235f + carRailWidth / 2;
            carRailRightPosX = 235f - carRailWidth / 2;
            cwtRailLRightPosX = 235f + 145f;
            cwtRailLeftPosX = cwtRailLRightPosX - 760f;

            if (cwtDgb != 0)
            {
                cwtRailLeftPosX = cwtRailLRightPosX - cwtDgb;
            }
        }

        using var paintCarRail = new SKPaint
        {
            Color = SKColors.MediumVioletRed,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using var paintCwtRail = new SKPaint
        {
            Color = SKColors.BlueViolet,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using var paintCarRailJointLine = new SKPaint
        {
            Color = SKColors.MediumVioletRed,
            PathEffect = SKPathEffect.CreateDash([250, 50, 20, 50], 1),
            IsAntialias = true,
            StrokeWidth = _stokeWith * 2f,
            Style = SKPaintStyle.Stroke
        };

        using var paintCwtRailJointLine = new SKPaint
        {
            Color = SKColors.BlueViolet,
            PathEffect = SKPathEffect.CreateDash([250, 50, 20, 50], 1),
            IsAntialias = true,
            StrokeWidth = _stokeWith * 2f,
            Style = SKPaintStyle.Stroke
        };

        SKRect carRailLeftRect = new()
        {
            Size = new SKSize(carRailWidth, -carRailLength),
            Location = new SKPoint(carRailLeftPosX - carRailWidth, posY)
        };
        canvas.DrawRect(carRailLeftRect, paintCarRail);

        SKRect carRailRightRect = new()
        {
            Size = new SKSize(carRailWidth, -carRailLength),
            Location = new SKPoint(carRailRightPosX, posY)
        };
        canvas.DrawRect(carRailRightRect, paintCarRail);

        SKRect cwtRailLeftRect = new()
        {
            Size = new SKSize(cwtRailWidth, -cwtRailLength),
            Location = new SKPoint(cwtRailLeftPosX - cwtRailWidth, posY)
        };
        canvas.DrawRect(cwtRailLeftRect, paintCwtRail);

        SKRect cwtRailRightRect = new()
        {
            Size = new SKSize(cwtRailWidth, -cwtRailLength),
            Location = new SKPoint(cwtRailLRightPosX, posY)
        };
        canvas.DrawRect(cwtRailRightRect, paintCwtRail);

        float startCarRailLength = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Startschiene");
        float startCwtRailLength = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_HilfsschienenlaengeStartstueck");
        int carRailCount = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_Anzahl_5m_Schienen");
        int cwtRailCount = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_AnzahlHilfsschienenlaengeStueck");
        SKPoint startPoint = new(-(float)(shaftDepth / 2 + 600), -_shaftPitOffset);
        SKPoint endPoint = new((float)(shaftDepth / 2 + 600), -_shaftPitOffset);

        SKPath cwtRailJointLine = new();
        cwtRailJointLine.MoveTo(startPoint);
        cwtRailJointLine.LineTo(endPoint);

        SKPath carRailJointLine = new();
        carRailJointLine.MoveTo(startPoint);
        carRailJointLine.LineTo(endPoint);

        cwtRailJointLine.Offset(0, -startCwtRailLength);
        canvas.DrawPath(cwtRailJointLine, paintCwtRailJointLine);
        carRailJointLine.Offset(0, -startCarRailLength);
        canvas.DrawPath(carRailJointLine, paintCarRailJointLine);

        if (cwtRailCount > 0)
        {
            for (int i = 0; i < cwtRailCount; i++)
            {
                cwtRailJointLine.Offset(0, -(float)_cwtRailSplit);
                canvas.DrawPath(cwtRailJointLine, paintCwtRailJointLine);
            }
        }

        if (carRailCount > 0)
        {
            for (int i = 0; i < carRailCount; i++)
            {
                carRailJointLine.Offset(0, -(float)_carRailSplit);
                canvas.DrawPath(carRailJointLine, paintCarRailJointLine);
            }
        }
    }

    private void DrawRailBrackets(SKCanvas canvas)
    {
        using var paintRailBracketLine = new SKPaint
        {
            Color = SKColors.LimeGreen,
            PathEffect = SKPathEffect.CreateDash([250, 50, 20, 50], 1),
            IsAntialias = true,
            StrokeWidth = _stokeWith * 2f,
            Style = SKPaintStyle.Stroke
        };
        float shaftPitBracket = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_B1");

        SKPoint startPoint = new(-(float)(shaftDepth / 2 + 600), -_shaftPitOffset);
        SKPoint endPoint = new((float)(shaftDepth / 2 + 600), -_shaftPitOffset);

        SKPath railBracketLine = new();
        railBracketLine.MoveTo(startPoint);
        railBracketLine.LineTo(endPoint);

        railBracketLine.Offset(0, -shaftPitBracket);
        canvas.DrawPath(railBracketLine, paintRailBracketLine);

        //if (cwtRailCount > 0)
        //{
        //    for (int i = 0; i < cwtRailCount; i++)
        //    {
        //        railBracketLine.Offset(0, -(float)_cwtRailSplit);
        //        canvas.DrawPath(railBracketLine, paintRailBracketLine);
        //    }
        //}
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

        var carGuideRailLength = _parametercontext.Set<GuideRailLength>().FirstOrDefault(x => x.Name == ParameterDictionary["var_SchienenteilungFK"].Value);
        if (carGuideRailLength is not null)
        {
            _carRailSplit = carGuideRailLength.RailLength;
        }

        var cwtGuideRailLength = _parametercontext.Set<GuideRailLength>().FirstOrDefault(x => x.Name == ParameterDictionary["var_SchienenteilungGGW"].Value);
        if (cwtGuideRailLength is not null)
        {
            _cwtRailSplit = cwtGuideRailLength.RailLength;
        }

        //calculations
        DistanceCarRailShaftCeiling = ShaftHeight - carRailLength;
        DistanceCWTRailShaftCeiling = ShaftHeight - cwtRailLength;

        var carRailData = CalculateRailData(carRailLength, startCarRailLength, _carRailSplit, maxRailLength);
        var cwtRailData = CalculateRailData(cwtRailLength, startCwtRailLength, _cwtRailSplit, maxRailLength);

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
        ErrorStandardRail = _carRailSplit > maxRailLength || _cwtRailSplit > maxRailLength;
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
        canvas.Translate(info.Width / 2, info.Height);
        canvas.Scale(_scale);
        DrawShaftWall(canvas);
        DrawGuideRails(canvas);
        DrawRailBrackets(canvas);
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
            CalculateDimensions();
        }   
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}