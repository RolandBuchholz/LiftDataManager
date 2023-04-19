using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.CalculationResultsModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Colors = QuestPDF.Helpers.Colors;

namespace PDFTests.Services.DocumentGeneration;

public class KabinenLüftungDocument : IDocument
{
    private readonly ICalculationsModule _calculationsModuleService;
    public ObservableDictionary<string, Parameter> ParameterDictionary { get; set; }
    public CarVentilationResult CarVentilationResult = new();

    public static string Title => "Be- und Entlüftung";
    public string Index => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Index");
    public string AuftragsNummer => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_AuftragsNummer");
    public string FabrikNummer => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_FabrikNummer");
    public string Kennwort => LiftParameterHelper.GetLiftParameterValue<string>(ParameterDictionary, "var_Kennwort");
    private readonly string user;

    public KabinenLüftungDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService)
    {
        _calculationsModuleService = calculationsModuleService;
        ParameterDictionary = parameterDictionary;
        CarVentilationResult = _calculationsModuleService.GetCarVentilationCalculation(parameterDictionary);
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

    void Content(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingLeft(10, Unit.Millimetre).Element(TableCarData);
            column.Item().PaddingLeft(10, Unit.Millimetre).Element(TableVentilationCeiling);
            column.Item().PaddingLeft(10, Unit.Millimetre).Element(TableVentilationCarDoor);
            column.Item().PageBreak();
            column.Item().PaddingTop(10, Unit.Millimetre).PaddingLeft(10, Unit.Millimetre).Element(TableVentilationSkirtingBoard);
        });
    }

    void TableCarData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.RelativeColumn();
            });

            table.Cell().Row(1).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Daten der Aufzugkabine C100 (aufg. Sockel)").FontSize(16).Bold();
            table.Cell().Row(2).Column(1).PaddingLeft(15).Text("Türbreite (TB)");
            table.Cell().Row(2).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_TB"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(3).Column(1).PaddingLeft(15).Text("Türhöhe (TH)");
            table.Cell().Row(3).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_TH"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(4).Column(1).PaddingLeft(15).Text("Fahrkorbbreite (KB)");
            table.Cell().Row(4).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_KBI"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(5).Column(1).PaddingLeft(15).Text("Fahrkorbhöhe (KH)");
            table.Cell().Row(5).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_KHLicht"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(6).Column(1).Background(Colors.Grey.Lighten2).PaddingLeft(15).Text("Kabinengrundfläche (A)");
            table.Cell().Row(6).Column(2).Background(Colors.Grey.Lighten2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_A_Kabine"].Value);
                text.Span(" m²");
            });
            table.Cell().Row(7).Column(1).Background(Colors.Grey.Lighten2).PaddingLeft(15).Text("Kabinengrundfläche (1%)");
            table.Cell().Row(7).Column(2).Background(Colors.Grey.Lighten2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AKabine1Pozent.ToString());
                text.Span(" mm²");
            });
        });
    }

    void TableVentilationCeiling(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.RelativeColumn();
            });

            table.Cell().Row(1).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Belüftung durch die Decke").FontSize(16).Bold();
            table.Cell().Row(2).Column(1).ColumnSpan(3).PaddingLeft(15).PaddingBottom(5).PaddingRight(55, Unit.Millimetre)
                 .Text("In Fahrkorbtiefe ist links und rechts der Decke ein Luftspalt von 10 mm, hierdurch ist ein Luftaustritt durch die offene Decke ins Freie gewährleistet. " +
                 "Die zusätzliche Belüftung durch die Beleuchtungseinheit wurde hier nicht berücksichtigt.");

            table.Cell().Row(3).Column(1).PaddingLeft(15).Text("Fahrkorbtiefe (KT)");
            table.Cell().Row(3).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_KTI"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(4).Column(1).PaddingLeft(15).Text("Luftspaltöffnung");
            table.Cell().Row(4).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.Luftspaltoeffnung.ToString());
                text.Span(" mm");
            });
            table.Cell().Row(5).Column(1).Background(Colors.Grey.Lighten2).PaddingLeft(15).Text("Belüftung pro Seite");
            table.Cell().Row(5).Column(2).Background(Colors.Grey.Lighten2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.Belueftung1Seite.ToString());
                text.Span(" mm²");
            });
            table.Cell().Row(6).Column(1).Background(Colors.Grey.Lighten2).PaddingLeft(15).Text("Gesamtbelüftung (2 Seiten)");
            table.Cell().Row(6).Column(2).Background(Colors.Grey.Lighten2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.Belueftung2Seiten.ToString());
                text.Span(" m²");
            });
            table.Cell().Row(7).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Prüfung Belüftung über Decke").FontSize(16).Bold();
            table.Cell().Row(8).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Row(row =>
            {
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text("Gesamtbelüftung (2 Seiten)");
                row.RelativeItem().AlignCenter().Text(">");
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text("Kabinengrundfläche (1%)");
            });
            table.Cell().Row(9).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Row(row =>
            {
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text(text =>
                {
                    text.Span(CarVentilationResult.Belueftung2Seiten.ToString());
                    text.Span(" mm²");
                });
                row.RelativeItem().AlignCenter().Text(">");
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text(text =>
                {
                    text.Span(CarVentilationResult.AKabine1Pozent.ToString());
                    text.Span(" mm²");
                });
            });
            table.Cell().Row(10).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Text(text =>
            {
                if (CarVentilationResult.ErgebnisBelueftungDecke)
                { text.Span("  Vorschrift erfüllt !  ").Bold().BackgroundColor(Colors.Green.Lighten3); }
                else
                { text.Span("  Vorschrift nicht erfüllt !  ").Bold().BackgroundColor(Colors.Red.Lighten3); };
            });
        });
    }

    void TableVentilationCarDoor(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(80, Unit.Millimetre);
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });

            table.Cell().Row(1).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Entlüftung durch die Fahrkorbtüre").FontSize(16).Bold();
            table.Cell().Row(2).Column(1).PaddingLeft(15).Text("Anzahl Kabinentüren");
            table.Cell().Row(2).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlKabinentueren.ToString());
                text.Span(" Stk");
            });
            table.Cell().Row(3).Column(1).PaddingLeft(15).Text("Anzahl Türflügel je Kabinentür");
            table.Cell().Row(3).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_AnzahlTuerfluegel"].Value);
                text.Span(" Stk");
            });
            table.Cell().Row(4).Column(1).ColumnSpan(2).PaddingLeft(15).Text("Anrechnung zu 50 % des Türluftspaltes auf 1 % der Grundfläche");

            table.Cell().Row(5).Column(1).PaddingLeft(15).Text("Türbreite (TB)");
            table.Cell().Row(5).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_TB"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(6).Column(1).PaddingLeft(15).Text("Türhöhe (TH)");
            table.Cell().Row(6).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_TH"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(7).Column(1).PaddingLeft(15).Text("Luftspaltöffnungen in TB");
            table.Cell().Row(7).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlLuftspaltoeffnungenTB.ToString());
                text.Span(" x ");
                text.Span(CarVentilationResult.Tuerspalt.ToString());
                text.Span("mm x (TB) = ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenTB.ToString());
                text.Span(" mm");
            });
            table.Cell().Row(8).Column(1).PaddingLeft(15).Text("Luftspaltöffnungen in TH");
            table.Cell().Row(8).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlLuftspaltoeffnungenTH.ToString());
                text.Span(" x ");
                text.Span(CarVentilationResult.Tuerspalt.ToString());
                text.Span("mm x (TH) = ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenTH.ToString());
                text.Span(" mm");
            });

            table.Cell().Row(9).Column(1).ColumnSpan(2).Background(Colors.Grey.Lighten2).PaddingLeft(15).Row(row =>
            {
                row.AutoItem().Text("Entlüftung durch die Türspalten 50 % (F3.1)");
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span(CarVentilationResult.EntlueftungTuerspalten50Pozent.ToString());
                    text.Span(" mm²");
                });
            });
        });
    }

    void TableVentilationSkirtingBoard(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.ConstantColumn(70, Unit.Millimetre);
                columns.RelativeColumn();
            });

            table.Cell().Row(1).Column(1).ColumnSpan(3).PaddingTop(5).PaddingBottom(5).Text("Entlüftung durch die Sockelleisten").FontSize(16).Bold();
            table.Cell().Row(2).Column(1).ColumnSpan(3).PaddingLeft(15).PaddingBottom(5).PaddingRight(55, Unit.Millimetre)
                 .Text("An den Fahrkorbwänden ist links, rechts (und hinten) ein Luftspalt zwischen Sockelleiste und Kabinenboden von 10 mm mit Luftaustritt durch Öffnungen in der Kabinenwand ins Freie.");
            table.Cell().Row(3).Column(1).PaddingLeft(15).Text("Fahrkorbbreite (KB)");
            table.Cell().Row(3).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_KBI"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(4).Column(1).PaddingLeft(15).Text("Fahrkorbtiefe (KT)");
            table.Cell().Row(4).Column(2).AlignRight().Text(text =>
            {
                text.Span(ParameterDictionary["var_KTI"].Value);
                text.Span(" mm");
            });
            table.Cell().Row(5).Column(1).PaddingLeft(15).Text("Luftspaltöffnungen in FB");
            table.Cell().Row(5).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlLuftspaltoeffnungenFB.ToString());
                text.Span(" x ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenFB.ToString());
                text.Span(" mm² = ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenFBGesamt.ToString());
                text.Span(" mm");
            });
            table.Cell().Row(6).Column(1).PaddingLeft(15).Text("Luftspaltöffnungen in FT");
            table.Cell().Row(6).Column(2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.AnzahlLuftspaltoeffnungenFT.ToString());
                text.Span(" x ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenFT.ToString());
                text.Span(" mm² = ");
                text.Span(CarVentilationResult.FlaecheLuftspaltoeffnungenFTGesamt.ToString());
                text.Span(" mm");
            });
            table.Cell().Row(7).Column(1).ColumnSpan(2).Background(Colors.Grey.Lighten2).PaddingLeft(15).Row(row =>
            {
                row.AutoItem().Text("Entlüftung durch die Spalten an den Sockelleisten (F3.2)");
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span(CarVentilationResult.FlaecheEntLueftungSockelleisten.ToString());
                    text.Span(" mm²");
                });
            });
            table.Cell().Row(8).Column(1).Background(Colors.Grey.Lighten2).PaddingLeft(15).Text("Entlüftung gesamt (F3.1 + F3.2)");
            table.Cell().Row(8).Column(2).Background(Colors.Grey.Lighten2).AlignRight().Text(text =>
            {
                text.Span(CarVentilationResult.FlaecheEntLueftungGesamt.ToString());
                text.Span(" mm²");
            });

            table.Cell().Row(9).Column(1).ColumnSpan(2).PaddingLeft(15).PaddingTop(15).AlignCenter().Row(row =>
            {
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text("Entlüftung gesamt (F3.1 + F3.2)");
                row.RelativeItem().AlignCenter().Text(">");
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text("Kabinengrundfläche (1%)");
            });
            table.Cell().Row(10).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Row(row =>
            {
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text(text =>
                {
                    text.Span(CarVentilationResult.FlaecheEntLueftungGesamt.ToString());
                    text.Span(" mm²");
                });
                row.RelativeItem().AlignCenter().Text(">");
                row.ConstantItem(60, Unit.Millimetre).AlignCenter().Text(text =>
                {
                    text.Span(CarVentilationResult.AKabine1Pozent.ToString());
                    text.Span(" mm²");
                });
            });
            table.Cell().Row(11).Column(1).ColumnSpan(2).PaddingLeft(15).AlignCenter().Text(text =>
            {
                if (CarVentilationResult.ErgebnisEntlueftung)
                { text.Span("  Vorschrift erfüllt !  ").Bold().BackgroundColor(Colors.Green.Lighten3); }
                else
                { text.Span("  Vorschrift nicht erfüllt !  ").Bold().BackgroundColor(Colors.Red.Lighten3); };
            });
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
