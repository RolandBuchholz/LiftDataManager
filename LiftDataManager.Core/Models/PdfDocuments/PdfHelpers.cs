using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Windows.UI;

namespace LiftDataManager.Core.Models.PdfDocuments;
public static class PdfHelpers
{
    public static bool LowColor { get; set; }
    public static bool LowHighlightColor { get; set; }

    private static readonly string primaryColor = Colors.Blue.Lighten5;
    private static readonly string primaryVariantColor = Colors.Blue.Medium;
    private static readonly string secondaryColor = Colors.Grey.Lighten2;
    private static readonly string secondaryVariantColor = Colors.Grey.Darken3;
    private static readonly string background = Colors.White;
    private static readonly string onPrimaryColor = Colors.Black;
    private static readonly string onPrimaryVariantColor = Colors.White;
    private static readonly string onSecondaryColor = Colors.Black;
    private static readonly string onSecondaryVariantColor = Colors.Black;
    private static readonly string errorColor = Colors.Red.Lighten3;
    private static readonly string successfulColor = Colors.Green.Lighten3;
    private static readonly string highlightColor = Colors.Lime.Accent3;
    private static readonly string borderColor = Colors.BlueGrey.Darken3;
    private static readonly string baseHeaderColor = Colors.Red.Darken4;
    private static readonly string liftDocAccentColor = "156082";

    //FontSize
    private static readonly float fontSizeXXS = 6;
    private static readonly float fontSizeXS = 8;
    private static readonly float fontSizeS = 10;
    private static readonly float fontSizeStandard = 12;
    private static readonly float fontSizeL = 14;
    private static readonly float fontSizeXL = 16;
    private static readonly float fontSizeXXL = 18;
    private static readonly float fontSizeBig = 20;

