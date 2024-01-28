using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.PdfDocuments;
using LiftDataManager.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;
using System.Text.Json;

namespace PDFTests.Services.DocumentGeneration;

public class EinreichunterlagenDocument : PdfBaseDocument
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly bool LowPrintColor;
    public TechnicalLiftDocumentation LiftDocumentation { get; set; }

    public EinreichunterlagenDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService, bool lowPrintColor)
    {
        _calculationsModuleService = calculationsModuleService;
        ParameterDictionary = parameterDictionary;
        Title = "Technische Unterlagen";
        LowPrintColor = lowPrintColor;
        LiftDocumentation ??= new();
        SetPdfStyle(LowPrintColor, false);
        SetLiftDocumentation();
    }

    private void SetLiftDocumentation()
    {
        var liftDocumentation = ParameterDictionary?["var_Einreichunterlagen"].Value;
        if (!string.IsNullOrWhiteSpace(liftDocumentation))
        {
            var liftdoku = JsonSerializer.Deserialize<TechnicalLiftDocumentation>(liftDocumentation);
            if (liftdoku is not null)
            {
                LiftDocumentation = liftdoku;
            }
        }
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
                                column.Item().Text(FabrikNummer).FontSize(fontSizeXL).FontColor(onPrimaryColor);
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
                        column.Item().Text(FabrikNummer).FontSize(fontSizeXL).FontColor(onPrimaryColor);
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
        container.PaddingLeft(25, Unit.Millimetre).Column(column =>
        {
            column.Item().PaddingTop(15, Unit.Millimetre).Text("Beschreibung der Aufzugsanlage").FontSize(fontSizeXXL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(20).Element(TableBaseLiftData);
            column.Item().PageBreak();
            column.Item().PaddingTop(10, Unit.Millimetre).Text("1. Allgemeine Angaben").FontSize(fontSizeL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(GenerallyLiftData);
            column.Item().PageBreak();
            column.Item().PaddingTop(10, Unit.Millimetre).Text("2.Fahrschacht - Fahrschachtzugänge").FontSize(fontSizeL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(LiftShaftData);
            column.Item().PaddingTop(10, Unit.Millimetre).Text("3. Tragmittel").FontSize(fontSizeL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(LiftRopeData);
            column.Item().PageBreak();
            column.Item().PaddingTop(10, Unit.Millimetre).Text("4. Antrieb").FontSize(fontSizeL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(LiftDriveData);
            column.Item().PaddingTop(10, Unit.Millimetre).Text("5.Fahrkorb - Gegengewicht").FontSize(fontSizeL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(LiftCarCWTData);
            column.Item().PaddingTop(10, Unit.Millimetre).Text("6. Elektrische Ausrüstung").FontSize(fontSizeL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(ElectricalEquipment);
            column.Item().PageBreak();
            column.Item().PaddingTop(10, Unit.Millimetre).Text("7. Liste der Sicherheitsbauteile").FontSize(fontSizeL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(SafetyComponents);
            column.Item().PaddingTop(5, Unit.Millimetre).Text("Elektronische Sicherheitsschaltungen").FontSize(fontSizeStandard).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(ElectricalSafetyComponents);
            column.Item().PaddingTop(10, Unit.Millimetre).Text("8. Besonderheiten der Aufzugsanlage").FontSize(fontSizeL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(SpecialFeatures);
            column.Item().PaddingTop(10, Unit.Millimetre).Text("Anlagen:").FontSize(fontSizeL).FontColor(primaryVariantColor).Bold();
            column.Item().PaddingTop(15).Element(Attachments);
            column.Item().PaddingTop(10, Unit.Millimetre).Text("Der Montagebetrieb").FontSize(fontSizeStandard);
            column.Item().PaddingTop(20, Unit.Millimetre).Text($"Rieblingen, den {DateTime.Now.ToShortDateString()}").FontSize(fontSizeStandard);

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
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("Betriebsort:").Bold();
            table.Cell().Row(1).Column(2).PaddingVertical(10).Text(ParameterDictionary["var_Projekt"].Value);
            table.Cell().Row(2).Column(1).PaddingVertical(10).Text("Fabrik-Nr.:").Bold();
            table.Cell().Row(2).Column(2).PaddingVertical(10).Text(ParameterDictionary["var_FabrikNummer"].Value);
            table.Cell().Row(3).Column(1).PaddingVertical(10).Text("Art des Aufzuges:").Bold();
            table.Cell().Row(3).Column(2).PaddingVertical(10).Text(_calculationsModuleService.GetLiftTyp(ParameterDictionary["var_Aufzugstyp"].Value));
            table.Cell().Row(4).Column(1).PaddingVertical(10).Text("Aufzugtyp:").Bold();
            table.Cell().Row(4).Column(2).PaddingVertical(10).Text(ParameterDictionary["var_Bausatz"].Value);
            table.Cell().Row(5).Column(1).PaddingVertical(10).Text("Errichtungsvorschrift:").Bold();
            table.Cell().Row(5).Column(2).PaddingVertical(10).Row(row =>
            {
                row.AutoItem().Text(ParameterDictionary["var_Normen"].Value);
                row.AutoItem().PaddingLeft(10).CheckBoxvalue(ParameterDictionary["var_EN8121"]);
                row.RelativeItem().PaddingLeft(10).CheckBoxvalue(ParameterDictionary["var_EN8172"]);
            });
            table.Cell().Row(6).Column(1).PaddingVertical(10).Text("Richtlinie:").Bold();
            table.Cell().Row(6).Column(2).PaddingVertical(10).Text("Aufzugsrichtlinie 2014/33/EU");
            table.Cell().Row(7).Column(1).PaddingVertical(10).Text("Name des Betreibers:").Bold();
            table.Cell().Row(7).Column(2).PaddingVertical(10).Text(ParameterDictionary["var_Betreiber"].Value);
            table.Cell().Row(8).Column(1).PaddingVertical(10).Text("Einzelprüfung:").Bold();
            table.Cell().Row(8).Column(2).PaddingVertical(10).Text(ParameterDictionary["var_Aufzugstyp"].Value != "Umbau" ? "gemäß Anhang VIII (Modul G)" : "Prüfung nach § 15 BetrSichV");
        });
    }

    void GenerallyLiftData(IContainer container)
    {
        var manufacturer = """
                           Berchtenbreiter GmbH
                           Maschinenbau - Aufzugtechnik
                           Mähderweg 1a
                           86637 Rieblingen
                           """;
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("Hersteller und/oder Einführer:").Bold();
            table.Cell().Row(1).Column(2).PaddingVertical(10).PaddingLeft(5).Text(manufacturer);
            table.Cell().Row(2).Column(1).PaddingVertical(10).Text("Tragfähigkeit:").Bold();
            table.Cell().Row(2).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{ParameterDictionary["var_Q"].Value} kg oder {ParameterDictionary["var_Personen"].Value} Personen" );
            table.Cell().Row(3).Column(1).PaddingVertical(10).Text("Baujahr:").Bold();
            table.Cell().Row(3).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{LiftDocumentation?.ManufactureYear}");
            table.Cell().Row(4).Column(1).PaddingVertical(10).Text("Errichtungsdatum:").Bold();
            table.Cell().Row(4).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{LiftDocumentation?.MonthOfConstruction} - {LiftDocumentation?.YearOfConstruction}");
            table.Cell().Row(5).Column(1).PaddingVertical(10).Text("Betriebsgeschwindigkeit:").Bold();
            table.Cell().Row(5).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{ParameterDictionary["var_v"].Value} m/s");
            table.Cell().Row(6).Column(1).PaddingVertical(10).Text("Förderhöhe:").Bold();
            table.Cell().Row(6).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{ParameterDictionary["var_FH"].Value} m");
            table.Cell().Row(7).Column(1).PaddingVertical(10).Text("Anzahl der Schachttüren:").Bold();
            table.Cell().Row(7).Column(2).PaddingVertical(10).PaddingLeft(5).Text(ParameterDictionary["var_Zugangsstellen"].Value);
            table.Cell().Row(8).Column(1).PaddingVertical(10).Text("Anzahl der Haltestellen:").Bold();
            table.Cell().Row(8).Column(2).PaddingVertical(10).PaddingLeft(5).Text(ParameterDictionary["var_Haltestellen"].Value);
            table.Cell().Row(9).Column(1).ColumnSpan(2).PaddingVertical(10).Text(string.IsNullOrWhiteSpace(ParameterDictionary?["var_GemeinsamerSchachtMit"].Value) ?
                                                    "Vorstehender Aufzug ist mit keinem weiteren Aufzug im gleichen Schacht errichtet." :
                                                    $"Vorstehender Aufzug ist mit dem Aufzug - den Aufzügen - Fabrik.-Nr.: {ParameterDictionary["var_GemeinsamerSchachtMit"].Value} \n" +
                                                    $"im gleichem Schacht errichtet.").Bold();
        });
    }

    void LiftShaftData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("Ausführung der Fahrschachtwände:").Bold();
            table.Cell().Row(1).Column(2).PaddingVertical(10).PaddingLeft(5).Text(ParameterDictionary["var_Schacht"].Value);
            table.Cell().Row(2).Column(1).PaddingVertical(10).Text("untere Schutzraumhöhe:").Bold();
            table.Cell().Row(2).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{LiftDocumentation.SafetySpacePit} mm\n" +
                                                                   $"Art des Schutzraums: {LiftDocumentation.ProtectedSpaceTypPit}");
            table.Cell().Row(3).Column(1).PaddingVertical(10).Text("obere Schutzraumhöhe:").Bold();
            table.Cell().Row(3).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{LiftDocumentation.SafetySpaceHead} mm\n" +
                                                                   $"Art des Schutzraums: {LiftDocumentation.ProtectedSpaceTypHead}");
            table.Cell().Row(4).Column(1).PaddingVertical(10).Text("Art der maschinell- handbetätigten Fahrschachttüren:").Bold();
            table.Cell().Row(4).Column(2).PaddingVertical(10).PaddingLeft(5).Column(column => 
            {
                column.Item().Text($"{ParameterDictionary["var_Zugangsstellen"].Value} Stück");
                column.Item().Text($"Fabrikat: {ParameterDictionary["var_Tuertyp"].Value?.Replace(" -","")}");
                column.Item().Text($"Türbreite: {ParameterDictionary["var_TB"].Value} mm");
                column.Item().Text($"Türhöhe: {ParameterDictionary["var_TH"].Value} mm");
            });
            table.Cell().Row(5).Column(1).ColumnSpan(2).PaddingVertical(10).Text(LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_TuerSchauOeffnungKT") || LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_TuerSchauOeffnungST") ?
                                                                                 "Schauöffnungen (aus 10 mm dickem VSG - Glas) in den Fahr/Schachttüren vorhanden." :
                                                                                 "Schauöffnungen in den Fahr/Schachttüren - nicht vorhanden.").Bold();
            table.Cell().Row(1).RowSpan(4).Column(3).Column(column =>
            {
                column.Item().PaddingVertical(5).ProtectedSpaceTypInfoBox("Schachtgrube", LiftDocumentation.ProtectedSpaceTypPit);
                column.Item().PaddingVertical(5).ProtectedSpaceTypInfoBox("Schachtkopf",LiftDocumentation.ProtectedSpaceTypHead);
            });
        });
    }

    void LiftRopeData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("Anzahl und Art der Tragmittel:").Bold();
            table.Cell().Row(1).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{ParameterDictionary["var_NumberOfRopes"].Value} Stk  {ParameterDictionary["var_Tragseiltyp"].Value}");
            table.Cell().Row(2).Column(1).PaddingVertical(10).Text("Aufhängung des Fahrkorbes:").Bold();
            table.Cell().Row(2).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{ParameterDictionary["var_AufhaengungsartRope"].Value}:1, des Gegengewichtes {ParameterDictionary["var_AufhaengungsartRope"].Value}:1");
            table.Cell().Row(3).Column(1).PaddingVertical(10).Text("Anzahl und Art der\n" + "gespannten oder nicht\n" + "gespannten Unterseile:").Bold();
            table.Cell().Row(3).Column(2).PaddingVertical(10).PaddingLeft(5).AlignMiddle().Text("keine");
        });
    }

    void LiftDriveData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("Art des Antriebs:").Bold();
            table.Cell().Row(1).Column(2).PaddingVertical(10).PaddingLeft(5).Text(_calculationsModuleService.GetDriveTyp(ParameterDictionary["var_Getriebe"].Value, (int)LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_AufhaengungsartRope")));
            table.Cell().Row(2).Column(1).PaddingVertical(10).Text("Antriebsregelung:").Bold();
            table.Cell().Row(2).Column(2).PaddingVertical(10).PaddingLeft(5).Text(_calculationsModuleService.GetDriveControl(ParameterDictionary["var_Aggregat"].Value));
            table.Cell().Row(3).Column(1).PaddingVertical(10).Text("Aufstellung des Triebwerkes:").Bold();
            table.Cell().Row(3).Column(2).PaddingVertical(10).PaddingLeft(5).AlignMiddle().Text(_calculationsModuleService.GetDrivePosition(ParameterDictionary["var_Maschinenraum"].Value));
        });
    }

    void LiftCarCWTData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("Fahrkorbgrundfläche:").Bold();
            table.Cell().Row(1).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{ParameterDictionary["var_A_Kabine"].Value} m²");
            table.Cell().Row(2).Column(1).PaddingVertical(10).Text("Fahrkorbhöhe:").Bold();
            table.Cell().Row(2).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{ParameterDictionary["var_KHLicht"].Value} mm");
            table.Cell().Row(3).Column(1).PaddingVertical(10).Text("Anzahl der Fahrkorbzugänge mit maschinell betätigter Fahrkorbtür:").Bold();
            table.Cell().Row(3).Column(2).PaddingVertical(10).PaddingLeft(5).AlignMiddle().Text($"{_calculationsModuleService.GetNumberOfCardoors(ParameterDictionary)} Stück");
            table.Cell().Row(4).Column(1).PaddingVertical(10).Text("Gewicht des Fahrkorbes:").Bold();
            table.Cell().Row(4).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{ParameterDictionary["var_F"].Value} kg");
            table.Cell().Row(5).Column(1).PaddingVertical(10).Text("Gewicht des Gegengewichtes:").Bold();
            table.Cell().Row(5).Column(2).PaddingVertical(10).PaddingLeft(5).Text($"{ParameterDictionary["var_Gegengewichtsmasse"].Value} kg");
            table.Cell().Row(6).Column(1).PaddingVertical(10).Text("Gewicht des Seilgewichtsausgleiches (Unterseil bzw. Seilausgleichskette):").Bold();
            table.Cell().Row(6).Column(2).PaddingVertical(10).PaddingLeft(5).AlignMiddle().Text("---");
        });
    }

    void ElectricalEquipment(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("Spannung und Netzform des Anschlussnetzes:").Bold();
            table.Cell().Row(1).Column(2).PaddingVertical(10).PaddingLeft(5).AlignMiddle().Text(ParameterDictionary["var_Stromanschluss"].Value);
            table.Cell().Row(2).Column(1).PaddingVertical(10).Text("Nennstrom des Antriebsmotors:").Bold();
            table.Cell().Row(2).Column(2).PaddingVertical(10).PaddingLeft(5).AlignMiddle().Text($"{ParameterDictionary["var_ZA_IMP_Motor_Ir"].Value} A");
        });
    }

    void SafetyComponents(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("Fangvorrichtung.........").Bold();
        });
    }

    void ElectricalSafetyComponents(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("--------").Bold();
        });
    }

    void SpecialFeatures(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text(LiftDocumentation.SpecialFeatures).Bold();
        });
    }

    void Attachments(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(60, Unit.Millimetre);
                columns.RelativeColumn();
            });
            table.Cell().Row(1).Column(1).PaddingVertical(10).Text("ddd").Bold();
        });
    }
}