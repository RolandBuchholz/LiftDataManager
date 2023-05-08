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

    //Colors
    public string primaryColor = Colors.Pink.Accent3;
    public string primaryVariantColor = Colors.Pink.Lighten2;
    public string secondaryColor = Colors.BlueGrey.Darken3;
    public string secondaryVariantColor = Colors.BlueGrey.Lighten3;
    public string background = Colors.White;
    public string onPrimaryColor = Colors.White;
    public string onPrimaryVariantColor = Colors.White;
    public string onSecondaryColor = Colors.White;
    public string onSecondaryVariantColor = Colors.White;
    public string errorColor = Colors.Red.Lighten3;
    public string successfulColor = Colors.Green.Lighten3;
    public string highlightColor = Colors.Lime.Accent3;

    //FontSize
    public float fontSizeXXS = 6;
    public float fontSizeXS = 8;
    public float fontSizeS = 10;
    public float fontSizeStandard = 12;
    public float fontSizeL = 14;
    public float fontSizeXL = 16;
    public float fontSizeXXL = 18;
    public float fontSizeBig = 20;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.MarginTop(0, Unit.Millimetre);
            page.MarginBottom(0, Unit.Millimetre);
            page.MarginLeft(0, Unit.Millimetre);
            page.MarginRight(0, Unit.Millimetre);
            page.PageColor(background);
            page.DefaultTextStyle(x => x.FontSize(fontSizeStandard));
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
                    using var paintPrimaryColor = new SKPaint
                    {
                        Color = SKColor.Parse(primaryColor),
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(250, 0);
                    path.LineTo(250, 90);
                    path.ArcTo(size.Width, 90, size.Width, 0, 10);
                    path.LineTo(size.Width, 0);
                    path.Close();

                    canvas.DrawPath(path, paintPrimaryColor);
                });

                layers.PrimaryLayer().Height(120).Width(595).Canvas((canvas, size) =>
                {
                    using var paintSecondaryColor = new SKPaint
                    {
                        Color = SKColor.Parse(secondaryColor),
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

                    canvas.DrawPath(path, paintSecondaryColor);
                });

                layers.Layer().Canvas((canvas, size) =>
                {
                    using var paintPrimaryColor = new SKPaint
                    {
                        Color = SKColor.Parse(primaryColor),
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(size.Width, 0);
                    path.LineTo(size.Width, 30);
                    path.ArcTo(size.Width - 60, 30, size.Width - 60, 20, 10);
                    path.LineTo(size.Width - 60, 0);
                    path.Close();

                    canvas.DrawPath(path, paintPrimaryColor);
                });

                layers.Layer().Row(row =>
                {
                    row.ConstantItem(65).PaddingTop(10).PaddingLeft(10).Image(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "BE_Logo.png"));
                    row.AutoItem().PaddingTop(8).PaddingLeft(10).Column(column =>
                    {
                        column
                            .Item().PaddingBottom(-5).Text("BERCHTENBREITER GmbH")
                            .FontSize(fontSizeXXL).SemiBold().Italic().FontColor(primaryColor);
                        column
                            .Item().Text("MASCHINENBAU - AUFZUGTECHNIK")
                            .FontSize(fontSizeStandard).SemiBold().Italic().FontColor(Colors.Grey.Lighten2);
                        column
                            .Item().Text("Mähderweg 1a  86637 Rieblingen")
                            .FontSize(fontSizeS).FontColor(Colors.Grey.Lighten1);
                        column
                            .Item().Text("Telefon 08272 / 9867-0  Telefax 9867-30")
                            .FontSize(fontSizeS).FontColor(Colors.Grey.Lighten1);
                        column
                            .Item().Text("E-Mail: info@berchtenbreiter-gmbh.de")
                            .FontSize(fontSizeS).FontColor(Colors.Grey.Lighten1);
                        column
                            .Item().PaddingTop(3).PaddingLeft(-50).Text(Title)
                            .Bold().FontSize(fontSizeXXL).FontColor(onSecondaryColor);
                    });
                    row.RelativeItem().PaddingLeft(5, Unit.Millimetre).Column(column =>
                    {
                        column
                            .Item().ContentFromRightToLeft().Width(60).AlignCenter().Text(Index).Bold().FontSize(fontSizeBig).FontColor(onPrimaryColor);

                        column
                            .Item().ContentFromRightToLeft().PaddingTop(7).PaddingRight(5).Text(GetMetadata().ModifiedDate.ToString("dddd, dd MMMM yyyy")).FontSize(fontSizeStandard).FontColor(secondaryColor);

                        column
                            .Item().PaddingTop(7).Row(row =>
                            {
                                row.AutoItem().PaddingLeft(50).Column(column =>
                                {
                                    column.Item().Text("Auftragsnummer:").FontSize(fontSizeXXS).FontColor(onPrimaryColor);
                                    column.Item().Text(AuftragsNummer).FontSize(fontSizeXL).FontColor(onPrimaryColor);
                                });
                                row.AutoItem().PaddingVertical(2).PaddingHorizontal(10).LineVertical(1).LineColor(onPrimaryColor);
                                row.AutoItem().PaddingLeft(10).Column(column =>
                                {
                                    column.Item().Text("Fabriknummer:").FontSize(fontSizeXXS).FontColor(onPrimaryColor);
                                    column.Item().Text(FabrikNummer).FontSize(fontSizeXL).FontColor(onPrimaryColor);
                                });
                            });
                        column
                            .Item().PaddingLeft(20).PaddingTop(-2).Text("Kennwort:").FontSize(fontSizeXXS);
                        column
                           .Item().PaddingLeft(20).Text(Kennwort).FontSize(fontSizeL);
                    });
                });
            });

            column.Item().SkipOnce().Layers(layers =>
            {
                layers.Layer().Canvas((canvas, size) =>
                {
                    using var paintPrimaryColor = new SKPaint
                    {
                        Color = SKColor.Parse(primaryColor),
                        IsAntialias = true
                    };

                    using var path = new SKPath();
                    path.MoveTo(250, 0);
                    path.LineTo(250, 30);
                    path.ArcTo(size.Width, 30, size.Width, 0, 10);
                    path.LineTo(size.Width, 0);
                    path.Close();

                    canvas.DrawPath(path, paintPrimaryColor);
                });

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
                    path.ArcTo(275, 60, 325, 0, 30);
                    path.ArcTo(325, 0, size.Width, 0, 30);
                    path.Close();
                    canvas.DrawPath(path, paintSecondaryColor);
                });

                layers.Layer().Row(row =>
                {
                    row.
                        ConstantItem(290).PaddingTop(32).PaddingLeft(15).Text(Title)
                        .Bold().FontSize(18).FontColor(onSecondaryColor);
                    row.RelativeItem().Column(column =>
                    {
                        column
                            .Item().PaddingLeft(42).Row(row =>
                            {
                                row.AutoItem().Column(column =>
                                {
                                    column.Item().Text("Auftragsnummer:").FontSize(fontSizeXXS).FontColor(onPrimaryColor);
                                    column.Item().Text(AuftragsNummer).FontSize(fontSizeXL).FontColor(onPrimaryColor);
                                });
                                row.AutoItem().PaddingVertical(2).PaddingHorizontal(10).LineVertical(1).LineColor(onPrimaryColor);
                                row.AutoItem().PaddingLeft(10).Column(column =>
                                {
                                    column.Item().Text("Fabriknummer:").FontSize(fontSizeXXS).FontColor(onPrimaryColor);
                                    column.Item().Text(FabrikNummer).FontSize(fontSizeXL).FontColor(onPrimaryColor);
                                });
                            });

                        column
                            .Item().PaddingLeft(12).PaddingTop(-2).Text("Kennwort:").FontSize(fontSizeXXS);
                        column
                            .Item().PaddingLeft(12).Text(Kennwort).FontSize(fontSizeL);
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
                using var paintprimaryColor = new SKPaint
                {
                    Color = SKColor.Parse(primaryColor),
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
                canvas.DrawPath(path, paintprimaryColor);
            });

            layers.Layer().Row(row =>
            {
                row.AutoItem().PaddingTop(20).PaddingLeft(5, Unit.Millimetre).Text(text =>
                {
                    text.Span("www.berchtenbreiter-gmbh.de").FontColor(onPrimaryColor);
                });
                row.RelativeItem().AlignRight().PaddingTop(5).PaddingRight(4, Unit.Millimetre).Text(text =>
                {
                    text.CurrentPageNumber().Bold().FontSize(fontSizeBig).FontColor(onPrimaryColor);
                    text.Span(" / ").Bold().FontSize(fontSizeBig).FontColor(onPrimaryColor);
                    text.TotalPages().Bold().FontSize(fontSizeBig).FontColor(onPrimaryColor);
                });
            });
        });
    }
}
