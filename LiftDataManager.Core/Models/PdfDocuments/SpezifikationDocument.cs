using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.PdfDocuments;
using LiftDataManager.Core.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace PDFTests.Services.DocumentGeneration;

public class SpezifikationDocument : PdfBaseDocument
{
    private readonly ICalculationsModule _calculationsModuleService;

    public SpezifikationDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService)
    {
        ParameterDictionary = parameterDictionary;
        _calculationsModuleService = calculationsModuleService;
        Title = "Spezifikation";
    }

    protected override void Content(IContainer container)
    {
        container.PaddingLeft(10, Unit.Millimetre).PaddingRight(10, Unit.Millimetre).DefaultTextStyle(x => x.FontSize(fontSizeXS)).Column(column =>
        {
            column.Item().AlignRight().Row(row =>
            {
                row.ConstantItem(70).ParameterDateCell(ParameterDictionary["var_ErstelltAm"], false, true, null);
                row.ConstantItem(70).ParameterStringCell(ParameterDictionary["var_ErstelltVon"], null, false, true, null);
            });
            column.Item().Element(AddressData);
            column.Item().Element(GenerallyData);
            column.Item().Element(ShaftData);
            column.Item().PageBreak();
            column.Item().Element(CarData);
            column.Item().PageBreak();
            column.Item().Element(CarFrameData);
        });
    }

    void AddressData(IContainer container)
    {
        float boxHeight = 112;

        container.Row(row =>
        {
            row.RelativeItem(4).Column(column =>
            {
                column.Item().Height(boxHeight / 2).Layers(layers =>
                {
                    layers.PrimaryLayer().Canvas((canvas, size) =>
                    {
                        using var paintSecondaryColor = new SKPaint
                        {
                            Color = SKColor.Parse(secondaryColor),
                            IsAntialias = true,
                            StrokeWidth = 1.5f,
                            IsStroke = true,
                        };
                        canvas.DrawRoundRect(0, 0, size.Width, boxHeight / 2, 4, 4, paintSecondaryColor);
                    });
                    layers.Layer().PaddingLeft(5).Text(text =>
                    {
                        text.Line("Bauvorhaben:").FontSize(fontSizeXXS).FontColor(secondaryColor).Bold();
                        text.Line(ParameterDictionary["var_Projekt"].Value);
                    });
                });
                column.Item().PaddingTop(3).Height(boxHeight / 2).Layers(layers =>
                {
                    layers.PrimaryLayer().Canvas((canvas, size) =>
                    {
                        using var paintSecondaryColor = new SKPaint
                        {
                            Color = SKColor.Parse(secondaryColor),
                            IsAntialias = true,
                            StrokeWidth = 1.5f,
                            IsStroke = true,
                        };
                        canvas.DrawRoundRect(0, 0, size.Width, boxHeight / 2, 4, 4, paintSecondaryColor);
                    });
                    layers.Layer().PaddingLeft(5).Text(text =>
                    {
                        text.Line("Betreiber:").FontSize(fontSizeXXS).FontColor(secondaryColor).Bold();
                        text.Line(ParameterDictionary["var_Betreiber"].Value);
                    });
                });
            });
            row.RelativeItem(6).PaddingLeft(3).Height(boxHeight + 3).Layers(layers =>
            {
                layers.PrimaryLayer().Canvas((canvas, size) =>
                {
                    using var paintSecondaryColor = new SKPaint
                    {
                        Color = SKColor.Parse(secondaryColor),
                        IsAntialias = true,
                        StrokeWidth = 1.5f,
                        IsStroke = true,
                    };
                    canvas.DrawRoundRect(0, 0, size.Width, boxHeight + 3, 4, 4, paintSecondaryColor);
                });

                layers.Layer().PaddingLeft(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(35, Unit.Millimetre);
                        columns.RelativeColumn();
                    });
                    table.Cell().Row(1).Column(1).ColumnSpan(2).Text("Ansprechperson:").FontSize(fontSizeXXS).FontColor(secondaryColor).Bold();
                    table.Cell().Row(2).Column(1).Text("Firma Auftraggeber:");
                    table.Cell().Row(2).Column(2).PaddingRight(2).BorderBottom(0.1f).BorderColor(primaryColor).Text(ParameterDictionary["var_Firma"].Value);
                    table.Cell().Row(3).Column(1).Text("Name Auftraggeber:");
                    table.Cell().Row(3).Column(2).PaddingRight(2).BorderBottom(0.1f).BorderColor(primaryColor).Text(ParameterDictionary["var_AnPersonZ1"].Value);
                    table.Cell().Row(4).Column(1).Text("Straße Hausnummer:");
                    table.Cell().Row(4).Column(2).PaddingRight(2).BorderBottom(0.1f).BorderColor(primaryColor).Text(ParameterDictionary["var_AnPersonZ2"].Value);
                    table.Cell().Row(5).Column(1).Text("Postleitzahl Ort:");
                    table.Cell().Row(5).Column(2).PaddingRight(2).BorderBottom(0.1f).BorderColor(primaryColor).Text(ParameterDictionary["var_AnPersonZ3"].Value);
                    table.Cell().Row(6).Column(1).Text("Fachplaner:");
                    table.Cell().Row(6).Column(2).PaddingRight(2).BorderBottom(0.1f).BorderColor(primaryColor).Text(ParameterDictionary["var_AnPersonZ4"].Value);
                    table.Cell().Row(7).Column(1).Text("Adresse Fachplaner:");
                    table.Cell().Row(7).Column(2).PaddingRight(2).BorderBottom(0.1f).BorderColor(primaryColor).Text(ParameterDictionary["var_FP_Adresse"].Value);
                    table.Cell().Row(8).Column(1).Text("Telefon Fachplaner:");
                    table.Cell().Row(8).Column(2).PaddingRight(2).BorderBottom(0.1f).BorderColor(primaryColor).Text(ParameterDictionary["var_AnPersonPhone"].Value);
                    table.Cell().Row(9).Column(1).Text("Mobiltelefon Fachplaner:");
                    table.Cell().Row(9).Column(2).PaddingRight(2).BorderBottom(0.1f).BorderColor(primaryColor).Text(ParameterDictionary["var_AnPersonMobil"].Value);
                    table.Cell().Row(10).Column(1).Text("E-Mail Fachplaner:");
                    table.Cell().Row(10).Column(2).PaddingRight(2).BorderBottom(0.1f).BorderColor(primaryColor).Text(ParameterDictionary["var_AnPersonMail"].Value);
                });
            });
        });
    }

    void GenerallyData(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintSecondaryColor = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintSecondaryColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintSecondaryColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintSecondaryColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintSecondaryColorSmall);
            });

            layers.PrimaryLayer().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(15);
                    columns.ConstantColumn(85);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                table.Cell().Row(1).Column(1).RowSpan(11).RotateLeft().AlignMiddle().AlignCenter().Text("Allgemeine Daten").FontColor(secondaryColor).Bold();
                table.Cell().Row(1).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Termine").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(1).Column(3).ParameterDateCell(ParameterDictionary["var_FreigabeErfolgtAm"]);
                table.Cell().Row(1).Column(4).ParameterDateCell(ParameterDictionary["var_Demontage"]);
                table.Cell().Row(1).Column(5).ParameterDateCell(ParameterDictionary["var_AuslieferungAm"]);
                table.Cell().Row(1).Column(6).ParameterDateCell(ParameterDictionary["var_FertigstellungAm"], false, true, null);
                table.Cell().Row(2).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Grundinformationen").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(2).Column(3).ParameterStringCell(ParameterDictionary["var_Lieferart"]);
                table.Cell().Row(2).Column(4).ParameterStringCell(ParameterDictionary["var_InformationAufzug"]);
                table.Cell().Row(2).Column(5).ParameterStringCell(ParameterDictionary["var_FabriknummerBestand"]);
                table.Cell().Row(2).Column(6).ParameterStringCell(ParameterDictionary["var_CeNummer"]);
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Gebaeudetyp"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_FE_Gebaeude"], null, false, false, "Gebäudetyp Zusatzinformationen");
                table.Cell().Row(4).RowSpan(6).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Anlagedaten").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(4).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Aufzugstyp"]);
                table.Cell().Row(4).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_FE_Aufzugstyp"], null, false, false, "Aufzugstyp Zusatzinformationen");
                table.Cell().Row(5).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Beladegeraet"]);
                table.Cell().Row(5).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_FE_Beladegeraete"], null, false, false, "Beladegerät Zusatzinformationen");
                table.Cell().Row(6).Column(3).ParameterStringCell(ParameterDictionary["var_Q"], "kg");
                table.Cell().Row(6).Column(4).ParameterStringCell(ParameterDictionary["var_Q1"], "kg");
                table.Cell().Row(6).Column(5).ParameterStringCell(ParameterDictionary["var_F"], "kg");
                table.Cell().Row(6).Column(6).ParameterStringCell(ParameterDictionary["var_Personen"], "Pers.");
                table.Cell().Row(7).Column(3).ParameterStringCell(ParameterDictionary["var_v"], "m/s");
                table.Cell().Row(7).Column(4).ParameterStringCell(ParameterDictionary["var_FH"], "m");
                table.Cell().Row(7).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_FE_FH"], null, false, false, "Förderhöhe Zusatzinformationen");
                table.Cell().Row(8).Column(3).ParameterStringCell(ParameterDictionary["var_Haltestellen"]);
                table.Cell().Row(8).Column(4).ParameterStringCell(ParameterDictionary["var_Zugangsstellen"]);
                table.Cell().Row(8).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_A"], true, "Zugang A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_B"], true, "Zugang B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_C"], true, "Zugang C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_D"], true, "Zugang D");
                });
                table.Cell().Row(9).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Aussenhaltestellen"]);
                table.Cell().Row(9).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_maxFahrtenStunde"]);
                table.Cell().Row(10).RowSpan(2).Column(2).PaddingLeft(5).AlignMiddle().Text("Normen").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(10).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_Normen"], null, true);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_EN8121"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_18040"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_EN8131"], null, true);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_EN8158"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_18090"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_18091"]);
                });
                table.Cell().Row(11).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_TRBS1121"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_EN8170"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_EN8128"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_EN8171Cat012"], null, true);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_EN8172"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_EN8173"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_EN8176"], true, null);
                });
            });
        });
    }

    void ShaftData(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintSecondaryColor = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintSecondaryColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintSecondaryColor);
                canvas.DrawLine(15, 69, 15, size.Height, paintSecondaryColorSmall);
                canvas.DrawLine(100, 69, 100, size.Height, paintSecondaryColorSmall);
            });

            layers.PrimaryLayer().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(15);
                    columns.ConstantColumn(85);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                table.Cell().Row(1).Column(1).RowSpan(7).RotateLeft().AlignMiddle().AlignCenter().Text("Schacht").FontColor(secondaryColor).Bold();
                table.Cell().Row(1).Column(1).ColumnSpan(6).Element(EntranceData);
                table.Cell().Row(2).RowSpan(2).Column(2).BorderHorizontal(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Schacht").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(2).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_SchachtInformation"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_Schacht"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_Befestigung"]);
                });
                table.Cell().Row(3).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_Maschinenraum"]);
                table.Cell().Row(4).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Schachtabmessungen\n(innen im Lichten)").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(4).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_SB"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_ST"], "mm");
                });

                table.Cell().Row(5).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_SG"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_SK"], "mm");
                });
                table.Cell().Row(6).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Schachtgerüst").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();

                table.Cell().Row(6).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_GeruestFarbe"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_GeruestFeldfuellung"]);
                });
                table.Cell().Row(7).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Brandschutz Schacht").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();

                table.Cell().Row(7).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(3).ParameterStringCell(ParameterDictionary["var_BsMit"]);
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_BsLiefer"]);
                    row.RelativeItem(2.5f).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_BsDin"], true, "Brandschutz nach DIN 4102 F90");
                });
            });
        });
    }

    void CarData(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintSecondaryColor = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintSecondaryColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintSecondaryColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintSecondaryColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintSecondaryColorSmall);
            });

            layers.PrimaryLayer().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(15);
                    columns.ConstantColumn(85);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                table.Cell().Row(1).Column(1).RowSpan(33).RotateLeft().AlignMiddle().AlignCenter().Text("Fahrkorb").FontColor(secondaryColor).Bold();
                table.Cell().Row(1).RowSpan(8).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Grunddaten").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Fahrkorbtyp"]);
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Kabine_ZT"]);
                table.Cell().Row(3).Column(3).ParameterStringCell(ParameterDictionary["var_KBI"],"mm");
                table.Cell().Row(3).Column(4).ParameterStringCell(ParameterDictionary["var_KTI"],"mm");
                table.Cell().Row(4).Column(3).ParameterStringCell(ParameterDictionary["var_KHLicht"], "mm");
                table.Cell().Row(4).Column(4).ParameterStringCell(ParameterDictionary["var_KD"], "mm");
                table.Cell().Row(5).Column(3).ParameterStringCell(ParameterDictionary["var_KU"], "mm");
                table.Cell().Row(5).Column(4).ParameterStringCell(ParameterDictionary["var_KHA"], "mm");
                table.Cell().Row(6).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_L1"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_R1"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_L2"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_R2"], "mm");
                });
                table.Cell().Row(7).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_L3"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_R3"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_L4"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_R4"], "mm");
                });
                table.Cell().Row(7).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_L3"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_R3"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_L4"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_R4"], "mm");
                });
                table.Cell().Row(8).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_TuerEinbau"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_TuerEinbauB"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_TuerEinbauC"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_TuerEinbauD"], "mm");
                });
                table.Cell().Row(1).RowSpan(8).Column(5).ColumnSpan(2).Padding(5).AlignCenter().MaxHeight(170).Component(new CarDesignComponent(ParameterDictionary));
                table.Cell().Row(9).RowSpan(5).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Seitenwände").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(9).Column(3).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_Seitenwaende"]);
                table.Cell().Row(9).Column(6).ParameterStringCell(ParameterDictionary["var_RAL_Seitenwand"]);
                table.Cell().Row(10).Column(3).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_Rueckwand"]);
                table.Cell().Row(10).Column(6).ParameterStringCell(ParameterDictionary["var_RAL_Rueckwand"]);
                table.Cell().Row(11).Column(3).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_Eingangswand"]);
                table.Cell().Row(11).Column(6).ParameterStringCell(ParameterDictionary["var_RAL_Eingangswand"]);
                table.Cell().Row(12).Column(3).ParameterStringCell(ParameterDictionary["var_Materialstaerke"]);
                table.Cell().Row(12).Column(4).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_Schotten_ZT"]);
                table.Cell().Row(13).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Antidroehn"]);
                table.Cell().Row(13).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Antidroehn_ZT"]);
                table.Cell().Row(14).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Decke").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(14).Column(3).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_Decke"]);
                table.Cell().Row(14).Column(6).ParameterStringCell(ParameterDictionary["var_RAL_Decke"]);
                table.Cell().Row(15).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_BelagAufDemKabinendach"]);
                table.Cell().Row(16).RowSpan(5).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Boden").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(16).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Bodentyp"]);
                table.Cell().Row(16).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_KU"],"mm",false,false,"Kabinenbodenhöhe (inkl. Bodenbelag)");
                table.Cell().Row(17).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_Bodenblech"], "mm");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_BoPr"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_SonderExternBodengewicht"], "kg/m²");
                });
                table.Cell().Row(18).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_Bodenprofil_ZT"],null,false,false, "Boden Zusatzinformationen");
                table.Cell().Row(19).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Bodenbelag"]);
                table.Cell().Row(19).Column(5).ParameterStringCell(ParameterDictionary["var_Bodenbelagsgewicht"],"kg/m²");
                table.Cell().Row(19).Column(6).ParameterStringCell(ParameterDictionary["var_Bodenbelagsdicke"],"mm");
                table.Cell().Row(20).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_BodenbelagBeschreibung"]);

                table.Cell().Row(21).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Beleuchtung").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(21).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Beleuchtung"]);
                table.Cell().Row(21).Column(5).ParameterStringCell(ParameterDictionary["var_AnzahlBeleuchtung"],"Stk");
                table.Cell().Row(21).Column(6).ParameterStringCell(ParameterDictionary["var_FarbtemperaturBeleuchtung"],"Kelvin");
                table.Cell().Row(22).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_abgDecke"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_NotlichtKab"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_52"]);
                });
                table.Cell().Row(23).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_Dimmer"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_NotlichtTab"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_1"]);
                });

                table.Cell().Row(24).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Spiegel").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(24).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Spiegel"]);
                table.Cell().Row(24).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SpiegelA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SpiegelB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SpiegelC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SpiegelD"], true, "Seite D");
                });
                table.Cell().Row(25).Column(3).ParameterStringCell(ParameterDictionary["var_BreiteSpiegel"],"mm");
                table.Cell().Row(25).Column(4).ParameterStringCell(ParameterDictionary["var_HoeheSpiegel"],"mm");
                table.Cell().Row(25).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_InfoSpiegel"]);

                table.Cell().Row(26).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_SpiegelAntiKratzf"], true,"Antisplitterfolie");
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_SpiegelPaneel"], true);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_Spiegelleiste"], true);
                });
                table.Cell().Row(26).Column(5).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_2"]);
                table.Cell().Row(26).Column(6).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_3"]);

                table.Cell().Row(27).RowSpan(5).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Ausstattung").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(27).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_Handlauf"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_HoeheHandlauf"],"mm");
                });
                table.Cell().Row(27).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_HandlaufA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_HandlaufB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_HandlaufC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_HandlaufD"], true, "Seite D");
                });
                table.Cell().Row(28).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Sockelleiste"]);
                table.Cell().Row(28).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SockelleisteA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SockelleisteB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SockelleisteC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SockelleisteD"], true, "Seite D");
                });
                table.Cell().Row(29).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_Rammschutz"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_HoeheRammschutz"], "mm");
                });
                table.Cell().Row(29).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_RammschutzA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_RammschutzB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_RammschutzC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_RammschutzD"], true, "Seite D");
                });
                table.Cell().Row(30).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Aussenverkleidung_ZT"]);
                table.Cell().Row(30).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussenverkleidungA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussenverkleidungB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussenverkleidungC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussenverkleidungD"], true, "Seite D");
                });
                table.Cell().Row(31).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Paneelmaterial"]);
                table.Cell().Row(31).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_PaneelPosA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_PaneelPosB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_PaneelPosC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_PaneelPosD"], true, "Seite D");
                });
                table.Cell().Row(32).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Ventilator").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(32).Column(3).ParameterBoolCell(ParameterDictionary["var_VentilatorLuftmenge"],false, "Ventilator 90 m³/h");
                table.Cell().Row(32).Column(4).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_VentilatorAnzahl"], "Stk",true);
                table.Cell().Row(33).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Sonstiges").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(33).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_SonstigesFahrkorb"],null,true,true);
            });
        });
    }

    void CarFrameData(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintSecondaryColor = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintSecondaryColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintSecondaryColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintSecondaryColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintSecondaryColorSmall);
            });

            layers.PrimaryLayer().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(15);
                    columns.ConstantColumn(85);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                table.Cell().Row(1).Column(1).RowSpan(16).RotateLeft().AlignMiddle().AlignCenter().Text("Bausatz").FontColor(secondaryColor).Bold();
                table.Cell().Row(1).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Bausatztyp").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_Bausatz"]);
                    row.RelativeItem(1).Border(0.1f)
                                       .BorderColor(secondaryColor)
                                       .PaddingLeft(5).PaddingTop(0)
                                       .PaddingBottom(-10).Text(text => 
                                       {
                                           text.Line("Rahmengewicht").FontSize(fontSizeXXS).FontColor(secondaryColor).Bold();
                                           text.Line($"{_calculationsModuleService.GetCarFrameWeight(ParameterDictionary)} kg");
                                       });
                });
                table.Cell().Row(1).RowSpan(16).Column(5).ColumnSpan(2).Background(Colors.Purple.Darken1);
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Bausatz_ZT"]);
                table.Cell().Row(3).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Führungsart FK").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(3).Column(3).ParameterStringCell(ParameterDictionary["var_Fuehrungsart"]);
                table.Cell().Row(3).Column(4).ParameterStringCell(ParameterDictionary["var_TypFuehrung"]);
                table.Cell().Row(4).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Führungsart GGW").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(4).Column(3).ParameterStringCell(ParameterDictionary["var_Fuehrungsart_GGW"]);
                table.Cell().Row(4).Column(4).ParameterStringCell(ParameterDictionary["var_TypFuehrung_GGW"]);
                table.Cell().Row(5).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Schienen FK").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(5).Column(3).ParameterStringCell(ParameterDictionary["var_FuehrungsschieneFahrkorb"]);
                table.Cell().Row(5).Column(4).ParameterStringCell(ParameterDictionary["var_StatusFuehrungsschienen"]);
                table.Cell().Row(6).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Schienen GGW").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(6).Column(3).ParameterStringCell(ParameterDictionary["var_FuehrungsschieneGegengewicht"]);
                table.Cell().Row(6).Column(4).ParameterStringCell(ParameterDictionary["var_StatusGGWSchienen"]);
                table.Cell().Row(7).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Fangvorrichtung").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(7).Column(3).ParameterStringCell(ParameterDictionary["var_Fangvorrichtung"]);
                table.Cell().Row(7).Column(4).ParameterStringCell(ParameterDictionary["var_TypFV"]);
                table.Cell().Row(8).Column(3).ColumnSpan(2).Border(0.1f)
                                       .BorderColor(secondaryColor)
                                       .PaddingLeft(5).PaddingTop(0)
                                       .PaddingBottom(-10).Text(text =>
                                       {
                                           text.Line("Fangvorrichtungsbereich").FontSize(fontSizeXXS).FontColor(secondaryColor).Bold();
                                           text.Line($"{_calculationsModuleService.GetCarFrameWeight(ParameterDictionary)} kg");
                                       });
                table.Cell().Row(9).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Geschwindigkeits- begrenzer").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(9).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Geschwindigkeitsbegrenzer"]);
                table.Cell().Row(10).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Geschwindigkeitsbeg_ZT"]);
                table.Cell().Row(11).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Ersatzmaßnahmen").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(11).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Ersatzmassnahmen"]);
                table.Cell().Row(12).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Ersatzmaßnahmen_ZT"]);
                table.Cell().Row(13).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Lastmesseinrichtung").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(13).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Lastmesseinrichtung"]);
                table.Cell().Row(14).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Lastmesseinrichtung_ZT"]);
                table.Cell().Row(15).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Beschichtung").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(15).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Beschichtung"],null,false,false,"Beschichtungsart Fangrahmen");
                table.Cell().Row(16).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_RALTonTragrahmen"], null, false, false, "Beschichtungsart Fangrahmen");
                table.Cell().Row(17).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(secondaryColor).PaddingLeft(5).AlignMiddle().Text("Bausatz sonstiges").FontSize(fontSizeXS).FontColor(secondaryColor).Bold();
                table.Cell().Row(17).Column(3).ColumnSpan(4).MinHeight(70).ParameterStringCell(ParameterDictionary["var_SonstigesBausatz"],null, true, true);
            });
        });
    }

    void EntranceData(IContainer container)
    {
        container.Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintSecondaryColor = new SKPaint
                {
                    Color = SKColor.Parse(secondaryColor),
                    IsAntialias = true,
                };
                using var path = new SKPath();
                path.MoveTo(0, 12);
                path.LineTo(size.Width, 12);
                path.ArcTo(size.Width, 0, 0, 0, 4);
                path.ArcTo(0, 0, 0, 12, 4);
                path.Close();
                canvas.DrawPath(path, paintSecondaryColor);
            });
            layers.PrimaryLayer().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(75);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                table.Cell().Row(1).Column(1).PaddingLeft(15).Text("Zugänge").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(2).AlignCenter().Text("0").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(3).AlignCenter().Text("1").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(4).AlignCenter().Text("2").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(5).AlignCenter().Text("3").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(6).AlignCenter().Text("4").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(7).AlignCenter().Text("5").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(8).AlignCenter().Text("6").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(9).AlignCenter().Text("7").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(10).AlignCenter().Text("8").FontColor(onSecondaryColor);
                table.Cell().Row(1).Column(11).AlignCenter().Text("9").FontColor(onSecondaryColor);

                table.Cell().Row(2).Column(1).PaddingLeft(10).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_A"], true, "Zugang A");
                foreach (var haltestelle in ParameterDictionary.Keys.Where(x => x.StartsWith("var_ZugangA")))
                {
                    table.Cell().Border(0.1f).BorderColor(secondaryColor).AlignCenter().Text(ParameterDictionary[haltestelle].Value);
                }
                table.Cell().Row(3).Column(1).PaddingLeft(10).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_B"], true, "Zugang B");
                foreach (var haltestelle in ParameterDictionary.Keys.Where(x => x.StartsWith("var_ZugangB")))
                {
                    table.Cell().Border(0.1f).BorderColor(secondaryColor).AlignCenter().Text(ParameterDictionary[haltestelle].Value);
                }
                table.Cell().Row(4).Column(1).PaddingLeft(10).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_C"], true, "Zugang C");
                foreach (var haltestelle in ParameterDictionary.Keys.Where(x => x.StartsWith("var_ZugangC")))
                {
                    table.Cell().Border(0.1f).BorderColor(secondaryColor).AlignCenter().Text(ParameterDictionary[haltestelle].Value);
                }
                table.Cell().Row(5).Column(1).PaddingLeft(10).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_D"], true, "Zugang D");
                foreach (var haltestelle in ParameterDictionary.Keys.Where(x => x.StartsWith("var_ZugangD")))
                {
                    table.Cell().Border(0.1f).BorderColor(secondaryColor).AlignCenter().Text(ParameterDictionary[haltestelle].Value);
                }
                table.Cell().Row(6).Column(1).PaddingLeft(15).Text("Etagenabstände");
                foreach (var etagenHoehe in ParameterDictionary.Keys.Where(x => x.StartsWith("var_Etagenhoehe")))
                {
                    if (string.IsNullOrWhiteSpace(ParameterDictionary[etagenHoehe].Value) || ParameterDictionary[etagenHoehe].Value == "0")
                        break;
                    table.Cell().Border(0.1f).BorderColor(secondaryColor).AlignCenter().Text(ParameterDictionary[etagenHoehe].Value);
                }

                var haupthalteStelle = ParameterDictionary["var_Haupthaltestelle"].Value;

                if (!string.IsNullOrWhiteSpace(haupthalteStelle) )
                {
                    if (haupthalteStelle == "NV") return;
                    uint row = haupthalteStelle[3..4] switch
                    {
                        "A" => 2,
                        "B" => 3,
                        "C" => 4,
                        "D" => 5,
                        _ => 2,
                    };
                    uint col = Convert.ToUInt32(haupthalteStelle[4..5]) + 2;
                    table.Cell().Row(row).Column(col).Padding(0.5f).Border(1).BorderColor(primaryColor);
                }
            });
        });
    }
}
