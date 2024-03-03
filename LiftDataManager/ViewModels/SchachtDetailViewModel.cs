using CommunityToolkit.Mvvm.Messaging.Messages;
using SkiaSharp;
using SkiaSharp.Views.Windows;

namespace LiftDataManager.ViewModels;

public partial class SchachtDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public SchachtDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
     base(parameterDataService, dialogService, navigationService)
    {
    }

    private readonly string[] shaftDesignParameter = { "var_ZUGANSSTELLEN_A", "var_ZUGANSSTELLEN_B",
                                                       "var_ZUGANSSTELLEN_C", "var_ZUGANSSTELLEN_D",
                                                       "var_MauerOeffnungBreiteA", "var_MauerOeffnungAbstandA",
                                                       "var_MauerOeffnungBreiteB", "var_MauerOeffnungAbstandB",
                                                       "var_MauerOeffnungBreiteC", "var_MauerOeffnungAbstandC",
                                                       "var_MauerOeffnungBreiteD", "var_MauerOeffnungAbstandD"};

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

        shaftWallHatch.Inflate(100f,100f);

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
            StrokeWidth= _stokeWith * 2,
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
        canvas.ClipRect(shaftWallOutSide);
        canvas.DrawRect(shaftWallHatch, paintHatch);
        canvas.DrawRect(shaftWallInSide, paintShaft);
        canvas.DrawRect(shaftWallInSide, paintHatchOutline);
  
    }

    private void DrawEntranceWall(SKCanvas canvas)
    {
        string[] entrances = new[] { "A","B","C","D" };

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

    public void OnNavigatedTo(object parameter)
    {
        IsActive = true;
        SynchronizeViewModelParameter();
        if (CurrentSpeziProperties is not null &&
            CurrentSpeziProperties.ParameterDictionary is not null &&
            CurrentSpeziProperties.ParameterDictionary.Values is not null)
        {
            _ = SetModelStateAsync();
            SetViewBoxDimensions();
        }
    }

    public void OnNavigatedFrom()
    {
        IsActive = false;
    }
}
