using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace LiftDataManager.Core.Models.PdfDocuments;
public static class PdfHelpers
{
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

    //FontSize
    static float fontSizeXXS = 6;
    static float fontSizeXS = 8;
    static float fontSizeS = 10;
    static float fontSizeStandard = 12;
    static float fontSizeL = 14;
    static float fontSizeXL = 16;
    static float fontSizeXXL = 18;
    static float fontSizeBig = 20;


    public static PdfStyleSet GetPdfStyleSet()
    {
        return new PdfStyleSet()
        {
            primaryColor = primaryColor,
            primaryVariantColor = primaryVariantColor,
            secondaryColor = secondaryColor,
            secondaryVariantColor = secondaryVariantColor,
            background = background,
            onPrimaryColor = onPrimaryColor,
            onPrimaryVariantColor = onPrimaryVariantColor,
            onSecondaryColor = onSecondaryColor,
            onSecondaryVariantColor = onSecondaryVariantColor,
            errorColor = errorColor,
            successfulColor = successfulColor,
            highlightColor = highlightColor,
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
        .Border(hideBorder ? 0 : 0.1f)
        .BorderColor(secondaryColor)
        .PaddingLeft(5).PaddingTop(0)
        .PaddingBottom(-10).Text(text => 
    {
        if (!hideHeader) text.Line(optinaleDescription is null ? parameter.DisplayName : optinaleDescription).FontSize(fontSizeXXS).FontColor(secondaryColor).Bold();
        text.Line(unit is null ? parameter.Value : $"{parameter.Value} {unit}");
    });

    public static void ParameterDateCell(this IContainer container, Parameter parameter, bool hideHeader = false, bool hideBorder = false, string? optinaleDescription = null) => container
        .Border(hideBorder ? 0 : 0.1f)
        .BorderColor(secondaryColor)
        .PaddingLeft(5)
        .PaddingTop(0)
        .PaddingBottom(-10)
        .Text(text =>
    {
        if (!hideHeader) text.Line(optinaleDescription is null? parameter.DisplayName : optinaleDescription).FontSize(fontSizeXXS).FontColor(secondaryColor).Bold();
        text.Line(LiftParameterHelper.ConvertExcelDate(parameter.Value));
    });

    public static void ParameterBoolCell(this IContainer container, Parameter parameter, bool hideBorder = false, string? optinaleDescription = null) => container
        .Border(hideBorder ? 0 : 0.1f)
        .BorderColor(secondaryColor)
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
                            Color = SKColor.Parse(primaryColor),
                            IsAntialias = true
                        };
                        using var paintOnPrimaryVariantColor = new SKPaint
                        {
                            Color = SKColor.Parse(onPrimaryVariantColor),
                            IsAntialias = true,
                            StrokeWidth = 1,
                            IsStroke = true,
                        };
                        canvas.DrawRoundRect(0, 1.75f, 8, 8, 1, 1, paintPrimaryColor);
                        var skCheck = new SKPath();
                        skCheck.MoveTo(1.5f, 5.25f);
                        skCheck.LineTo(3.25f, 7.75f);
                        skCheck.LineTo(6.5f, 3.25f);
                        canvas.DrawPath(skCheck, paintOnPrimaryVariantColor);
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

                layers.PrimaryLayer().PaddingLeft(12).Text(optinaleDescription is null ? parameter.DisplayName : optinaleDescription);
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
       container.ParameterBoolCell(parameter,hideBorder, customvalue);
    }
}
