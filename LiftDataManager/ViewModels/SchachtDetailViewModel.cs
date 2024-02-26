using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.WinUI.Animations;
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
        var canvas = e.Surface.Canvas;
        canvas.Clear();
        DrawShaftWall(canvas);
    }

    private void DrawShaftWall(SKCanvas canvas)
    {
        using var paintWall = new SKPaint
        {
            Color = SKColors.DarkGray,
            IsAntialias = true,
            
            Style = SKPaintStyle.StrokeAndFill
        };
        using var paintShaft = new SKPaint
        {
            Color = SKColors.LightGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        canvas.DrawRect(0f, 0f, (float)ShaftWidth + 500f, (float)ViewBoxHeight + 500f, paintWall);
        canvas.DrawRect(250f, 250f, (float)ShaftWidth, (float)ShaftDepth, paintShaft);
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
