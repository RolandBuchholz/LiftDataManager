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

    public static (SKPath, SKPath) CreateCarDoor(float x, float y, CarDoor carDoor, string entrance, float doorWidth, string openingDirection, float crossbarDepth)
    {
        var carDoorPath = new SKPath();
        var carDoorPanels = new SKPath();
        float crossbarWithClosingSide = 50f;
        float crossbarWithOpeningSide = 35f;
        float doorDimensionWidth;
        float doorDimensionDepth;

        if (openingDirection == "zentral")
        {
            doorDimensionWidth = carDoor.DoorPanelCount switch
            {
                6 => doorWidth * 1.33f + crossbarWithOpeningSide * 2f,
                4 => doorWidth * 1.5f + crossbarWithOpeningSide * 2f,
                2 => (doorWidth + crossbarWithOpeningSide) * 2f,
                _ => 0f
            };

            doorDimensionDepth = (float)crossbarDepth;
            carDoorPath.MoveTo(doorDimensionWidth * 0.5f, 0);
            carDoorPath.RLineTo(0, - doorDimensionDepth);
            carDoorPath.RLineTo(- doorDimensionWidth, 0);
            carDoorPath.RLineTo(0, doorDimensionDepth);
            carDoorPath.Close();

            carDoorPanels.MoveTo(doorDimensionWidth * 0.5f, 0f);
            carDoorPanels.RLineTo(0f, - (doorDimensionDepth + 3f));
            carDoorPanels.RLineTo(- 41f, 0f);
            carDoorPanels.RLineTo(0f, 3f);
            carDoorPanels.RLineTo(38f, 0f);
            carDoorPanels.RLineTo(0f, doorDimensionDepth + 3f);
            carDoorPanels.Close();
            carDoorPanels.MoveTo(-doorDimensionWidth * 0.5f, 0f);
            carDoorPanels.RLineTo(0f, -(doorDimensionDepth + 3f));
            carDoorPanels.RLineTo(41f, 0f);
            carDoorPanels.RLineTo(0f, 3f);
            carDoorPanels.RLineTo(-38f, 0f);
            carDoorPanels.RLineTo(0f, doorDimensionDepth + 3f);
            carDoorPanels.Close();

            //carDoorPanels.AddRect();
        }
        else
        {
            doorDimensionWidth = carDoor.DoorPanelCount switch
            {
                3 => doorWidth * 1.33f + crossbarWithClosingSide + crossbarWithOpeningSide,
                2 => doorWidth * 1.5f + crossbarWithClosingSide + crossbarWithOpeningSide,
                1 => doorWidth * 2f + crossbarWithClosingSide + crossbarWithOpeningSide,
                _ => 0f
            };

            doorDimensionDepth = (float)crossbarDepth;
            carDoorPath.MoveTo(doorWidth * 0.5f + crossbarWithClosingSide, 0);
            carDoorPath.RLineTo(0, - doorDimensionDepth);
            carDoorPath.RLineTo(- doorDimensionWidth, 0);
            carDoorPath.RLineTo(0, doorDimensionDepth);
            carDoorPath.Close();

            carDoorPanels.MoveTo(doorWidth * 0.5f + crossbarWithClosingSide, 0f);
            carDoorPanels.RLineTo(0f, -(doorDimensionDepth + 3f));
            carDoorPanels.RLineTo(-41f, 0f);
            carDoorPanels.RLineTo(0f, 3f);
            carDoorPanels.RLineTo(38f, 0f);
            carDoorPanels.RLineTo(0f, doorDimensionDepth + 3f);
            carDoorPanels.Close();
            carDoorPanels.RMoveTo(- doorDimensionWidth, 0f);
            carDoorPanels.RLineTo(0f, -(doorDimensionDepth + 3f));
            carDoorPanels.RLineTo(41f, 0f);
            carDoorPanels.RLineTo(0f, 3f);
            carDoorPanels.RLineTo(-38f, 0f);
            carDoorPanels.RLineTo(0f, doorDimensionDepth + 3f);
            carDoorPanels.Close();

            if (openingDirection == "rechts")
            {
                carDoorPath.Transform(SKMatrix.CreateScale(-1f,1f));
                carDoorPanels.Transform(SKMatrix.CreateScale(-1f, 1f));
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
        carDoorPanels.Transform(SKMatrix.CreateTranslation(x, y));
        return (carDoorPath, carDoorPanels);
    }
}
