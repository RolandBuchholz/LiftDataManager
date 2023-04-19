using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.CalculationResultsModels;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Colors = QuestPDF.Helpers.Colors;

namespace PDFTests.Services.DocumentGeneration;

public class NutzlastberechnungDocument : IDocument
{
    private readonly ICalculationsModule _calculationsModuleService;
    public ObservableDictionary<string, Parameter> ParameterDictionary { get; set; }
    public PayLoadResult CarVentilationResult = new();

    public static string Title => "Nutzfläche des Fahrkorbs";
    public string Index => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Index");
    public string AuftragsNummer => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_AuftragsNummer");
    public string FabrikNummer => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_FabrikNummer");
    public string Kennwort => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Kennwort");
    private readonly string user;

    public NutzlastberechnungDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService)
    {
        _calculationsModuleService = calculationsModuleService;
        ParameterDictionary = parameterDictionary;
        CarVentilationResult = _calculationsModuleService.GetPayLoadCalculation(parameterDictionary);
        user = string.IsNullOrWhiteSpace(System.Security.Principal.WindowsIdentity.GetCurrent().Name) ? "no user detected" : System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("PPS\\", "");
    }

    public DocumentMetadata GetMetadata() => new()
    {
        ImageQuality = 101,
        RasterDpi = 72,
        Title = $"{AuftragsNummer}-{Title}",
        Author = user,
        Subject = "Berechnung",
        Keywords = "#LiftDataManager#Berechnung",
        ModifiedDate = DateTime.Now,
        CreationDate = DateTime.Now,
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.MarginTop(0, Unit.Millimetre);
            page.MarginBottom(0, Unit.Millimetre);
            page.MarginLeft(0, Unit.Millimetre);
            page.MarginRight(0, Unit.Millimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(12));
            page.Header().Element(Header);
            page.Content().Element(Content);
            page.Footer().Element(Footer);

        });
    }

    void Header(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().ShowOnce().Layers(layers =>
            {
                layers.Layer().Canvas((canvas, size) =>
                {
                    using var paintRed = new SKPaint
                    {
                        Color = SKColor.Parse(Colors.Red.Accent3),
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(250, 0);
                    path.LineTo(250, 90);
                    path.ArcTo(size.Width, 90, size.Width, 0, 10);
                    path.LineTo(size.Width, 0);
                    path.Close();

                    canvas.DrawPath(path, paintRed);
                });

                layers.PrimaryLayer().Height(120).Width(595).Canvas((canvas, size) =>
                {
                    using var paintGrey = new SKPaint
                    {
                        Color = SKColor.Parse(Colors.Grey.Darken3),
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(0, 0);
                    path.LineTo(0, 120);
                    path.ArcTo(275, 120, 325, 60, 30);
                    path.ArcTo(325, 60, size.Width, 60, 30);
                    path.LineTo(size.Width, 60);
                    path.LineTo(size.Width, 0);
                    path.Close();

                    canvas.DrawPath(path, paintGrey);
                });

                layers.Layer().Canvas((canvas, size) =>
                {
                    using var paintRed = new SKPaint
                    {
                        Color = SKColor.Parse(Colors.Red.Accent3),
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(size.Width, 0);
                    path.LineTo(size.Width, 30);
                    path.ArcTo(size.Width - 60, 30, size.Width - 60, 20, 10);
                    path.LineTo(size.Width - 60, 0);
                    path.Close();

                    canvas.DrawPath(path, paintRed);
                });

                layers.Layer().Row(row =>
                {
                    row.ConstantItem(65).PaddingTop(10).PaddingLeft(10).Image(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "BE_Logo.png"));
                    row.AutoItem().PaddingTop(10).PaddingLeft(10).Column(Column =>
                    {
                        Column
                            .Item().PaddingBottom(-5).Text("BERCHTENBREITER GmbH")
                            .FontSize(17).SemiBold().Italic().FontColor(Colors.Red.Darken2);
                        Column
                            .Item().Text("MASCHINENBAU - AUFZUGTECHNIK")
                            .FontSize(12).SemiBold().Italic().FontColor(Colors.Grey.Lighten2);
                        Column
                            .Item().Text("Mähderweg 1a  86637 Rieblingen")
                            .FontSize(10).FontColor(Colors.Grey.Lighten1);
                        Column
                            .Item().Text("Telefon 08272 / 9867-0  Telefax 9867-30")
                            .FontSize(10).FontColor(Colors.Grey.Lighten1);
                        Column
                            .Item().Text("E-Mail: info@berchtenbreiter-gmbh.de")
                            .FontSize(10).FontColor(Colors.Grey.Lighten1);
                        Column
                            .Item().PaddingTop(3).PaddingLeft(-50).Text(Title)
                            .Bold().FontSize(18).FontColor(Colors.White);
                    });
                    row.RelativeItem().PaddingLeft(5, Unit.Millimetre).Column(Column =>
                    {
                        Column
                            .Item().ContentFromRightToLeft().Width(60).AlignCenter().Text(Index).Bold().FontSize(20).FontColor(Colors.White);

                        Column
                            .Item().ContentFromRightToLeft().PaddingTop(7).PaddingRight(5).Text(GetMetadata().ModifiedDate.ToString("dddd, dd MMMM yyyy")).FontSize(12).FontColor(Colors.White);

                        Column
                            .Item().PaddingTop(9).Row(row =>
                            {
                                row.AutoItem().PaddingLeft(50).Text(AuftragsNummer).FontSize(16).FontColor(Colors.White);
                                row.AutoItem().PaddingHorizontal(10).LineVertical(1).LineColor(Colors.White);
                                row.RelativeItem().Text(FabrikNummer).FontSize(16).FontColor(Colors.White);
                            });
                        Column
                           .Item().PaddingTop(9).PaddingLeft(20).Text(Kennwort).FontSize(16).FontColor(Colors.Black);
                    });
                });
            });

            column.Item().SkipOnce().Layers(layers =>
            {
                layers.Layer().Canvas((canvas, size) =>
                {
                    using var paintRed = new SKPaint
                    {
                        Color = SKColor.Parse(Colors.Red.Accent3),
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(250, 0);
                    path.LineTo(250, 30);
                    path.ArcTo(size.Width, 30, size.Width, 0, 10);
                    path.LineTo(size.Width, 0);
                    path.Close();

                    canvas.DrawPath(path, paintRed);
                });

                layers.PrimaryLayer().Height(60).Width(595).Canvas((canvas, size) =>
                {
                    using var paintGrey = new SKPaint
                    {
                        Color = SKColor.Parse(Colors.Grey.Darken3),
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(0, 0);
                    path.LineTo(0, 60);
                    path.ArcTo(275, 60, 325, 0, 30);
                    path.ArcTo(325, 0, size.Width, 0, 30);
                    path.Close();
                    canvas.DrawPath(path, paintGrey);
                });

                layers.Layer().Row(row =>
                {
                    row.
                        ConstantItem(290).PaddingTop(32).PaddingLeft(15).Text(Title)
                        .Bold().FontSize(18).FontColor(Colors.White);
                    row.RelativeItem().Column(column =>
                    {
                        column
                            .Item().PaddingLeft(42).PaddingTop(5).Row(row =>
                            {
                                row.AutoItem().Text(AuftragsNummer).FontSize(16).FontColor(Colors.White);
                                row.AutoItem().PaddingHorizontal(10).LineVertical(1).LineColor(Colors.White);
                                row.RelativeItem().Text(FabrikNummer).FontSize(16).FontColor(Colors.White);
                            });

                        column
                           .Item().PaddingTop(3).PaddingLeft(12).Text(Kennwort).FontSize(16).FontColor(Colors.Black);
                    });
                });
            });
        });
    }

    void Content(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingLeft(10, Unit.Millimetre).Background(Colors.Grey.Lighten2);
        });
    }

    void Footer(IContainer container)
    {
        container.Layers(layers =>
        {
            layers.PrimaryLayer().Height(40).Width(595).Canvas((canvas, size) =>
            {
                using var paintRed = new SKPaint
                {
                    Color = SKColor.Parse(Colors.Red.Accent3),
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
                canvas.DrawPath(path, paintRed);
            });

            layers.Layer().Row(row =>
            {
                row.AutoItem().PaddingTop(20).PaddingLeft(5, Unit.Millimetre).Text(text =>
                {
                    text.Span("www.berchtenbreiter-gmbh.de").FontColor(Colors.White);
                });
                row.RelativeItem().AlignRight().PaddingTop(5).PaddingRight(4, Unit.Millimetre).Text(text =>
                {
                    text.CurrentPageNumber().Bold().FontSize(20).FontColor(Colors.White);
                    text.Span(" / ").Bold().FontSize(20).FontColor(Colors.White);
                    text.TotalPages().Bold().FontSize(20).FontColor(Colors.White);
                });
            });
        });
    }
}
