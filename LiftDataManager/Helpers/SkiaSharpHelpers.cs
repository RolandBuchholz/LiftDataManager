using SkiaSharp;

namespace LiftDataManager.Helpers;
public static class SkiaSharpHelpers
{
    public static SKPath CreateArrow(float x, float y, float degrees = 0)
    {
        var arrow = new SKPath();
        arrow.RMoveTo(-75, -10);
        arrow.RLineTo(0, 20);
        arrow.RLineTo(75, 0);
        arrow.RLineTo(0, 30);
        arrow.RLineTo(75, -40);
        arrow.RLineTo(-75, -40);
        arrow.RLineTo(0, 30);
        arrow.Close();
        if (degrees != 0)
            arrow.Transform(SKMatrix.CreateRotationDegrees(degrees));
        arrow.Transform(SKMatrix.CreateTranslation(x, y));

        return arrow;
    }
    public static SKMatrix Multiply(SKMatrix first, SKMatrix second)
    {
        SKMatrix target = SKMatrix.CreateIdentity();
        SKMatrix.Concat(ref target, first, second);
        return target;
    }
}
