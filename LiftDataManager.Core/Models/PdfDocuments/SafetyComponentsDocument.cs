using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace LiftDataManager.Core.Models.PdfDocuments;

public class SafetyComponentsDocument : PdfBaseDocument
{
    private static readonly string liftDocAccentColor = "156082";
    private static readonly string secondRowColor = Colors.Grey.Lighten3;
    private static readonly int defaultRowSpacing = 2;

    public LiftCommission LiftCommission { get; set; }

    public SafetyComponentsDocument(ObservableDictionary<string, Parameter> parameterDictionary, LiftCommission liftCommission)
    {
        Title = "Sicherheitskomponenten";
        ParameterDictionary = parameterDictionary;
        LiftCommission = liftCommission;
        SetPdfStyle(false, false);
    }

    protected override void Header(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ShowOnce().Layers(layers =>
            {
                layers.PrimaryLayer().Height(120).Width(595).Canvas((canvas, size) =>
                {
                    using var paintSecondaryColor = new SKPaint
                    {
                        Color = SKColor.Parse(secondaryColor),
                        IsAntialias = true
                    };

                    using var paintSecondaryStokeColor = new SKPaint
                    {
                        Color = SKColor.Parse(onPrimaryVariantColor),
                        StrokeWidth = 1,
                        IsStroke = true,
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(0, 0);
                    path.LineTo(0, 120);
                    path.ArcTo(325, 120, 375, 60, 30);
                    path.ArcTo(375, 60, size.Width, 60, 30);
                    path.LineTo(size.Width, 60);
                    path.LineTo(size.Width, 0);
                    path.Close();
                    canvas.DrawPath(path, paintSecondaryColor);
                });

                layers.Layer().Row(row =>
                {
                    row.ConstantItem(115).PaddingTop(10).PaddingLeft(50).Image(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "BE_Logo.png"));
                    row.AutoItem().PaddingTop(5).PaddingLeft(5, Unit.Millimetre).Column(column =>
                    {
                        column
                            .Item().PaddingBottom(-5).Text("BERCHTENBREITER GmbH")
                            .FontSize(fontSizeXXL).SemiBold().Italic().FontColor(baseHeaderColor);
                        column
                            .Item().Text("MASCHINENBAU - AUFZUGTECHNIK")
                            .FontSize(fontSizeStandard).SemiBold().Italic().FontColor(onSecondaryColor);
                        column
                            .Item().Text("Mähderweg 1a  86637 Rieblingen")
                            .FontSize(fontSizeS).FontColor(onSecondaryVariantColor);
                        column
                            .Item().Text("Telefon 08272 / 9867-0  Telefax 9867-30")
                            .FontSize(fontSizeS).FontColor(onSecondaryVariantColor);
                        column
                            .Item().Text("E-Mail: info@berchtenbreiter-gmbh.de")
                            .FontSize(fontSizeS).FontColor(onSecondaryVariantColor);
                        column
                            .Item().PaddingTop(3).Text(Title)
                            .Bold().FontSize(fontSizeXXL).FontColor(onSecondaryColor);
                    });
                    row.RelativeItem().PaddingLeft(165).Column(column =>
                    {
                        column.Item().PaddingTop(12).Row(row =>
                        {
                            row.AutoItem().Column(column =>
                            {
                                column.Item().Text("Fabriknummer:").FontSize(fontSizeXXS).FontColor(onPrimaryColor);
                                column.Item().Text(LiftCommission.Name).FontSize(fontSizeXL).FontColor(onPrimaryColor);
                            });
                        });
                    });
                });
            });

            column.Item().SkipOnce().Layers(layers =>
            {
                layers.PrimaryLayer().Height(60).Width(595).Canvas((canvas, size) =>
                {
                    using var paintSecondaryColor = new SKPaint
                    {
                        Color = SKColor.Parse(secondaryColor),
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(0, 0);
                    path.LineTo(0, 60);
                    path.ArcTo(325, 60, 375, 0, 30);
                    path.ArcTo(375, 0, size.Width, 0, 30);
                    path.Close();
                    canvas.DrawPath(path, paintSecondaryColor);
                });

                layers.Layer().Row(row =>
                {
                    row.ConstantItem(500).PaddingTop(32).PaddingLeft(25, Unit.Millimetre).Text(Title).Bold().FontSize(18).FontColor(onSecondaryColor);
                    row.RelativeItem().PaddingTop(12).Column(column =>
                    {
                        column.Item().Text("Fabriknummer:").FontSize(fontSizeXXS).FontColor(onPrimaryColor);
                        column.Item().Text(LiftCommission.Name).FontSize(fontSizeXL).FontColor(onPrimaryColor);
                    });
                });
            });
        });
    }

    protected override void Footer(IContainer container)
    {
        container.Layers(layers =>
        {
            layers.PrimaryLayer().Height(40).Width(595).Canvas((canvas, size) =>
            {
                using var paintSecondaryColor = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true
                };

                using var path = new SKPath();
                path.MoveTo(0, 40);
                path.LineTo(0, 20);
                path.ArcTo(520, 20, 540, 0, 20);
                path.ArcTo(540, 0, 595, 0, 20);
                path.LineTo(595, 0);
                path.LineTo(595, 40);
                path.Close();
                canvas.DrawPath(path, paintSecondaryColor);
            });

            layers.Layer().Row(row =>
            {
                row.AutoItem().PaddingTop(20).PaddingLeft(15, Unit.Millimetre).Text(text =>
                {
                    text.Span("www.berchtenbreiter-gmbh.de").FontColor(onSecondaryColor);
                });
                row.RelativeItem().AlignRight().PaddingTop(5).PaddingRight(4, Unit.Millimetre).Text(text =>
                {
                    text.CurrentPageNumber().Bold().FontSize(fontSizeBig).FontColor(onSecondaryColor);
                    text.Span(" / ").Bold().FontSize(fontSizeBig).FontColor(onSecondaryColor);
                    text.TotalPages().Bold().FontSize(fontSizeBig).FontColor(onSecondaryColor);
                });
            });
        });
    }

    protected override void Content(IContainer container)
    {
        container.PaddingLeft(25, Unit.Millimetre).PaddingRight(15, Unit.Millimetre).Column(column =>
        {
            column.Item().PaddingTop(5, Unit.Millimetre).Background(liftDocAccentColor).PaddingLeft(15).Text("EQUIPMENT").FontFamily(Fonts.SegoeUI).Italic().FontSize(fontSizeXXL).FontColor(Colors.White);
            column.Item().PaddingTop(15).Element(TableBaseLiftData);
            column.Item().PaddingTop(10, Unit.Millimetre).Background(liftDocAccentColor).PaddingLeft(15).Text("SICHERHEITSKOMPONENTEN").FontSize(fontSizeL).FontColor(Colors.White).Italic();
            column.Item().PaddingTop(15).Element(SafetyComponents);
            column.Item().PaddingTop(10).ShowEntire().Element(Attachments);
        });
    }

    void TableBaseLiftData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(defaultRowSpacing).Text("Montagebetrieb:").Bold();
            table.Cell().Row(1).Column(2).PaddingVertical(defaultRowSpacing).Text($"{LiftCommission.LiftInstallerID}"); 
            table.Cell().Row(2).Column(1).Background(secondRowColor).PaddingVertical(defaultRowSpacing).Text("Hersteller Nr.:").Bold();
            table.Cell().Row(2).Column(2).Background(secondRowColor).PaddingVertical(defaultRowSpacing).Text(LiftCommission.Name);
            table.Cell().Row(3).Column(1).PaddingVertical(defaultRowSpacing).Text("Equipment Nr.:").Bold();
            table.Cell().Row(3).Column(2).PaddingVertical(defaultRowSpacing).Text(LiftCommission.SAISEquipment);
            table.Cell().Row(4).Column(1).Background(secondRowColor).PaddingVertical(defaultRowSpacing).Text("Straße / Hausnr.:").Bold();
            table.Cell().Row(4).Column(2).Background(secondRowColor).PaddingVertical(defaultRowSpacing).Text($"{LiftCommission.Street} {LiftCommission.HouseNumber}");
            table.Cell().Row(5).Column(1).PaddingVertical(defaultRowSpacing).Text("PLZ / Stadt / Land:").Bold();
            table.Cell().Row(5).Column(2).PaddingVertical(defaultRowSpacing).Text($"{LiftCommission.ZIPCode} {LiftCommission.City} {LiftCommission.Country}");
        });
    }

    void SafetyComponents(IContainer container)
    {
        var liftSafetyComponents = LiftCommission.SafetyComponentRecords;
        container.Column(column =>
        {
            //foreach (var item in liftSafetyComponents)
            //{
            //    column.Item().PaddingVertical(5).Background(secondRowColor).ShowEntire().SafetyComponentTypDataField(item);
            //}
        });
    }

    void Attachments(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(10, Unit.Millimetre).Text("Der Montagebetrieb").FontSize(fontSizeStandard);
            column.Item().PaddingTop(15, Unit.Millimetre).Text($"Rieblingen, den {DateTime.Now.ToShortDateString()}").FontSize(fontSizeStandard);
        });
    }
}