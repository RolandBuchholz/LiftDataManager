using Cogs.Collections;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace LiftDataManager.Core.Models.PdfDocuments;

public class PdfBaseDocument : IDocument
{
    public ObservableDictionary<string, Parameter> ParameterDictionary { get; set; } = new();

    public string Title { get; set; } = string.Empty;
    public string Index => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Index");
    public string AuftragsNummer => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_AuftragsNummer");
    public string FabrikNummer => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_FabrikNummer");
    public string Kennwort => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Kennwort");
    private readonly string user;

    public PdfBaseDocument()
    {
        user = GetUserName();
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

    private string GetUserName()
    {
        string? user;
        try
        {
        #pragma warning disable CA1416 
            user = string.IsNullOrWhiteSpace(System.Security.Principal.WindowsIdentity.GetCurrent().Name) 
                ? "no user detected" 
                : System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("PPS\\", "");
        #pragma warning restore CA1416 
        }
        catch 
        {
            user = "Useridentification failed";
        }
        return user;
    }

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
                    row.AutoItem().PaddingTop(10).PaddingLeft(10).Column(column =>
                    {
                        column
                            .Item().PaddingBottom(-5).Text("BERCHTENBREITER GmbH")
                            .FontSize(17).SemiBold().Italic().FontColor(Colors.Red.Darken2);
                        column
                            .Item().Text("MASCHINENBAU - AUFZUGTECHNIK")
                            .FontSize(12).SemiBold().Italic().FontColor(Colors.Grey.Lighten2);
                        column
                            .Item().Text("Mähderweg 1a  86637 Rieblingen")
                            .FontSize(10).FontColor(Colors.Grey.Lighten1);
                        column
                            .Item().Text("Telefon 08272 / 9867-0  Telefax 9867-30")
                            .FontSize(10).FontColor(Colors.Grey.Lighten1);
                        column
                            .Item().Text("E-Mail: info@berchtenbreiter-gmbh.de")
                            .FontSize(10).FontColor(Colors.Grey.Lighten1);
                        column
                            .Item().PaddingTop(3).PaddingLeft(-50).Text(Title)
                            .Bold().FontSize(18).FontColor(Colors.White);
                    });
                    row.RelativeItem().PaddingLeft(5, Unit.Millimetre).Column(column =>
                    {
                        column
                            .Item().ContentFromRightToLeft().Width(60).AlignCenter().Text(Index).Bold().FontSize(20).FontColor(Colors.White);

                        column
                            .Item().ContentFromRightToLeft().PaddingTop(7).PaddingRight(5).Text(GetMetadata().ModifiedDate.ToString("dddd, dd MMMM yyyy")).FontSize(12).FontColor(Colors.White);

                        column
                            .Item().PaddingTop(7).Row(row =>
                            {
                                row.AutoItem().PaddingLeft(50).Column(column =>
                                {
                                    column.Item().Text("Auftragsnummer:").FontSize(6).FontColor(Colors.White);
                                    column.Item().Text(AuftragsNummer).FontSize(16).FontColor(Colors.White);
                                });
                                row.AutoItem().PaddingVertical(2).PaddingHorizontal(10).LineVertical(1).LineColor(Colors.White);
                                row.AutoItem().PaddingLeft(10).Column(column =>
                                {
                                    column.Item().Text("Fabriknummer:").FontSize(6).FontColor(Colors.White);
                                    column.Item().Text(FabrikNummer).FontSize(16).FontColor(Colors.White);
                                });
                            });
                        column
                            .Item().PaddingLeft(20).PaddingTop(-2).Text("Kennwort:").FontSize(6).FontColor(Colors.Black);
                        column
                           .Item().PaddingLeft(20).Text(Kennwort).FontSize(14).FontColor(Colors.Black);
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
                            .Item().PaddingLeft(42).Row(row =>
                            {
                                row.AutoItem().Column(column =>
                                {
                                    column.Item().Text("Auftragsnummer:").FontSize(6).FontColor(Colors.White);
                                    column.Item().Text(AuftragsNummer).FontSize(16).FontColor(Colors.White);
                                });
                                row.AutoItem().PaddingVertical(2).PaddingHorizontal(10).LineVertical(1).LineColor(Colors.White);
                                row.AutoItem().PaddingLeft(10).Column(column =>
                                {
                                    column.Item().Text("Fabriknummer:").FontSize(6).FontColor(Colors.White);
                                    column.Item().Text(FabrikNummer).FontSize(16).FontColor(Colors.White);
                                });
                            });

                        column
                            .Item().PaddingLeft(12).PaddingTop(-2).Text("Kennwort:").FontSize(6).FontColor(Colors.Black);
                        column
                            .Item().PaddingLeft(12).Text(Kennwort).FontSize(14).FontColor(Colors.Black);
                    });
                });
            });
        });
    }

    protected virtual void Content(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Height(100).Padding(10, Unit.Millimetre).Background(Colors.Grey.Medium).Text("Pdf Base Document");
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
