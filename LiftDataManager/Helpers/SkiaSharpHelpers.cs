using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;
using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
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
        {
            arrow.Transform(SKMatrix.CreateRotationDegrees(degrees));
        }
        arrow.Transform(SKMatrix.CreateTranslation(x, y));
        return arrow;
    }
    public static SKMatrix Multiply(SKMatrix first, SKMatrix second)
    {
        SKMatrix target = SKMatrix.CreateIdentity();
        SKMatrix.Concat(ref target, first, second);
        return target;
    }
    public static SKPath CreateGuideRail(float x, float y, GuideRails guideRail, float degrees = 0)
    {
        var rail = new SKPath();
        rail.RMoveTo(0, 0);
        rail.RLineTo(0, -(float)(guideRail.RailHead * 0.5));
        rail.RLineTo(-(float)(guideRail.Width - guideRail.ThicknessF), 0);
        rail.RLineTo(0, -(float)((guideRail.Height - guideRail.RailHead) * 0.5));
        rail.RLineTo(-(float)guideRail.ThicknessF, 0);
        rail.RLineTo(0, (float)guideRail.Height);
        rail.RLineTo((float)guideRail.ThicknessF, 0);
        rail.RLineTo(0, -(float)((guideRail.Height - guideRail.RailHead) * 0.5));
        rail.RLineTo((float)(guideRail.Width - guideRail.ThicknessF), 0);
        rail.Close();
        if (degrees != 0)
        {
            rail.Transform(SKMatrix.CreateRotationDegrees(degrees));
        }
        rail.Transform(SKMatrix.CreateTranslation(x, y));
        return rail;
    }

    public static (SKPath, SKPath) CreateCarDoor(float x, float y, CarDoor carDoor, string entrance, float doorWidth, string openingDirection)
    {
        var carDoorPath = new SKPath();
        var carDoorPanels = new SKPath();
        if (openingDirection == "zentral")
        {
            carDoorPath.RMoveTo(doorWidth * 0.5f, 0);
            carDoorPath.RLineTo(0, -(float)carDoor.SillWidth);
            carDoorPath.RLineTo(-doorWidth * 1.5f, 0);
            carDoorPath.RLineTo(0, (float)carDoor.SillWidth);
            carDoorPath.Close();
        }
        else
        {
            carDoorPath.RMoveTo(doorWidth * 0.5f, 0);
            carDoorPath.RLineTo(0, -(float)carDoor.SillWidth);
            carDoorPath.RLineTo(-doorWidth * 1.5f, 0);
            carDoorPath.RLineTo(0, (float)carDoor.SillWidth);
            carDoorPath.Close();
            if (openingDirection == "rechts")
            {
                carDoorPath.Transform(SKMatrix.CreateScale(-1f,1f));
            }
        }

        var degree = entrance switch
        {
            "A"=> 0f,
            "B"=> 270f,
            "C"=> 180f,
            "D" => 90f,
            _ => 0f
        };
        carDoorPath.Transform(SKMatrix.CreateRotationDegrees(degree));
        carDoorPanels.Transform(SKMatrix.CreateRotationDegrees(degree));

        carDoorPath.Transform(SKMatrix.CreateTranslation(x, y));
        return (carDoorPath, carDoorPanels);
    }
}
