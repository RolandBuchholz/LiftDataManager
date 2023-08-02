using Cogs.Collections;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace LiftDataManager.Core.Models.PdfDocuments;
public class CarDesignComponent : IComponent
{
    public ObservableDictionary<string, Parameter> ParameterDictionary { get; set; }
    public bool LowPrintColor { get; set; }

    float canvasWidth;
    float canvasHeight;
    float kabinenbreite;
    float kabinentiefe;
    bool zugangA;
    bool zugangB;
    bool zugangC;
    bool zugangD;
    float tuerbreiteA;
    float tuerbreiteB;
    float tuerbreiteC;
    float tuerbreiteD;
    float halsL1;
    float halsL2;
    float halsL3;
    float halsL4;
    string? flaeche;
    string? personen;


    public CarDesignComponent(ObservableDictionary<string, Parameter> parameterDictionary, bool lowPrintColor)
    {
        ParameterDictionary = parameterDictionary;
        LowPrintColor = lowPrintColor;
        SetCarDesignParameter();
    }

    private void SetCarDesignParameter()
    {
        kabinenbreite = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KBI");
        kabinentiefe = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_KTI");
        zugangA = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_A");
        zugangB = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_B");
        zugangC = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_C");
        zugangD = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_D");
        tuerbreiteA = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB");
        tuerbreiteB = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB_B");
        tuerbreiteC = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB_C");
        tuerbreiteD = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_TB_D");
        halsL1 = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L1");
        halsL2 = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L2");
        halsL3 = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L3");
        halsL4 = (float)LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_L4");
        flaeche = string.IsNullOrWhiteSpace(LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_A_Kabine")) ? "0"
                                                               : LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_A_Kabine");
        personen = string.IsNullOrWhiteSpace(LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Personen")) ? "0"
                                                                : LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Personen");

        if (kabinenbreite <= 0)
            kabinenbreite = 1100;

        if (kabinentiefe <= 0)
            kabinentiefe = 1400;

        canvasWidth = kabinenbreite + 200;
        canvasHeight = kabinentiefe + 200;
    }

    public void Compose(IContainer container)
    {
        container.ScaleToFit().Element(LayerCarDesign);
    }

    void LayerCarDesign(IContainer container)
    {
        container.Height(canvasHeight).Width(canvasWidth).Layers(layers =>
        {
            layers.PrimaryLayer().Canvas((canvas, size) =>
            {
                canvas.Clear();
                using var paintBase = new SKPaint
                {
                    Color = SKColor.Parse(LowPrintColor ? Colors.Grey.Darken4 : Colors.BlueGrey.Medium),
                    IsStroke = LowPrintColor,
                    StrokeWidth = 7,
                    IsAntialias = true
                };

                using var paintBlueDoor = new SKPaint
                {
                    Color = SKColor.Parse(LowPrintColor ? Colors.Grey.Darken4 : Colors.BlueGrey.Lighten1),
                    IsStroke = LowPrintColor,
                    StrokeWidth = 7,
                    IsAntialias = true
                };

                var carBase = new SKRect()
                {
                    Size = new SKSize(kabinenbreite, kabinentiefe),
                };

                carBase.Offset(100, 100);

                canvas.DrawRect(carBase, paintBase);

                if (zugangA)
                {
                    canvas.DrawRect(100 + halsL1, kabinentiefe + 100, tuerbreiteA, 100, paintBlueDoor);
                }
                if (zugangB)
                {
                    canvas.DrawRect(100 + kabinenbreite, kabinentiefe + 100 - halsL3, 100, -tuerbreiteB, paintBlueDoor);
                }
                if (zugangC)
                {
                    canvas.DrawRect(100 + (kabinenbreite - halsL2), 0, -tuerbreiteC, 100, paintBlueDoor);
                }
                if (zugangD)
                {
                    canvas.DrawRect(100, halsL4 + 100, -100, tuerbreiteD, paintBlueDoor);
                }
            });

            layers.Layer().AlignMiddle().Column(colum =>
            {
                colum.Item().AlignCenter().Text($"{flaeche} m²").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(120);
                colum.Item().AlignCenter().Text($"{personen} Personen").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(120);
            });

            layers.Layer().ShowIf(zugangA).AlignBottom().Column(colum =>
            {
                colum.Item().PaddingBottom(-20).Row(row =>
                {
                    row.AutoItem().PaddingLeft(100).Text("L1").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.RelativeItem().AlignCenter().Text("TB (A)").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.AutoItem().AlignRight().PaddingRight(100).Text("R1").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                });
                colum.Item().PaddingBottom(-20).Row(row =>
                {
                    row.ConstantItem(100 + halsL1).AlignRight().Padding(5).Text($"{halsL1}").FontColor(Colors.Black).FontSize(80);
                    row.RelativeItem().AlignCenter().Text($"{tuerbreiteA}").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.ConstantItem(100 + kabinenbreite - tuerbreiteA - halsL1).Padding(5).Text($"{kabinenbreite - tuerbreiteA - halsL1}").FontColor(Colors.Black).FontSize(80);
                });
            });

            layers.Layer().ShowIf(zugangB).AlignBottom().AlignRight().RotateLeft().Column(colum =>
            {
                colum.Item().PaddingBottom(-30).Row(row =>
                {
                    row.AutoItem().PaddingLeft(200).Text("L3").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.RelativeItem().AlignCenter().Text("TB (C)").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.AutoItem().AlignRight().PaddingRight(200).Text("R3").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                });
                colum.Item().PaddingBottom(-10).Row(row =>
                {
                    row.ConstantItem(100 + halsL3).AlignRight().Padding(5).Text($"{halsL3}").FontColor(Colors.Black).FontSize(80);
                    row.RelativeItem().AlignCenter().Text($"{tuerbreiteB}").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.ConstantItem(100 + kabinentiefe - tuerbreiteB - halsL3).Padding(5).Text($"{kabinentiefe - tuerbreiteB - halsL3}").FontColor(Colors.Black).FontSize(80);
                });
            });

            layers.Layer().ShowIf(zugangC).AlignTop().Column(colum =>
            {
                colum.Item().PaddingTop(-10).Row(row =>
                {
                    row.ConstantItem(100 + kabinenbreite - tuerbreiteC - halsL2).AlignRight().Padding(5).Text($"{kabinenbreite - tuerbreiteC - halsL2}").FontColor(Colors.Black).FontSize(80);
                    row.RelativeItem().AlignCenter().Text($"{tuerbreiteC}").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.ConstantItem(100 + halsL2).AlignLeft().Padding(5).Text($"{halsL2}").FontColor(Colors.Black).FontSize(80);
                });
                colum.Item().PaddingTop(-30).Row(row =>
                {
                    row.AutoItem().PaddingLeft(100).Text("R2").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.RelativeItem().AlignCenter().Text("TB (B)").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.AutoItem().AlignRight().PaddingRight(100).Text("L2").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                });
            });

            layers.Layer().ShowIf(zugangD).AlignBottom().RotateLeft().Column(colum =>
            {
                colum.Item().PaddingBottom(-10).Row(row =>
                {
                    row.ConstantItem(100 + kabinentiefe - tuerbreiteD - halsL4).AlignRight().Padding(5).Text($"{kabinentiefe - tuerbreiteD - halsL4}").FontColor(Colors.Black).FontSize(80);
                    row.RelativeItem().AlignCenter().Text($"{tuerbreiteD}").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.ConstantItem(100 + halsL4).AlignLeft().Padding(5).Text($"{halsL4}").FontColor(Colors.Black).FontSize(80);

                });
                colum.Item().PaddingTop(-30).Row(row =>
                {
                    row.AutoItem().PaddingLeft(200).Text("R4").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.RelativeItem().AlignCenter().Text("TB (D)").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                    row.AutoItem().AlignRight().PaddingRight(200).Text("L4").FontColor(LowPrintColor ? Colors.Black : Colors.White).FontSize(80);
                });

            });
        });
    }
}





