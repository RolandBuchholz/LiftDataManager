using CommunityToolkit.Mvvm.Messaging.Messages;
using SkiaSharp;

namespace LiftDataManager.ViewModels;

public partial class SchachtDetailViewModel : DataViewModelBase, INavigationAware, IRecipient<PropertyChangedMessage<string>>, IRecipient<PropertyChangedMessage<bool>>
{
    public SchachtDetailViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
     base(parameterDataService, dialogService, navigationService)
    {
    }

    [ObservableProperty]
    private double viewBoxWidth;

    [ObservableProperty]
    private double viewBoxHeight;

    [ObservableProperty]
    private double shaftWidth;

    [ObservableProperty]
    private double shaftDepth;

    [RelayCommand]
    private void OnPaintSurface(SkiaSharp.Views.Windows.SKPaintSurfaceEventArgs e)
    {
        //SKImageInfo info = e.Info;
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        canvas.Clear();
        DrawShaftWall(canvas);
    }

    private void DrawShaftWall(SKCanvas canvas)
    {
        float hatchStokeWith = ((float)shaftWidth + (float)shaftDepth) / 400f;

        SKPathEffect diagLinesPath = SKPathEffect.Create2DLine(hatchStokeWith,
        SkiaSharpHelpers.Multiply(SKMatrix.CreateScale(hatchStokeWith * 10, hatchStokeWith * 10), SKMatrix.CreateRotationDegrees(45)));
        SKRect shaftHatch = new(-hatchStokeWith * 10, -hatchStokeWith * 10, (float)ViewBoxWidth, (float)ViewBoxHeight);
        SKRect shaftWall = new(0f, 0f, (float)ViewBoxWidth, (float)ViewBoxHeight);

        using var paintWallSolid = new SKPaint
        {
            Color = SKColors.DarkGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        using var paintHatch = new SKPaint
        {
            Color = SKColors.DarkRed,
            PathEffect = diagLinesPath,
            IsAntialias = true,
            IsStroke = true,
            Style = SKPaintStyle.Stroke
        };
        using var paintHatchOutline = new SKPaint
        {
            Color = SKColors.DarkRed,
            IsAntialias = true,
            StrokeWidth= hatchStokeWith * 2,
            Style = SKPaintStyle.Stroke
        };
        using var paintShaft = new SKPaint
        {
            Color = SKColors.LightGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.ClipRect(shaftWall);
        canvas.DrawRect(shaftWall, paintWallSolid);
        canvas.DrawRect(shaftHatch, paintHatch);
        canvas.DrawRect(shaftWall, paintHatchOutline);
        canvas.DrawRect(250f, 250f, (float)ShaftWidth, (float)ShaftDepth, paintShaft);
        canvas.DrawRect(250f, 250f, (float)ShaftWidth, (float)ShaftDepth, paintHatchOutline);
    }

    public void RefreshView()
    {
        //xamlCanvas.Invalidate();
    }

    private void SetViewBoxDimensions()
    {
        ShaftWidth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_SB");
        ShaftDepth = LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_ST");
        if (ShaftWidth > 0 && ShaftDepth > 0)
        {
            ViewBoxWidth = (ShaftWidth + 500);
            ViewBoxHeight = (ShaftDepth + 500);
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
