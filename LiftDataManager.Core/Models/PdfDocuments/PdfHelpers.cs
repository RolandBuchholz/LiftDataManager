using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace LiftDataManager.Core.Models.PdfDocuments;
public static class PdfHelpers
{
    public static bool LowColor { get; set; }
    public static bool LowHighlightColor { get; set; }

    static string primaryColor = Colors.Pink.Accent3;
    static string primaryVariantColor = Colors.Pink.Lighten2;
    static string secondaryColor = Colors.BlueGrey.Darken3;
    static string secondaryVariantColor = Colors.BlueGrey.Lighten3;
    static string background = Colors.White;
    static string onPrimaryColor = Colors.White;
    static string onPrimaryVariantColor = Colors.White;
    static string onSecondaryColor = Colors.White;
    static string onSecondaryVariantColor = Colors.White;
    static string errorColor = Colors.Red.Lighten3;
    static string successfulColor = Colors.Green.Lighten3;
    static string highlightColor = Colors.Lime.Accent3;
    static string borderColor = Colors.BlueGrey.Darken3;
    static string baseHeaderColor = Colors.Pink.Accent3;

    //FontSize
    static float fontSizeXXS = 6;
    static float fontSizeXS = 8;
    static float fontSizeS = 10;
    static float fontSizeStandard = 12;
    static float fontSizeL = 14;
    static float fontSizeXL = 16;
    static float fontSizeXXL = 18;
    static float fontSizeBig = 20;

    static string primaryColorLow = Colors.Grey.Lighten5;
    static string primaryVariantColorLow = Colors.Grey.Lighten1;
    static string secondaryColorLow = Colors.Grey.Lighten3;
    static string secondaryVariantColorLow = Colors.Grey.Lighten4;
    static string onPrimaryColorLow = Colors.Black;
    static string onPrimaryVariantColorLow = Colors.Black;
    static string onSecondaryColorLow = Colors.Black;
    static string onSecondaryVariantColorLow = Colors.Black;
    static string highlightColorLow = Colors.Grey.Lighten3;
    static string borderColorLow = Colors.Black;
    static string baseHeaderColorLow = Colors.Red.Darken4;

    public static PdfStyleSet GetPdfStyleSet(bool lowColor, bool lowHighlightColor)
    {
        LowColor = lowColor;
        LowHighlightColor = lowHighlightColor;
        return new PdfStyleSet()
        {
            primaryColor = LowColor ? primaryColorLow : primaryColor,
            primaryVariantColor = LowColor ? primaryVariantColorLow : primaryVariantColor,
            secondaryColor = LowColor ? secondaryColorLow : secondaryColor,
            secondaryVariantColor = LowColor ? secondaryVariantColorLow : secondaryVariantColor,
            background = background,
            onPrimaryColor = LowColor ? onPrimaryColorLow : onPrimaryColor,
            onPrimaryVariantColor = LowColor ? onPrimaryVariantColorLow : onPrimaryVariantColor,
            onSecondaryColor = LowColor ? onSecondaryColorLow : onSecondaryColor,
            onSecondaryVariantColor = LowColor ? onSecondaryVariantColorLow : onSecondaryVariantColor,
            errorColor = errorColor,
            successfulColor = successfulColor,
            highlightColor = lowHighlightColor ? highlightColorLow : highlightColor,
            borderColor = LowColor ? borderColorLow : borderColor,
            baseHeaderColor = LowColor ? baseHeaderColorLow : baseHeaderColor,
            fontSizeXXS = fontSizeXXS,
            fontSizeXS = fontSizeXS,
            fontSizeS = fontSizeS,
            fontSizeStandard = fontSizeStandard,
            fontSizeL = fontSizeL,
            fontSizeXL = fontSizeXL,
            fontSizeXXL = fontSizeXXL,
            fontSizeBig = fontSizeBig,
        };
    }

    static IContainer Cell(this IContainer container, bool background)
    {
        return container
            .Border(0.5f)
            .BorderColor(Colors.Grey.Lighten1)
            .Background(background ? Colors.Grey.Lighten4 : Colors.White)
            .PaddingLeft(5);
    }

    public static IContainer ValueCell(this IContainer container)
    {
        return container.Cell(false);
    }

    public static IContainer LabelCell(this IContainer container)
    {
        return container.Cell(true);
    }

    public static void ValueCell(this IContainer container, string text) => container.ValueCell().Text(text);

