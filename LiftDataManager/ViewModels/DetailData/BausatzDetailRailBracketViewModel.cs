using CommunityToolkit.Mvvm.Messaging.Messages;
using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Input;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace LiftDataManager.ViewModels;

public partial class BausatzDetailRailBracketViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<RefreshModelStateMessage>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    private readonly ILogger<BausatzDetailViewModel> _logger;

    public ObservableCollection<Parameter> ListOfCustomRailBracketDistances { get; set; } = [];
    public ObservableCollection<Parameter> ActiveCustomRailBracketDistances { get; set; } = [];
    public List<double> RailBracketDistances { get; set; } = [];

    public BausatzDetailRailBracketViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                                             ISettingService settingService, ILogger<DataViewModelBase> baseLogger, ParameterContext parametercontext, ICalculationsModule calculationsModuleService, ILogger<BausatzDetailViewModel> logger) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
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
        if (message is null ||
            !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }
        if (message.PropertyName == "var_SG" || message.PropertyName == "var_FH" || message.PropertyName == "var_SK")
        {
            SetViewBoxDimensions();
        };

        if (message.PropertyName == "var_Gegengewicht_Einlagenbreite" || message.PropertyName == "var_Gegengewicht_Einlagentiefe")
        {
            if (FrameCalculationData is not null)
            {
                if (double.TryParse(message.NewValue, out double cwtDimension))
                {
                    if (message.PropertyName == "var_Gegengewicht_Einlagenbreite")
                    {
                        FrameCalculationData.CounterweightWidth = cwtDimension;
                    }
                    else
                    {
                        FrameCalculationData.CounterweightDepth = cwtDimension;
                    }
                    UpdateFrameCalculationData();
                }
            }
        };

        if (shaftDesignParameter.Contains(message.PropertyName))
        {
            CalculateDimensions();
            RefreshView();
        };

        if (railBracketDistancesParameter.Contains(message.PropertyName))
        {
            //Workaround for ActiveCustomRailBracketDistances.Clear();
            Thread.Sleep(50);
            //Workaround for ActiveCustomRailBracketDistances.Clear();
            OrderListOfRailBrackets(message.NewValue, message.PropertyName);
            CalculateDimensions();
            RefreshView();
        };

        SetInfoSidebarPanelText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
    }

    private float _scale;
    private float _stokeWith;
    private readonly float _shaftPitOffset = 400f;
    private double _carRailSplit = 5000;
    private double _cwtRailSplit = 5000;
    private double _startCarRailLength;
    private double _startCwtRailLength;

    private SKXamlCanvas? _xamlCanvas;

    public CarFrameType? CarFrameTyp { get; set; }

    [ObservableProperty]
    public partial FrameCalculationData? FrameCalculationData { get; set; }
    [ObservableProperty]
    public partial double ViewBoxWidth { get; set; }
    [ObservableProperty]
    public partial double ViewBoxHeight { get; set; }
    [ObservableProperty]
    public partial double ShaftDepth { get; set; }
    [ObservableProperty]
    public partial double ShaftPit { get; set; }
    [ObservableProperty]
    public partial double ShaftTravel { get; set; }
    partial void OnShaftTravelChanged(double value)
    {
        ParameterDictionary["var_FH"].Value = (value / 1000).ToString();
    }

    [ObservableProperty]
    public partial double ShaftHeadroom { get; set; }
    [ObservableProperty]
    public partial double ShaftHeight { get; set; }
    [ObservableProperty]
    public partial bool IsCFPControlled { get; set; }
    [ObservableProperty]
    public partial string CWTRailName { get; set; } = "Führungsschienen Gegengewicht";
    [ObservableProperty]
    public partial string CWTRailNameShaftCeilling { get; set; } = "Führungsschienen Gegengewicht";
    [ObservableProperty]
    public partial bool ErrorStartRail { get; set; }
    [ObservableProperty]
    public partial bool ErrorStandardRail { get; set; }
    [ObservableProperty]
    public partial bool ErrorTotalRailLength { get; set; }
    [ObservableProperty]
    public partial bool ErrorDistanceGuideRailBracket { get; set; }

    [ObservableProperty]
    public partial bool ErrorTotalDistanceGuideRailBrackets { get; set; }
    [ObservableProperty]
    public partial double DistanceCarRailShaftCeiling { get; set; }
    [ObservableProperty]
    public partial double DistanceCWTRailShaftCeiling { get; set; }
    [ObservableProperty]
    public partial double CarRailBracketDistanceLeft { get; set; }
    [ObservableProperty]
    public partial double CwtRailBracketDistanceLeft { get; set; }
    [ObservableProperty]
    public partial int RailBracketLevelCount { get; set; }
    [ObservableProperty]
    public partial double MaxRailBracketSpacing { get; set; }
    [ObservableProperty]
    public partial double DistanceCarRailjoint { get; set; }
    [ObservableProperty]
    public partial double DistanceCwtRailjoint { get; set; }
    [ObservableProperty]
    public partial bool CustomRailBracketSpacing { get; set; }
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
        RefreshView();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveCustomRailBracketDistanceCommand))]
    public partial bool CanRemoveCustomRailBracketDistance { get; set; }
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCustomRailBracketDistanceCommand))]
    public partial bool CanAddCustomRailBracketDistance { get; set; }
    [ObservableProperty]
    public partial double DistanceGuideShoesCarframe { get; set; }
    partial void OnDistanceGuideShoesCarframeChanged(double value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.DistanceGuideShoesCarframe = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial double DistanceGuideShoesCounterweight { get; set; }
    partial void OnDistanceGuideShoesCounterweightChanged(double value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.DistanceGuideShoesCounterweight = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial double CarCenterOfMassX { get; set; }
    partial void OnCarCenterOfMassXChanged(double value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.CarCenterOfMassX = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial double CarCenterOfMassY { get; set; }
    partial void OnCarCenterOfMassYChanged(double value)

    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.CarCenterOfMassY = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial double AdditionalRailForceCarframe { get; set; }
    partial void OnAdditionalRailForceCarframeChanged(double value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.AdditionalRailForceCarframe = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial double AdditionalRailForceCounterweight { get; set; }
    partial void OnAdditionalRailForceCounterweightChanged(double value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.AdditionalRailForceCounterweight = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial double OffsetCounterweightSuspensionCenter { get; set; }
    partial void OnOffsetCounterweightSuspensionCenterChanged(double value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.OffsetCounterweightSuspensionCenter = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial int CarframeBracketClipCount { get; set; } = 2;
    partial void OnCarframeBracketClipCountChanged(int value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.CarframeBracketClipCount = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial int CounterweightBracketClipCount { get; set; } = 2;
    partial void OnCounterweightBracketClipCountChanged(int value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.CounterweightBracketClipCount = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial bool HasSlidingClips { get; set; }
    partial void OnHasSlidingClipsChanged(bool value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.HasSlidingClips = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial double BuildingDeflectionX { get; set; }
    partial void OnBuildingDeflectionXChanged(double value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.BuildingDeflectionX = value;
            UpdateFrameCalculationData();
        }
    }
    [ObservableProperty]
    public partial double BuildingDeflectionY { get; set; }
    partial void OnBuildingDeflectionYChanged(double value)
    {
        if (FrameCalculationData is not null)
        {
            FrameCalculationData.BuildingDeflectionY = value;
            UpdateFrameCalculationData();
        }
    }
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
            Size = new SKSize((float)ShaftDepth, (float)ShaftHeight),
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

        if (cwtRailLength > 0)
        {
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
        }

        float startCarRailLength = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_Startschiene");
        float startCwtRailLength = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_HilfsschienenlaengeStartstueck");
        int carRailCount = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_Anzahl_5m_Schienen");
        int cwtRailCount = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_AnzahlHilfsschienenlaengeStueck");
        SKPoint startPoint = new(-(float)(ShaftDepth / 2 + 400), -_shaftPitOffset);
        SKPoint endPoint = new((float)(ShaftDepth / 2 + 400), -_shaftPitOffset);

        SKPath cwtRailJointLine = new();
        cwtRailJointLine.MoveTo(startPoint);
        cwtRailJointLine.LineTo(endPoint);
        if (cwtRailLength > 0)
        {
            cwtRailJointLine.Offset(0, -startCwtRailLength);
            canvas.DrawPath(cwtRailJointLine, paintCwtRailJointLine);
        }

        SKPath carRailJointLine = new();
        carRailJointLine.MoveTo(startPoint);
        carRailJointLine.LineTo(endPoint);
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
        if (RailBracketLevelCount < 1)
        {
            return;
        }

        float shaftPitBracket = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_B1");
        int carRailCount = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_Anzahl_5m_Schienen");
        int cwtRailCount = LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_AnzahlHilfsschienenlaengeStueck");
        double cwtRailLength = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Hilfsschienenlaenge");

        DistanceCarRailjoint = Math.Abs(_startCarRailLength - shaftPitBracket);
        DistanceCwtRailjoint = cwtRailLength > 0 ? Math.Abs(_startCwtRailLength - shaftPitBracket) : 0;

        var railBracketsDesignList = new List<(float, double)>
        {
            (shaftPitBracket, cwtRailLength > 0 ? Math.Min(DistanceCarRailjoint, DistanceCwtRailjoint) : DistanceCarRailjoint)
        };
        var railBracketLevel = shaftPitBracket;

        if (!CustomRailBracketSpacing)
        {
            float standardRailBracketSpacing = LiftParameterHelper.GetLiftParameterValue<float>(ParameterDictionary, "var_B2");
            if (standardRailBracketSpacing != 0)
            {
                for (int i = 0; i < RailBracketLevelCount - 1; i++)
                {
                    railBracketLevel += standardRailBracketSpacing;
                    var railJointDistanceCar = CalculateRailJointDistance(railBracketLevel, _startCarRailLength, _carRailSplit, carRailCount);
                    var railJointDistanceCwt = CalculateRailJointDistance(railBracketLevel, _startCwtRailLength, _cwtRailSplit, cwtRailCount);

                    if (DistanceCarRailjoint > railJointDistanceCar)
                    {
                        DistanceCarRailjoint = railJointDistanceCar;
                    }

                    if (DistanceCwtRailjoint > railJointDistanceCwt)
                    {
                        DistanceCwtRailjoint = railJointDistanceCwt;
                    }
                    railBracketsDesignList.Add((standardRailBracketSpacing, Math.Min(railJointDistanceCar, railJointDistanceCwt)));
                }
            }
        }
        else
        {
            if (RailBracketDistances.Count > 0)
            {
                foreach (var railBracketDistance in RailBracketDistances)
                {
                    railBracketLevel += (float)railBracketDistance;
                    var railJointDistanceCar = CalculateRailJointDistance(railBracketLevel, _startCarRailLength, _carRailSplit, carRailCount);
                    var railJointDistanceCwt = CalculateRailJointDistance(railBracketLevel, _startCwtRailLength, _cwtRailSplit, cwtRailCount);

                    if (DistanceCarRailjoint > railJointDistanceCar)
                    {
                        DistanceCarRailjoint = railJointDistanceCar;
                    }

                    if (DistanceCwtRailjoint > railJointDistanceCwt)
                    {
                        DistanceCwtRailjoint = railJointDistanceCwt;
                    }
                    railBracketsDesignList.Add(((float)railBracketDistance, Math.Min(railJointDistanceCar, railJointDistanceCwt)));
                }
            }
        }

        ErrorDistanceGuideRailBracket = cwtRailLength > 0 ? DistanceCarRailjoint < 200 || DistanceCwtRailjoint < 200 : DistanceCarRailjoint < 200;

        using var paintRailBracketLine = new SKPaint
        {
            Color = SKColors.LimeGreen,
            PathEffect = SKPathEffect.CreateDash([250, 50, 20, 50], 1),
            IsAntialias = true,
            StrokeWidth = _stokeWith * 2f,
            Style = SKPaintStyle.Stroke
        };

        using var paintRailBracketLineWarning = new SKPaint
        {
            Color = SKColors.DarkRed,
            PathEffect = SKPathEffect.CreateDash([250, 50, 20, 50], 1),
            IsAntialias = true,
            StrokeWidth = _stokeWith * 2f,
            Style = SKPaintStyle.Stroke
        };

        using var paintRailBracketDescription = new SKPaint
        {
            Color = SKColors.LimeGreen,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using var paintRailBracketDescriptionWarning = new SKPaint
        {
            Color = SKColors.DarkRed,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using var paintText = new SKPaint
        {
            Color = SKColors.WhiteSmoke,
            IsAntialias = true,
        };
        using var font = new SKFont
        {
            Size = 25 * _stokeWith,
        };

        SKPoint startPoint = new(-(float)(ShaftDepth / 2 + 400), -_shaftPitOffset);
        SKPoint endPoint = new((float)(ShaftDepth / 2 + 400), -_shaftPitOffset);
        SKPoint centerBracketDescription = new(-(float)((ShaftDepth / 2 + 300) - 15 * _stokeWith), -(_shaftPitOffset + 17 * _stokeWith));
        SKPath railBracketLine = new();
        railBracketLine.MoveTo(startPoint);
        railBracketLine.LineTo(endPoint);

        if (railBracketsDesignList.Count > 0)
        {
            int i = 0;
            foreach (var railBracket in railBracketsDesignList)
            {
                railBracketLine.Offset(0, -railBracket.Item1);
                centerBracketDescription.Offset(0, -railBracket.Item1);
                canvas.DrawPath(railBracketLine, railBracket.Item2 < 200 ? paintRailBracketLineWarning : paintRailBracketLine);
                canvas.DrawCircle(centerBracketDescription, 15f * _stokeWith, railBracket.Item2 < 200 ? paintRailBracketDescriptionWarning : paintRailBracketDescription);
                SKRect textRect = new();
                i++;
                string bracketDescriptionText = i.ToString();
                font.MeasureText(bracketDescriptionText, out textRect);
                canvas.DrawText(bracketDescriptionText, centerBracketDescription.X, centerBracketDescription.Y - textRect.MidY, SKTextAlign.Center, font, paintText);
            }
        }
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
        _startCarRailLength = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_Startschiene");
        _startCwtRailLength = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_HilfsschienenlaengeStartstueck");
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
        DistanceCWTRailShaftCeiling = cwtRailLength > 0 ? ShaftHeight - cwtRailLength : 0;

        var carRailData = CalculateRailData(carRailLength, _startCarRailLength, _carRailSplit, maxRailLength);
        var cwtRailData = CalculateRailData(cwtRailLength, _startCwtRailLength, _cwtRailSplit, maxRailLength);

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
            CwtRailBracketDistanceLeft = cwtRailLength > 0 ? cwtRailLength - firstRailBracket - customrailbracketdistance : 0;
        }
        else
        {
            if (carRailLength > 0)
            {
                if (equallyBracketSpacing > 0)
                {
                    RailBracketLevelCount = carRailLength >= cwtRailLength ? (int)Math.Floor(((carRailLength - firstRailBracket) / equallyBracketSpacing) + 1)
                                       : (int)Math.Floor(((cwtRailLength - firstRailBracket) / equallyBracketSpacing) + 1);
                    CarRailBracketDistanceLeft = carRailLength - firstRailBracket - (RailBracketLevelCount - 1) * equallyBracketSpacing;
                    CwtRailBracketDistanceLeft = cwtRailLength > 0 ? cwtRailLength - firstRailBracket - (RailBracketLevelCount - 1) * equallyBracketSpacing : 0;
                }
                else
                {
                    RailBracketLevelCount = firstRailBracket > 0 ? 1 : 0;
                    CarRailBracketDistanceLeft = carRailLength - firstRailBracket;
                    CwtRailBracketDistanceLeft = cwtRailLength - firstRailBracket;
                }
            }
            else
            {
                RailBracketLevelCount = 0;
                CarRailBracketDistanceLeft = 0;
                CwtRailBracketDistanceLeft = 0;
            }
        }

        //errors
        ErrorStartRail = _startCarRailLength > maxRailLength || _startCwtRailLength > maxRailLength;
        ErrorStandardRail = _carRailSplit > maxRailLength || _cwtRailSplit > maxRailLength;
        ErrorTotalRailLength = DistanceCarRailShaftCeiling < 0 || DistanceCWTRailShaftCeiling < 0;
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
            railCount--;
        }

        endRailLength = railLength - railCount * railSplit - startRailLength;

        if (maxRailLength >= endRailLength + railSplit)
        {
            railCount--;
            endRailLength = railLength - railCount * railSplit - startRailLength;
        }

        return (railCount, endRailLength);
    }

    private static double CalculateRailJointDistance(double railBracketLevel, double startRailLength, double railSplit, int railCount)
    {
        var railJointLevels = new List<double>
        {
            startRailLength
        };

        var currentRailJointLevel = startRailLength;

        if (railCount > 0)
        {
            for (int i = 0; i < railCount; i++)
            {
                currentRailJointLevel += railSplit;
                railJointLevels.Add(currentRailJointLevel);
            }
        }

        double minJointDistance = railSplit;

        foreach (var railJoint in railJointLevels)
        {
            if (Math.Abs(railBracketLevel - railJoint) < minJointDistance)
            {
                minJointDistance = Math.Abs(railBracketLevel - railJoint);
            }
        }
        return minJointDistance;
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

    private void UpdateFrameCalculationData()
    {
        if (FrameCalculationData is null)
        {
            return;
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_FuehrungsschieneFahrkorb"].Value))
        {
            FrameCalculationData.CarframeRail = _parametercontext.Set<GuideRails>().FirstOrDefault(x => x.Name.Contains(ParameterDictionary["var_FuehrungsschieneFahrkorb"].Value!));
        }
        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_FuehrungsschieneGegengewicht"].Value))
        {
            FrameCalculationData.CounterweightRail = _parametercontext.Set<GuideRails>().FirstOrDefault(x => x.Name.Contains(ParameterDictionary["var_FuehrungsschieneGegengewicht"].Value!));
        }

        if (FrameCalculationData != null)
        {
            ParameterDictionary["var_FrameCalculationData"].AutoUpdateParameterValue(JsonSerializer.Serialize(FrameCalculationData, _options).Replace("\r\n", "\n"));
        }
    }

    private void RestoreFrameCalculationData()
    {
        var frameCalculation = ParameterDictionary["var_FrameCalculationData"].Value;
        if (!string.IsNullOrWhiteSpace(frameCalculation))
        {
            try
            {
                var calculationData = JsonSerializer.Deserialize<FrameCalculationData>(frameCalculation);
                if (calculationData is not null)
                {
                    FrameCalculationData = calculationData;
                    DistanceGuideShoesCarframe = FrameCalculationData.DistanceGuideShoesCarframe;
                    DistanceGuideShoesCounterweight = FrameCalculationData.DistanceGuideShoesCounterweight;
                    CarCenterOfMassX = FrameCalculationData.CarCenterOfMassX;
                    CarCenterOfMassY = FrameCalculationData.CarCenterOfMassY;
                    AdditionalRailForceCarframe = FrameCalculationData.AdditionalRailForceCarframe;
                    AdditionalRailForceCounterweight = FrameCalculationData.AdditionalRailForceCounterweight;
                    OffsetCounterweightSuspensionCenter = FrameCalculationData.OffsetCounterweightSuspensionCenter;
                    CarframeBracketClipCount = FrameCalculationData.CarframeBracketClipCount;
                    CounterweightBracketClipCount = FrameCalculationData.CounterweightBracketClipCount;
                    HasSlidingClips = FrameCalculationData.HasSlidingClips;
                    BuildingDeflectionX = FrameCalculationData.BuildingDeflectionX;
                    BuildingDeflectionY = FrameCalculationData.BuildingDeflectionY;
                }
            }
            catch (Exception)
            {
                _logger.LogWarning(60202, "Restore FrameCalculationData failed");
            }
        }

        FrameCalculationData ??= new FrameCalculationData();

        if (double.TryParse(ParameterDictionary["var_Gegengewicht_Einlagenbreite"].Value, out double cwtWidth))
        {
            FrameCalculationData.CounterweightWidth = cwtWidth;
        }
        if (double.TryParse(ParameterDictionary["var_Gegengewicht_Einlagentiefe"].Value, out double cwtDepth))
        {
            FrameCalculationData.CounterweightDepth = cwtDepth;
        }
        UpdateFrameCalculationData();
    }

    private async Task UpdateCarFrameDataAsync(int delay)
    {
        await Task.Delay(delay);
        CarFrameTyp = _calculationsModuleService.GetCarFrameTyp(ParameterDictionary);
        if (CarFrameTyp is not null)
        {
            CWTRailName = CarFrameTyp.DriveTypeId == 2 ? "Führungsschienen Joch" : "Führungsschienen Gegengewicht";
            CWTRailNameShaftCeilling = CarFrameTyp.DriveTypeId == 2 ? "Jochschiene Schachtdecke" : "Gegengewichtsschiene Schachtdecke";
            IsCFPControlled = CarFrameTyp.IsCFPControlled;
        }
        CustomRailBracketSpacing = !string.IsNullOrWhiteSpace(ParameterDictionary["var_B2_1"].Value) &&
                           !string.Equals(ParameterDictionary["var_B2_1"].Value, "0");
        FillListOfRailBrackets();
        LiftParameterHelper.SetDefaultCarFrameData(ParameterDictionary, CarFrameTyp, false);
        CalculateDimensions();
        RestoreFrameCalculationData();
        RefreshView();
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
            if (!string.IsNullOrWhiteSpace(firstDistance) && !string.Equals(firstDistance, "0"))
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
    public partial PivotItem? SelectedPivotItem { get; set; }

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
        if (CurrentSpeziProperties is not null)
        {
            SetViewBoxDimensions();
            UpdateCarFrameDataAsync(500).SafeFireAndForget(onException: ex => LogTaskException(ex.ToString()));
        }
    }

    public void OnNavigatedFrom()
    {
        NavigatedFromBaseActions();
    }
}