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
        SKPath carDoorPath = new();
        SKPath carDoorPanels = new();
        SKPath carDoorPanelsMirrorImage = new();
        float crossbarWithClosingSide = 50f;
        float crossbarWithOpeningSide = 35f;
        float doorDimensionWidth;

        if (openingDirection == "zentral")
        {
            doorDimensionWidth = carDoor.DoorPanelCount switch
            {
                6 => doorWidth * 1.33f + crossbarWithOpeningSide * 2f,
                4 => doorWidth * 1.5f + crossbarWithOpeningSide * 2f,
                2 => (doorWidth + crossbarWithOpeningSide) * 2f,
                _ => 0f
            };

            carDoorPath.MoveTo(doorDimensionWidth * 0.5f, 0);
            carDoorPath.RLineTo(0, -(float)carDoor.SillWidth);
            carDoorPath.RLineTo(-doorDimensionWidth, 0);
            carDoorPath.RLineTo(0, (float)carDoor.SillWidth);
            carDoorPath.Close();

            if (crossbarDepth > 0)
            {
                carDoorPanels.MoveTo(doorDimensionWidth * 0.5f, 0f);
                carDoorPanels.RLineTo(0f, -(crossbarDepth + 3f));
                carDoorPanels.RLineTo(-41f, 0f);
                carDoorPanels.RLineTo(0f, 3f);
                carDoorPanels.RLineTo(38f, 0f);
                carDoorPanels.RLineTo(0f, crossbarDepth);
                carDoorPanels.Close();
                carDoorPanels.MoveTo(doorDimensionWidth * 0.5f - 41f, -crossbarDepth);
                carDoorPanels.RLineTo(-(doorDimensionWidth - 82f), 0f);
                carDoorPanelsMirrorImage.AddPath(carDoorPanels);
                carDoorPanelsMirrorImage.Transform(SKMatrix.CreateScale(-1f, 1f));
                carDoorPanels.AddPath(carDoorPanelsMirrorImage);
            }

            carDoorPanels.AddPath(DrawCentralDoorPanelPath(doorWidth, (float)carDoor.DoorPanelWidth, (float)carDoor.DoorPanelSpace, carDoor.DoorPanelCount, false));
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


            carDoorPath.MoveTo(doorWidth * 0.5f + crossbarWithClosingSide, 0);
            carDoorPath.RLineTo(0, -(float)carDoor.SillWidth);
            carDoorPath.RLineTo(-doorDimensionWidth, 0);
            carDoorPath.RLineTo(0, (float)carDoor.SillWidth);
            carDoorPath.Close();

            if (crossbarDepth > 0)
            {
                carDoorPanels.MoveTo(doorWidth * 0.5f + crossbarWithClosingSide, 0f);
                carDoorPanels.RLineTo(0f, -(crossbarDepth + 3f));
                carDoorPanels.RLineTo(-41f, 0f);
                carDoorPanels.RLineTo(0f, 3f);
                carDoorPanels.RLineTo(38f, 0f);
                carDoorPanels.RLineTo(0f, crossbarDepth);
                carDoorPanels.Close();
                carDoorPanels.RMoveTo(-doorDimensionWidth, 0f);
                carDoorPanels.RLineTo(0f, -(crossbarDepth + 3f));
                carDoorPanels.RLineTo(41f, 0f);
                carDoorPanels.RLineTo(0f, 3f);
                carDoorPanels.RLineTo(-38f, 0f);
                carDoorPanels.RLineTo(0f, crossbarDepth);
                carDoorPanels.Close();
                carDoorPanels.MoveTo(doorWidth * 0.5f + crossbarWithClosingSide - 41f, -crossbarDepth);
                carDoorPanels.RLineTo(-doorDimensionWidth + 41f, 0f);
            }
            carDoorPanels.AddPath(DrawSideOpeningDoorPanelPath(doorWidth, (float)carDoor.DoorPanelWidth, (float)carDoor.DoorPanelSpace, carDoor.DoorPanelCount, false));
            if (openingDirection == "rechts")
            {
                carDoorPath.Transform(SKMatrix.CreateScale(-1f, 1f));
                carDoorPanels.Transform(SKMatrix.CreateScale(-1f, 1f));
            }
        }
        var degree = entrance switch
        {
            "A" => 0f,
            "B" => 270f,
            "C" => 180f,
            "D" => 90f,
            _ => 0f
        };
        carDoorPath.Transform(SKMatrix.CreateRotationDegrees(degree));
        carDoorPanels.Transform(SKMatrix.CreateRotationDegrees(degree));

        carDoorPath.Transform(SKMatrix.CreateTranslation(x, y));
        carDoorPanels.Transform(SKMatrix.CreateTranslation(x, y));
        return (carDoorPath, carDoorPanels);
    }

    public static (SKPath, SKPath) CreateShaftDoor(float x, float y, ShaftDoor? shaftDoor, string entrance, float doorWidth, string openingDirection, string installationType)
    {
        SKPath shaftDoorPath = new();
        SKPath shaftDoorPanels = new();
        float crossbarWithClosingSide = 50f;
        float crossbarWithOpeningSide = 35f;
        float doorDimensionWidth;
        float sillgap = 30f;

        if (shaftDoor is null)
        {
            return (shaftDoorPath, shaftDoorPanels);
        }

        if (openingDirection == "zentral")
        {
            doorDimensionWidth = shaftDoor.DoorPanelCount switch
            {
                6 => doorWidth * 1.33f + crossbarWithOpeningSide * 2f,
                4 => doorWidth * 1.5f + crossbarWithOpeningSide * 2f,
                2 => (doorWidth + crossbarWithOpeningSide) * 2f,
                _ => 0f
            };

            shaftDoorPath.MoveTo(doorDimensionWidth * 0.5f, 0);
            shaftDoorPath.RLineTo(0, (float)shaftDoor.SillWidth + 4f);
            shaftDoorPath.RLineTo(-doorDimensionWidth, 0);
            shaftDoorPath.RLineTo(0, -((float)shaftDoor.SillWidth + 4f));
            shaftDoorPath.Close();
            shaftDoorPanels.AddPath(DrawCentralDoorPanelPath(doorWidth, (float)shaftDoor.DoorPanelWidth, (float)shaftDoor.DoorPanelSpace, shaftDoor.DoorPanelCount, true));
        }
        else
        {
            doorDimensionWidth = shaftDoor.DoorPanelCount switch
            {
                3 => doorWidth * 1.33f + crossbarWithClosingSide + crossbarWithOpeningSide,
                2 => doorWidth * 1.5f + crossbarWithClosingSide + crossbarWithOpeningSide,
                1 => doorWidth * 2f + crossbarWithClosingSide + crossbarWithOpeningSide,
                _ => 0f
            };


            shaftDoorPath.MoveTo(doorWidth * 0.5f + crossbarWithClosingSide, 0);
            shaftDoorPath.RLineTo(0, (float)shaftDoor.SillWidth + 4f);
            shaftDoorPath.RLineTo(-doorDimensionWidth, 0);
            shaftDoorPath.RLineTo(0, -((float)shaftDoor.SillWidth + 4f));
            shaftDoorPath.Close();
            shaftDoorPanels.AddPath(DrawSideOpeningDoorPanelPath(doorWidth, (float)shaftDoor.DoorPanelWidth, (float)shaftDoor.DoorPanelSpace, shaftDoor.DoorPanelCount, true));
        }

        float frameWidthRight = 0f;
        float frameWidthLeft = 0f;
        float frameDepth = 0f;

        switch (installationType)
        {
            case "Meiller Typ EvoN -" or "Wittur Typ -" or "Riedl Typ -":
                frameWidthRight = (float)shaftDoor.DefaultFrameWidth;
                frameWidthLeft = (float)shaftDoor.DefaultFrameWidth;
                frameDepth = (float)shaftDoor.DefaultFrameDepth;
                break;
            case "Meiller Typ EvoS -":
                frameWidthRight = openingDirection == "zentral" ? 200f : (float)shaftDoor.DefaultFrameWidth;
                frameWidthLeft = 200f;
                frameDepth = 23f;
                break;
            case "Meiller Typ EvoM -":
                frameWidthRight = 0;
                frameWidthLeft = 0;
                frameDepth = 0;
                break;
            case "Meiller Typ DT -":
                break;
            default:
                break;
        }

        SKRect doorFrame = new()
        {
            Location = new SKPoint(-(frameWidthLeft + doorWidth * 0.5f), (float)shaftDoor.SillWidth + 4f),
            Size = new SKSize(frameWidthRight + frameWidthLeft + doorWidth, frameDepth)
        };
        SKRect doorFrameRight = new()
        {
            Location = new SKPoint(doorFrame.Right - frameWidthRight, (float)shaftDoor.SillWidth + 4f),
            Size = new SKSize(frameWidthRight, frameDepth)
        };
        SKRect doorFrameLeft = new()
        {
            Location = new SKPoint(doorFrame.Left, (float)shaftDoor.SillWidth + 4f),
            Size = new SKSize(frameWidthLeft, frameDepth)
        };

        shaftDoorPath.AddRect(doorFrame);
        shaftDoorPath.AddRect(doorFrameRight);
        shaftDoorPath.AddRect(doorFrameLeft);

        shaftDoorPath.Offset(0f, sillgap);
        shaftDoorPanels.Offset(0f, sillgap);

        if (openingDirection == "rechts")
        {
            shaftDoorPath.Transform(SKMatrix.CreateScale(-1f, 1f));
            shaftDoorPanels.Transform(SKMatrix.CreateScale(-1f, 1f));
        }

        var degree = entrance switch
        {
            "A" => 0f,
            "B" => 270f,
            "C" => 180f,
            "D" => 90f,
            _ => 0f
        };
        shaftDoorPath.Transform(SKMatrix.CreateRotationDegrees(degree));
        shaftDoorPanels.Transform(SKMatrix.CreateRotationDegrees(degree));

        shaftDoorPath.Transform(SKMatrix.CreateTranslation(x, y));
        shaftDoorPanels.Transform(SKMatrix.CreateTranslation(x, y));

        return (shaftDoorPath, shaftDoorPanels);
    }

    private static SKPath DrawCentralDoorPanelPath(float doorWidth, float doorPanelDepth, float doorPanelSpace, int doorPanelCount, bool flipSPanels)
    {
        SKPath doorPanels = new();
        SKPath doorPanelsMirrorImage = new();
        float doorPanelWidth = doorWidth / doorPanelCount + 20f;

        SKRect doorPanel = new()
        {
            Size = new SKSize(doorPanelWidth, doorPanelDepth),
            Location = new SKPoint(0, -doorPanelDepth)
        };
        doorPanels.AddRect(doorPanel);

        for (int i = 1; i < doorPanelCount / 2; i++)
        {
            doorPanel.Offset(doorPanelWidth - 20f, -(doorPanelDepth + doorPanelSpace));
            doorPanels.AddRect(doorPanel);
        }
        doorPanelsMirrorImage.AddPath(doorPanels);
        doorPanelsMirrorImage.Transform(SKMatrix.CreateScale(-1f, 1f));
        doorPanels.AddPath(doorPanelsMirrorImage);

        if (flipSPanels)
        {
            doorPanels.Transform(SKMatrix.CreateScale(1f, -1f));
        }
        return doorPanels;
    }

    private static SKPath DrawSideOpeningDoorPanelPath(float doorWidth, float doorPanelDepth, float doorPanelSpace, int doorPanelCount, bool flipSPanels)
    {
        SKPath doorPanels = new();

        float doorPanelWidth = doorWidth / doorPanelCount + 20f;
        SKRect doorPanel = new()
        {
            Size = new SKSize(-doorPanelWidth, doorPanelDepth),
            Location = new SKPoint(doorWidth * 0.5f - 20f, -doorPanelDepth)
        };
        doorPanels.AddRect(doorPanel);

        for (int i = 1; i < doorPanelCount; i++)
        {
            doorPanel.Offset(-(doorPanelWidth - 20f), -(doorPanelDepth + doorPanelSpace));
            doorPanels.AddRect(doorPanel);
        }
        if (flipSPanels)
        {
            doorPanels.Transform(SKMatrix.CreateScale(1f, -1f));
        }
        return doorPanels;
    }
}