    public static void ParameterStringCell(this IContainer container, Parameter parameter, string? unit = null, bool hideHeader = false, bool hideBorder = false, string? optinaleDescription = null) => container
        .Background(parameter.IsKey ? LowHighlightColor ? highlightColorLow : highlightColor : Colors.Transparent)
        .Border(hideBorder ? 0 : 0.1f)
        .BorderColor(LowColor ? borderColorLow : borderColor)
        .PaddingLeft(5).PaddingTop(0)
        .PaddingBottom(-10).Text(text =>
    {
        if (!hideHeader)
            text.Line(optinaleDescription is null ? parameter.DisplayName : optinaleDescription).FontSize(fontSizeXXS).FontColor(LowColor ? borderColorLow : borderColor).Bold();
        if (parameter.IsKey && LowHighlightColor)
        {
            text.Line(unit is null ? parameter.Value : $"{parameter.Value} {unit}").Bold().Italic();
        }
        else
        {
            text.Line(unit is null ? parameter.Value : $"{parameter.Value} {unit}");
        }
    });

    public static void ParameterDateCell(this IContainer container, Parameter parameter, bool hideHeader = false, bool hideBorder = false, string? optinaleDescription = null) => container
        .Background(parameter.IsKey ? LowHighlightColor ? highlightColorLow : highlightColor : Colors.Transparent)
        .Border(hideBorder ? 0 : 0.1f)
        .BorderColor(LowColor ? borderColorLow : borderColor)
        .PaddingLeft(5)
        .PaddingTop(0)
        .PaddingBottom(-10)
        .Text(text =>
    {
        if (!hideHeader)
            text.Line(optinaleDescription is null ? parameter.DisplayName : optinaleDescription).FontSize(fontSizeXXS).FontColor(LowColor ? borderColorLow : borderColor).Bold();
        if (parameter.IsKey && LowHighlightColor)
        {
            text.Line(parameter.Value).Bold().Italic();
        }
        else
        {
            text.Line(parameter.Value);
        }
    });

    public static void ParameterBoolCell(this IContainer container, Parameter parameter, bool hideBorder = false, string? optinaleDescription = null) => container
        .Background(parameter.IsKey ? LowHighlightColor ? highlightColorLow : highlightColor : Colors.Transparent)
        .Border(hideBorder ? 0 : 0.1f)
        .BorderColor(LowColor ? borderColorLow : borderColor)
        .PaddingLeft(5)
        .Column(column =>
        {
            column.Item().Layers(layers =>
            {
                if (Convert.ToBoolean(parameter.Value))
                {
                    layers.Layer().Canvas((canvas, size) =>
                    {
                        using var paintPrimaryColor = new SKPaint
                        {
                            Color = SKColor.Parse(LowColor ? primaryColorLow : primaryColor),
                            IsAntialias = true
                        };
                        using var paintOnPrimaryColor = new SKPaint
                        {
                            Color = SKColor.Parse(LowColor ? onPrimaryColorLow : onPrimaryColor),
                            IsAntialias = true,
                            StrokeWidth = 1,
                            IsStroke = true,
                        };
                        canvas.DrawRoundRect(0, 1.75f, 8, 8, 1, 1, paintPrimaryColor);
                        var skCheck = new SKPath();
                        skCheck.MoveTo(1.5f, 5.25f);
                        skCheck.LineTo(3.25f, 7.75f);
                        skCheck.LineTo(6.5f, 3.25f);
                        canvas.DrawPath(skCheck, paintOnPrimaryColor);
                    });
                }
                layers.Layer().Canvas((canvas, size) =>
                {
                    using var paintSecondaryColor = new SKPaint
                    {
                        Color = SKColor.Parse(secondaryColor),
                        IsAntialias = true,
                        StrokeWidth = 1,
                        IsStroke = true,
                    };
                    canvas.DrawRoundRect(0, 1.75f, 8, 8, 1, 1, paintSecondaryColor);
                });
                if (parameter.IsKey && LowHighlightColor)
                {
                    layers.PrimaryLayer().PaddingLeft(12).Text(optinaleDescription is null ? parameter.DisplayName : optinaleDescription).Bold().Italic();
                }
                else
                {
                    layers.PrimaryLayer().PaddingLeft(12).Text(optinaleDescription is null ? parameter.DisplayName : optinaleDescription);
                }
            });
        });

    public static void ParameterCustomBoolCell(this IContainer container, Parameter parameter, bool hideBorder = false)
    {
        var customvalue = string.Empty;
        if (!string.IsNullOrWhiteSpace(parameter.Comment))
        {
            if (parameter.Comment == "Benutzerdefinierte Variable: 0")
            {
                customvalue = string.Empty;
            }
            else
            {
                customvalue = parameter.Comment.Replace("Benutzerdefinierte Variable: ", "");
            }
        }
        container.ParameterBoolCell(parameter, hideBorder, customvalue);
    }
}