    private static readonly string primaryColorLow = Colors.Grey.Lighten5;
    private static readonly string primaryVariantColorLow = Colors.Grey.Lighten2;
    private static readonly string secondaryColorLow = Colors.Grey.Lighten5;
    private static readonly string secondaryVariantColorLow = Colors.Grey.Lighten4;
    private static readonly string onPrimaryColorLow = Colors.Black;
    private static readonly string onPrimaryVariantColorLow = Colors.Black;
    private static readonly string onSecondaryColorLow = Colors.Black;
    private static readonly string onSecondaryVariantColorLow = Colors.Black;
    private static readonly string highlightColorLow = Colors.Grey.Lighten3;
    private static readonly string borderColorLow = Colors.Black;
    private static readonly string baseHeaderColorLow = Colors.Red.Darken4;

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
            {
                text.Line(optinaleDescription is null ? parameter.DisplayName : optinaleDescription).FontSize(fontSizeXXS).FontColor(LowColor ? borderColorLow : borderColor).Bold();
            }
            var parameterTextContent = parameter.ParameterTyp == ParameterTypValue.DropDownList ? parameter.DropDownListValue?.DisplayName
                                                                                                : parameter.Value;
            if (parameter.IsKey && LowHighlightColor)
            {
                text.Line(unit is null ? parameterTextContent : $"{parameterTextContent} {unit}").Bold().Italic();
            }
            else
            {
                text.Line(unit is null ? parameterTextContent : $"{parameterTextContent} {unit}");
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
                            Color = SKColor.Parse(LowColor ? primaryVariantColorLow : primaryVariantColor),
                            IsAntialias = true
                        };
                        using var paintOnPrimaryColor = new SKPaint
                        {
                            Color = SKColor.Parse(LowColor ? onPrimaryVariantColorLow : onPrimaryVariantColor),
                            IsAntialias = true,
                            StrokeWidth = 1,
                            StrokeCap = SKStrokeCap.Round,
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
                    using var paintSecondaryVariantColor = new SKPaint
                    {
                        Color = SKColor.Parse(secondaryVariantColor),
                        IsAntialias = true,
                        StrokeWidth = 1,
                        IsStroke = true,
                    };
                    canvas.DrawRoundRect(0, 1.75f, 8, 8, 1, 1, paintSecondaryVariantColor);
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

    public static void CheckBoxParameterValue(this IContainer container, Parameter parameter) => container
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
                        Color = SKColor.Parse(primaryVariantColor),
                        IsAntialias = true
                    };
                    using var paintOnPrimaryColor = new SKPaint
                    {
                        Color = SKColor.Parse(onPrimaryVariantColor),
                        IsAntialias = true,
                        StrokeWidth = 1,
                        StrokeCap = SKStrokeCap.Round,
                        IsStroke = true,
                    };
                    canvas.DrawRoundRect(0, 3.5f, 10, 10, 1, 1, paintPrimaryColor);
                    var skCheck = new SKPath();
                    skCheck.MoveTo(1.75f, 7.75f);
                    skCheck.LineTo(5.0f, 11.75f);
                    skCheck.LineTo(8.5f, 5.25f);
                    canvas.DrawPath(skCheck, paintOnPrimaryColor);
                });
            }
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintSecondaryVariantColor = new SKPaint
                {
                    Color = SKColor.Parse(secondaryVariantColor),
                    IsAntialias = true,
                    StrokeWidth = 1,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 3.5f, 10, 10, 1, 1, paintSecondaryVariantColor);
            });
            layers.PrimaryLayer().PaddingLeft(13).Text(parameter.DisplayName);
        });
    });

    public static void CheckBoxValue(this IContainer container, bool value, string description) => container
        .Column(column =>
        {
            column.Item().Layers(layers =>
            {
                if (value)
                {
                    layers.Layer().Canvas((canvas, size) =>
                    {
                        using var paintPrimaryColor = new SKPaint
                        {
                            Color = SKColor.Parse(primaryVariantColor),
                            IsAntialias = true
                        };
                        using var paintOnPrimaryColor = new SKPaint
                        {
                            Color = SKColor.Parse(onPrimaryVariantColor),
                            IsAntialias = true,
                            StrokeWidth = 1,
                            StrokeCap = SKStrokeCap.Round,
                            IsStroke = true,
                        };
                        canvas.DrawRoundRect(0, 3.5f, 10, 10, 1, 1, paintPrimaryColor);
                        var skCheck = new SKPath();
                        skCheck.MoveTo(1.75f, 7.75f);
                        skCheck.LineTo(5.0f, 11.75f);
                        skCheck.LineTo(8.5f, 5.25f);
                        canvas.DrawPath(skCheck, paintOnPrimaryColor);
                    });
                }
                layers.Layer().Canvas((canvas, size) =>
                {
                    using var paintSecondaryVariantColor = new SKPaint
                    {
                        Color = SKColor.Parse(secondaryVariantColor),
                        IsAntialias = true,
                        StrokeWidth = 1,
                        IsStroke = true,
                    };
                    canvas.DrawRoundRect(0, 3.5f, 10, 10, 1, 1, paintSecondaryVariantColor);
                });
                layers.PrimaryLayer().PaddingLeft(13).Text(description);
            });
        });

    public static void ProtectedSpaceTypInfoBox(this IContainer container, string position, ProtectedSpaceTyp protectedSpaceTyp) => container
        .Width(140).Column(column =>
        {
            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TechnicalLiftDocumentation.GetProtectedSpaceTypImage(protectedSpaceTyp).TrimStart('/'));
            var protectedSpaceTypDescription = TechnicalLiftDocumentation.GetProtectedSpaceTypDescription(protectedSpaceTyp);

            column.Item().AlignCenter().Text($"Schutzraum {position}").FontSize(fontSizeS).Bold();
            column.Item().PaddingHorizontal(20).Width(100).Image(imagePath);
            column.Item().AlignCenter().Text(protectedSpaceTypDescription).FontSize(fontSizeS);
        });

    public static void SafetyComponentTypDataField(this IContainer container, LiftSafetyComponent safetyComponentTyp) => container
    .Table(table =>
    {
        table.ColumnsDefinition(columns =>
        {
            columns.RelativeColumn();
            columns.RelativeColumn();
            columns.RelativeColumn();
        });
        table.Cell().Row(1).Column(1).PaddingLeft(5).Text(safetyComponentTyp.SafetyType).FontColor(liftDocAccentColor).Bold();
        table.Cell().Row(1).Column(2).Text(safetyComponentTyp.Manufacturer);
        table.Cell().Row(1).Column(3).AlignMiddle().AlignCenter().Text("EU-Baumusterprüfbescheinigung").FontColor(secondaryVariantColor).FontSize(fontSizeS).Bold();
        table.Cell().Row(2).Column(1).PaddingLeft(5).Text(safetyComponentTyp.SafetyComponentTyp).FontColor(secondaryVariantColor).FontSize(fontSizeS).Bold();
        table.Cell().Row(2).Column(2).Text(safetyComponentTyp.Model);
        table.Cell().Row(2).Column(3).RowSpan(2).AlignMiddle().AlignCenter().Text(safetyComponentTyp.CertificateNumber).Bold();
        table.Cell().Row(3).Column(1).ColumnSpan(2).PaddingLeft(25).Text(safetyComponentTyp.SpecialOption).FontSize(fontSizeS).Italic().Bold();
    });
}
