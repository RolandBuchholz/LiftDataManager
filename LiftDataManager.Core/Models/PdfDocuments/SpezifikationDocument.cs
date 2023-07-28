using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Models.PdfDocuments;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace PDFTests.Services.DocumentGeneration;

public class SpezifikationDocument : PdfBaseDocument
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly bool LowPrintColor;
    private readonly bool LowHighlightColor;

    public SpezifikationDocument(ObservableDictionary<string, Parameter> parameterDictionary, ICalculationsModule calculationsModuleService, bool lowPrintColor, bool lowHighlightColor)
    {
        ParameterDictionary = parameterDictionary;
        _calculationsModuleService = calculationsModuleService;
        Title = "Spezifikation";
        LowPrintColor = lowPrintColor;
        LowHighlightColor = lowHighlightColor;
        SetPdfStyle(LowPrintColor, LowHighlightColor);
    }

    protected override void Content(IContainer container)
    {
        container.PaddingLeft(10, Unit.Millimetre).PaddingRight(10, Unit.Millimetre).DefaultTextStyle(x => x.FontSize(fontSizeXS)).Column(column =>
        {
            column.Item().AlignRight().Row(row =>
            {
                row.ConstantItem(70).ParameterDateCell(ParameterDictionary["var_ErstelltAm"], false, true, null);
                row.ConstantItem(70).ParameterStringCell(ParameterDictionary["var_ErstelltVon"], null, false, true, null);
                row.ConstantItem(70).ParameterDateCell(ParameterDictionary["var_StandVom"], false, true, null);
            });
            column.Item().Element(AddressData);
            column.Item().Element(GenerallyData);
            column.Item().Element(ShaftData);
            column.Item().PageBreak();
            column.Item().Element(CarData);
            column.Item().ShowEntire().Element(CarDataDetail);
            column.Item().ShowEntire().Element(CarFrameData);
            column.Item().ShowEntire().Element(LiftDoorData);
            column.Item().ShowEntire().Element(LiftControlerData);
            column.Item().ShowEntire().Element(LiftDriveData);
            column.Item().ShowEntire().Element(LiftEmergencyCallData);
            column.Item().ShowEntire().Element(LiftSignalisationFT);
            column.Item().ShowEntire().Element(LiftSignalisationAT);
            column.Item().ShowEntire().Element(LiftSignalisationWA);
            column.Item().ShowEntire().Element(LiftMaintenance);
            column.Item().ShowEntire().Element(LiftMontage);
            column.Item().ShowEntire().Element(LiftRWA);
            column.Item().Element(LiftVarious);
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
                        using var paintBorderColor = new SKPaint
                        {
                            Color = SKColor.Parse(borderColor),
                            IsAntialias = true,
                            StrokeWidth = 1.5f,
                            IsStroke = true,
                        };
                        canvas.DrawRoundRect(0, 0, size.Width, boxHeight / 2, 4, 4, paintBorderColor);
                    });
                    layers.Layer().PaddingLeft(5).Text(text =>
                    {
                        text.Line("Bauvorhaben:").FontSize(fontSizeXXS).FontColor(borderColor).Bold();
                        text.Line(ParameterDictionary["var_Projekt"].Value);
                    });
                });
                column.Item().PaddingTop(3).Height(boxHeight / 2).Layers(layers =>
                {
                    layers.PrimaryLayer().Canvas((canvas, size) =>
                    {
                        using var paintBorderColor = new SKPaint
                        {
                            Color = SKColor.Parse(borderColor),
                            IsAntialias = true,
                            StrokeWidth = 1.5f,
                            IsStroke = true,
                        };
                        canvas.DrawRoundRect(0, 0, size.Width, boxHeight / 2, 4, 4, paintBorderColor);
                    });
                    layers.Layer().PaddingLeft(5).Text(text =>
                    {
                        text.Line("Betreiber:").FontSize(fontSizeXXS).FontColor(borderColor).Bold();
                        text.Line(ParameterDictionary["var_Betreiber"].Value);
                    });
                });
            });
            row.RelativeItem(6).PaddingLeft(3).Height(boxHeight + 3).Layers(layers =>
            {
                layers.PrimaryLayer().Canvas((canvas, size) =>
                {
                    using var paintBorderColor = new SKPaint
                    {
                        Color = SKColor.Parse(borderColor),
                        IsAntialias = true,
                        StrokeWidth = 1.5f,
                        IsStroke = true,
                    };
                    canvas.DrawRoundRect(0, 0, size.Width, boxHeight + 3, 4, 4, paintBorderColor);
                });

                layers.Layer().PaddingLeft(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(35, Unit.Millimetre);
                        columns.RelativeColumn();
                    });
                    table.Cell().Row(1).Column(1).ColumnSpan(2).Text("Ansprechperson:").FontSize(fontSizeXXS).FontColor(borderColor).Bold();
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
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(11).RotateLeft().AlignMiddle().AlignCenter().Text("Allgemeine Daten").FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Termine").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ParameterDateCell(ParameterDictionary["var_FreigabeErfolgtAm"]);
                table.Cell().Row(1).Column(4).ParameterDateCell(ParameterDictionary["var_Demontage"]);
                table.Cell().Row(1).Column(5).ParameterDateCell(ParameterDictionary["var_AuslieferungAm"]);
                table.Cell().Row(1).Column(6).ParameterDateCell(ParameterDictionary["var_FertigstellungAm"], false, true, null);
                table.Cell().Row(2).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Grundinformationen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(2).Column(3).ParameterStringCell(ParameterDictionary["var_Lieferart"]);
                table.Cell().Row(2).Column(4).ParameterStringCell(ParameterDictionary["var_InformationAufzug"]);
                table.Cell().Row(2).Column(5).ParameterStringCell(ParameterDictionary["var_FabriknummerBestand"]);
                table.Cell().Row(2).Column(6).ParameterStringCell(ParameterDictionary["var_CeNummer"]);
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Gebaeudetyp"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_FE_Gebaeude"], null, false, false, "Gebäudetyp Zusatzinformationen");
                table.Cell().Row(4).RowSpan(6).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Anlagedaten").FontSize(fontSizeXS).FontColor(borderColor).Bold();
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
                table.Cell().Row(10).RowSpan(2).Column(2).PaddingLeft(5).AlignMiddle().Text("Normen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
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
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 69, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 69, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(7).RotateLeft().AlignMiddle().AlignCenter().Text("Schacht").FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(1).ColumnSpan(6).Element(EntranceData);
                table.Cell().Row(2).RowSpan(2).Column(2).BorderHorizontal(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schacht").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(2).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_SchachtInformation"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_Schacht"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_Befestigung"]);
                });
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Maschinenraum"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Schachtgrubenleiter"]);
                table.Cell().Row(4).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schachtabmessungen\n(innen im Lichten)").FontSize(fontSizeXS).FontColor(borderColor).Bold();
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
                table.Cell().Row(6).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schachtgerüst").FontSize(fontSizeXS).FontColor(borderColor).Bold();

                table.Cell().Row(6).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_GeruestFarbe"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_GeruestFeldfuellung"]);
                });
                table.Cell().Row(7).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Brandschutz Schacht").FontSize(fontSizeXS).FontColor(borderColor).Bold();

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
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(20).RotateLeft().AlignMiddle().AlignCenter().Text("Fahrkorb Grunddaten").FontColor(borderColor).Bold();
                table.Cell().Row(1).RowSpan(8).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Abmessungen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
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
                table.Cell().Row(1).RowSpan(8).Column(5).ColumnSpan(2).Padding(5).AlignCenter().MaxHeight(170).Component(new CarDesignComponent(ParameterDictionary, LowPrintColor));
                table.Cell().Row(9).RowSpan(5).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Seitenwände").FontSize(fontSizeXS).FontColor(borderColor).Bold();
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
                table.Cell().Row(14).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Decke").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(14).Column(3).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_Decke"]);
                table.Cell().Row(14).Column(6).ParameterStringCell(ParameterDictionary["var_RAL_Decke"]);
                table.Cell().Row(15).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_BelagAufDemKabinendach"]);
                table.Cell().Row(16).RowSpan(5).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Boden").FontSize(fontSizeXS).FontColor(borderColor).Bold();
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
                table.Cell().Row(20).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_BodenbelagBeschreibung"], null, false, true);
            });
        });
    }

    void CarDataDetail(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(13).RotateLeft().AlignMiddle().AlignCenter().Text("Fahrkorb Ausstattung").FontColor(borderColor).Bold();
                table.Cell().Row(1).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Beleuchtung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Beleuchtung"]);
                table.Cell().Row(1).Column(5).ParameterStringCell(ParameterDictionary["var_AnzahlBeleuchtung"], "Stk");
                table.Cell().Row(1).Column(6).ParameterStringCell(ParameterDictionary["var_FarbtemperaturBeleuchtung"], "Kelvin");
                table.Cell().Row(2).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_abgDecke"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_NotlichtKab"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_52"]);
                });
                table.Cell().Row(3).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_Dimmer"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_NotlichtTab"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_1"]);
                });

                table.Cell().Row(4).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Spiegel").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(4).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Spiegel"]);
                table.Cell().Row(4).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SpiegelA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SpiegelB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SpiegelC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SpiegelD"], true, "Seite D");
                });
                table.Cell().Row(5).Column(3).ParameterStringCell(ParameterDictionary["var_BreiteSpiegel"], "mm");
                table.Cell().Row(5).Column(4).ParameterStringCell(ParameterDictionary["var_HoeheSpiegel"], "mm");
                table.Cell().Row(5).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_InfoSpiegel"]);

                table.Cell().Row(6).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_SpiegelAntiKratzf"], true, "Antisplitterfolie");
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_SpiegelPaneel"], true);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_Spiegelleiste"], true);
                });
                table.Cell().Row(6).Column(5).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_2"]);
                table.Cell().Row(6).Column(6).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_3"]);

                table.Cell().Row(7).RowSpan(5).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Ausstattung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(7).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_Handlauf"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_HoeheHandlauf"], "mm");
                });
                table.Cell().Row(7).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_HandlaufA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_HandlaufB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_HandlaufC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_HandlaufD"], true, "Seite D");
                });
                table.Cell().Row(8).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Sockelleiste"]);
                table.Cell().Row(8).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SockelleisteA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SockelleisteB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SockelleisteC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_SockelleisteD"], true, "Seite D");
                });
                table.Cell().Row(9).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_Rammschutz"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_HoeheRammschutz"], "mm");
                });
                table.Cell().Row(9).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_RammschutzA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_RammschutzB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_RammschutzC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_RammschutzD"], true, "Seite D");
                });
                table.Cell().Row(10).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Aussenverkleidung_ZT"]);
                table.Cell().Row(10).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussenverkleidungA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussenverkleidungB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussenverkleidungC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussenverkleidungD"], true, "Seite D");
                });
                table.Cell().Row(11).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Paneelmaterial"]);
                table.Cell().Row(11).Column(5).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_PaneelPosA"], true, "Seite A");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_PaneelPosB"], true, "Seite B");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_PaneelPosC"], true, "Seite C");
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_PaneelPosD"], true, "Seite D");
                });
                table.Cell().Row(12).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Fahkorb Ventilator").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(12).Column(3).ParameterBoolCell(ParameterDictionary["var_VentilatorLuftmenge"], false, "Ventilator 90 m³/h");
                table.Cell().Row(12).Column(4).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_VentilatorAnzahl"], "Stk", true);
                table.Cell().Row(13).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Fahkorb").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(13).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_SonstigesFahrkorb"], null, true, true);
            });
        });
    }

    void CarFrameData(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                var carFrameType= _calculationsModuleService.GetCarFrameTyp(ParameterDictionary);  
                var cWTRailName = carFrameType?.DriveTypeId == 2 ? "Schienen Joch" : "Schienen GGW";
                var cWTGuideName = carFrameType?.DriveTypeId == 2 ? "Führungsart Joch" : "Führungsart GGW";

                table.Cell().Row(1).Column(1).RowSpan(19).RotateLeft().AlignMiddle().AlignCenter().Text("Bausatz").FontColor(borderColor).Bold();
                table.Cell().Row(1).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Bausatztyp").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).Row(row =>
                {
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_Bausatz"]);
                    row.RelativeItem(1).Border(0.1f)
                                       .BorderColor(borderColor)
                                       .PaddingLeft(5).PaddingTop(0)
                                       .PaddingBottom(-10).Text(text => 
                                       {
                                           text.Line("Rahmengewicht").FontSize(fontSizeXXS).FontColor(borderColor).Bold();
                                           text.Line($"{_calculationsModuleService.GetCarFrameWeight(ParameterDictionary)} kg");
                                       });
                });
                table.Cell().Row(1).RowSpan(18).Column(5).ColumnSpan(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingHorizontal(5).Element(CarFrameDetailData);
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Bausatz_ZT"]);
                table.Cell().Row(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).Padding(5).AlignMiddle().Text("Führungsart FK").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(3).Column(3).ParameterStringCell(ParameterDictionary["var_Fuehrungsart"]);
                table.Cell().Row(3).Column(4).ParameterStringCell(ParameterDictionary["var_TypFuehrung"]);
                table.Cell().Row(4).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text(cWTGuideName).FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(4).Column(3).ParameterStringCell(ParameterDictionary["var_Fuehrungsart_GGW"], null, false,false, carFrameType?.DriveTypeId == 2 ? "Führungsart Joch" : "Führungsart GGW");
                table.Cell().Row(4).Column(4).ParameterStringCell(ParameterDictionary["var_TypFuehrung_GGW"], null, false, false, carFrameType?.DriveTypeId == 2 ? "Typ Führung Joch" : "Typ Führung GGW");
                table.Cell().Row(5).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schienen FK").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(5).Column(3).ParameterStringCell(ParameterDictionary["var_FuehrungsschieneFahrkorb"]);
                table.Cell().Row(5).Column(4).ParameterStringCell(ParameterDictionary["var_StatusFuehrungsschienen"]);
                table.Cell().Row(6).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text(cWTRailName).FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(6).Column(3).ParameterStringCell(ParameterDictionary["var_FuehrungsschieneGegengewicht"], null, false, false, carFrameType?.DriveTypeId == 2 ? "Führungsschiene Joch" : "Führungsschiene Gegengewicht");
                table.Cell().Row(6).Column(4).ParameterStringCell(ParameterDictionary["var_StatusGGWSchienen"], null, false, false, carFrameType?.DriveTypeId == 2 ? "Status Führungsschienen Joch" : "Status Führungsschienen GGW");
                table.Cell().Row(7).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Fangvorrichtung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(7).Column(3).ParameterStringCell(ParameterDictionary["var_Fangvorrichtung"]);
                table.Cell().Row(7).Column(4).ParameterStringCell(ParameterDictionary["var_TypFV"]);
                table.Cell().Row(8).Column(3).ColumnSpan(2).Border(0.1f)
                                       .BorderColor(borderColor)
                                       .PaddingLeft(5).PaddingTop(0)
                                       .PaddingBottom(-10).Text(text =>
                                       {
                                           text.Line("Fangvorrichtungsbereich").FontSize(fontSizeXXS).FontColor(borderColor).Bold();
                                           var safteyGearResult = _calculationsModuleService.GetSafetyGearCalculation(ParameterDictionary);
                                           text.Line($"{safteyGearResult.MinLoad} - {safteyGearResult.MaxLoad} kg | {safteyGearResult.CarRailSurface} / {safteyGearResult.Lubrication} | Schienenkopf : {safteyGearResult.AllowedRailHeads}");
                                       });
                table.Cell().Row(9).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Geschwindigkeits- begrenzer").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(9).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Geschwindigkeitsbegrenzer"]);
                table.Cell().Row(10).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Geschwindigkeitsbeg_ZT"]);
                table.Cell().Row(11).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Ersatzmaßnahmen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(11).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Ersatzmassnahmen"]);
                table.Cell().Row(12).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Ersatzmaßnahmen_ZT"]);
                table.Cell().Row(13).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schachtinformationen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(13).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Schachtinformationssystem"]);
                table.Cell().Row(14).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Schachtinformation_ZT"]);
                table.Cell().Row(15).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Lastmesseinrichtung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(15).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Lastmesseinrichtung"]);
                table.Cell().Row(16).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Lastmesseinrichtung_ZT"]);
                table.Cell().Row(17).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Beschichtung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(17).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Beschichtung"],null,false,false,"Beschichtungsart Fangrahmen");
                table.Cell().Row(18).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_RALTonTragrahmen"], null, false, false, "Beschichtungsart Fangrahmen");
                table.Cell().Row(19).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Bausatz").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(19).Column(3).ColumnSpan(4).MinHeight(70).ParameterStringCell(ParameterDictionary["var_SonstigesBausatz"],null, true, true);
            });
        });
    }

    void LiftDoorData(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                bool variableDoorData = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_Variable_Tuerdaten");
                bool entranceB = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_B");
                bool entranceC = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_C");
                bool entranceD = LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_ZUGANSSTELLEN_D");

                table.Cell().Row(1).Column(1).RowSpan(31).RotateLeft().AlignMiddle().AlignCenter().Text("Türen").FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text(variableDoorData ? "Tür Fabrikat A": "Türen Fabrikat").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuertyp"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuerbezeichnung"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_ZulassungTuere"], null, false, true);
                });
                table.Cell().Row(2).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text(variableDoorData ? "Tür Abmessungen A": "Türen Abmessungen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(2).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_TB"],"mm", false, false, "Türbreite");
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_TH"],"mm", false, false, "Türhöhe");
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuergewicht"],"kg");
                });
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Tueroeffnung"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AnzahlTuerfluegel"],"Stk");
                table.Cell().Row(4).Column(2).ShowIf(entranceB && variableDoorData).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tür Fabrikat B").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(4).Column(3).ColumnSpan(4).ShowIf(entranceB && variableDoorData).Row(row =>
                {
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuertyp_B"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuerbezeichnung_B"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_ZulassungTuere_B"], null, false, true);
                });
                table.Cell().Row(5).RowSpan(2).Column(2).ShowIf(entranceB && variableDoorData).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tür Abmessungen B").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(5).Column(3).ColumnSpan(4).ShowIf(entranceB && variableDoorData).Row(row =>
                {
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_TB_B"], "mm", false, false, "Türbreite");
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_TH_B"], "mm", false, false, "Türhöhe");
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuergewicht_B"], "kg");
                });
                table.Cell().Row(6).Column(3).ColumnSpan(2).ShowIf(entranceB && variableDoorData).ParameterStringCell(ParameterDictionary["var_Tueroeffnung_B"]);
                table.Cell().Row(6).Column(5).ColumnSpan(2).ShowIf(entranceB && variableDoorData).ParameterStringCell(ParameterDictionary["var_AnzahlTuerfluegel_B"], "Stk");
                table.Cell().Row(7).Column(2).BorderBottom(0.1f).ShowIf(entranceC && variableDoorData).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tür Fabrikat C").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(7).Column(3).ColumnSpan(4).ShowIf(entranceC && variableDoorData).Row(row =>
                {
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuertyp_C"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuerbezeichnung_C"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_ZulassungTuere_C"], null, false, true);
                });
                table.Cell().Row(8).RowSpan(2).Column(2).ShowIf(entranceC && variableDoorData).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tür Abmessungen C").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(8).Column(3).ColumnSpan(4).ShowIf(entranceC && variableDoorData).Row(row =>
                {
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_TB_C"], "mm", false, false, "Türbreite");
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_TH_C"], "mm", false, false, "Türhöhe");
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuergewicht_C"], "kg");
                });
                table.Cell().Row(9).Column(3).ColumnSpan(2).ShowIf(entranceC && variableDoorData).ParameterStringCell(ParameterDictionary["var_Tueroeffnung_C"]);
                table.Cell().Row(9).Column(5).ColumnSpan(2).ShowIf(entranceC && variableDoorData).ParameterStringCell(ParameterDictionary["var_AnzahlTuerfluegel_C"], "Stk");

                table.Cell().Row(10).Column(2).ShowIf(entranceD && variableDoorData).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tür Fabrikat D").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(10).Column(3).ColumnSpan(4).ShowIf(entranceD && variableDoorData).Row(row =>
                {
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuertyp_D"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuerbezeichnung_D"]);
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_ZulassungTuere_D"], null, false, true);
                });
                table.Cell().Row(11).RowSpan(2).Column(2).ShowIf(entranceD && variableDoorData).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tür Abmessungen D").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(11).Column(3).ColumnSpan(4).ShowIf(entranceD && variableDoorData).Row(row =>
                {
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_TB_D"], "mm", false, false, "Türbreite");
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_TH_D"], "mm", false, false, "Türhöhe");
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_Tuergewicht_D"], "kg");
                });
                table.Cell().Row(12).Column(3).ColumnSpan(2).ShowIf(entranceD && variableDoorData).ParameterStringCell(ParameterDictionary["var_Tueroeffnung_D"]);
                table.Cell().Row(12).Column(5).ColumnSpan(2).ShowIf(entranceD && variableDoorData).ParameterStringCell(ParameterDictionary["var_AnzahlTuerfluegel_D"], "Stk");
                //Schachttüren
                table.Cell().Row(13).Column(2).ColumnSpan(5).PaddingLeft(0.25f).PaddingRight(0.75f).Background(secondaryColor).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schachttüren").FontSize(fontSizeXS).FontColor(onPrimaryVariantColor).Bold();
                table.Cell().Row(14).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Material").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(14).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Tueroberflaeche"]);
                table.Cell().Row(14).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_RalSchachtuere"]);
                table.Cell().Row(15).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schwellen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(15).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Schwellenprofil"]);
                table.Cell().Row(15).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AnmerkungSchwelleST"]);
                table.Cell().Row(16).RowSpan(6).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schachttür Optionen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(16).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_StSchuerzeV2A"]);
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_STdghSchwellenwinkel"]);
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_KontaktSchutzraumueberwachung"]);
                });
                table.Cell().Row(17).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_STflRollen"]);
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_SThlRollen"]);
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_StSchwellenheizung"]);
                });
                table.Cell().Row(18).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_Estrichbleche"]);
                    row.RelativeItem(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_53"]);
                    
                });
                table.Cell().Row(19).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_Schwellenbeleuchtung"]);
                    row.RelativeItem(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_4"]);
                });

                table.Cell().Row(20).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_MauerumfassungszargenTF"]);
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_Mauerumfassungszargen"]);
                });
                table.Cell().Row(21).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_PortaleTF"]);
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_Portale"]);
                });
                table.Cell().Row(22).Column(3).ColumnSpan(4).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_5"]);
                //Kabinentüren
                table.Cell().Row(23).Column(2).ColumnSpan(5).PaddingLeft(0.25f).PaddingRight(0.75f).Background(secondaryColor).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Kabinentüren").FontSize(fontSizeXS).FontColor(onPrimaryVariantColor).Bold();


                table.Cell().Row(24).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Antrieb").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(24).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Tuersteuerung"]);
                table.Cell().Row(24).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_ErgAnTuere"]);
                table.Cell().Row(25).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Lichtgitter"]);
                table.Cell().Row(25).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Tuerverriegelung"]);
                table.Cell().Row(26).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Material").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(26).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_OberflaecheKabinentuere"]);
                table.Cell().Row(26).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_RalKabinentuere"]);
                table.Cell().Row(27).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schwellen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(27).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_SchwellenprofilKabTuere"]);
                table.Cell().Row(27).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AnmerkungSchwelleKT"]);
                table.Cell().Row(28).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Kabinentür Optionen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(28).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_KtSchuerzeV2A"]);
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_Schuerzeverstaerkt"]);
                    row.RelativeItem(1).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_54"]);
                });
                table.Cell().Row(29).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_KTflRollen"]);
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_KThlRollen"]);
                    row.RelativeItem(1).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_55"]);
                });
                table.Cell().Row(30).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_EcoPlus"]);
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_KtSchwellenheizung"]);
                    row.RelativeItem(1).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_6"]);
                });
                table.Cell().Row(31).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Kabinentüren").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(31).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_SonstigesKabinentuere"], null, true, true);
            });
        });
    }

    void LiftControlerData(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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


                table.Cell().Row(1).Column(1).RowSpan(15).RotateLeft().AlignMiddle().AlignCenter().Text("Steuerung").FontColor(borderColor).Bold();
                table.Cell().Row(1).RowSpan(11).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Steuerung/ELT").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Steuerungstyp"]);
                table.Cell().Row(1).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AnmerkungSteuerung"], null, false, true);
                table.Cell().Row(2).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(2).ParameterStringCell(ParameterDictionary["var_XKnopfsteuerung"], null, true);
                    row.RelativeItem(1).ParameterBoolCell(ParameterDictionary["var_Gruppe"], true, "Gruppe mit" );
                    row.RelativeItem(1).ParameterStringCell(ParameterDictionary["var_GruppeMit"], null, true, false);
                    row.RelativeItem(2).ParameterBoolCell(ParameterDictionary["var_AllstromSensitiverFI"]);
                });
                table.Cell().Row(3).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_VorzugInnen"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_VorzugAussen"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_Notstromsteuerung"]);
                });
                table.Cell().Row(4).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_Parkhaltestelle"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_dynBrandfall"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_einfacherBrandfall"]);
                });
                table.Cell().Row(5).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_EinfahrtmoeffnTuer"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_halogenfrVerdraht"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_8"]);
                });
                table.Cell().Row(6).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_SchachtbIP54"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_FeuerUndMittelschutz"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_10"]);
                });
                table.Cell().Row(7).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_Potentialausgleich"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_SchachtgrubenleiterKontaktgesichert"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_12"]);
                });
                table.Cell().Row(8).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_ErsatzmassnahmenSK"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_11"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_15"]);
                });
                table.Cell().Row(9).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_ErsatzmassnahmenSG"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_14"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_18"]);
                });
                table.Cell().Row(10).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_SchutzgelaenderKontakt"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_17"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_21"]);
                });
                table.Cell().Row(11).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_NotlichtKab"],false, "WECO Notstromgerät");
                    row.RelativeItem(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_20"]);
                    
                });
                table.Cell().Row(12).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Stromanschluß").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(12).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Stromanschluss"]);
                table.Cell().Row(12).Column(5).ColumnSpan(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_22"]);
                table.Cell().Row(13).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schaltschrank").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(13).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_LageSchaltschrank"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_Schaltschrankoberflaeche"]);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_SchaltschrankRAL"]);
                });
                table.Cell().Row(14).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_Schaltschrankgroesse"]);
                table.Cell().Row(15).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Steuerung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(15).Column(3).ColumnSpan(4).MinHeight(30).ParameterStringCell(ParameterDictionary["var_SonstigesSteuerung"], null, true,true);
            });
        });
    }

    void LiftDriveData(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(5).RotateLeft().AlignMiddle().AlignCenter().Text("Antrieb").FontColor(borderColor).Bold();
                table.Cell().Row(2).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Antrieb").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(2).Column(3).ParameterStringCell(ParameterDictionary["var_Aggregat"]);
                table.Cell().Row(2).Column(4).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_AnmerkungAntrieb"]);
                table.Cell().Row(3).Column(3).ParameterStringCell(ParameterDictionary["var_Getriebe"]);
                table.Cell().Row(3).Column(4).ColumnSpan(3).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_Evac3C"], true);
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_USVEvak"], true);
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_RevconZarec"], true);
                });
                table.Cell().Row(4).Column(3).ParameterStringCell(ParameterDictionary["var_Handlueftung"]);
                table.Cell().Row(4).Column(4).ColumnSpan(3).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_Fremdbelueftung"], true);
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_ElektrBremsenansteuerung"], true);
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_Treibscheibegehaertet"], true);
                });
                table.Cell().Row(5).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Antrieb").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(5).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem(3).ParameterStringCell(ParameterDictionary["var_SonstigesAntrieb"]);
                    row.RelativeItem(2).BorderLeft(0.1f).BorderTop(0.1f).BorderColor(borderColor).PaddingHorizontal(5).Element(DriveDetailData);
                });
            });
        });
    }

    void LiftEmergencyCallData(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(14).RotateLeft().AlignMiddle().AlignCenter().Text("Notruf").FontColor(borderColor).Bold();
                table.Cell().Row(2).Column(2).RowSpan(5).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Notruf").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Notruftyp"]);
                table.Cell().Row(2).Column(5).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_Steuerung"], true);
                table.Cell().Row(2).Column(6).PaddingTop(5).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_23"], true);
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Notruf"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_NotrufNetz"]);
                table.Cell().Row(4).Column(3).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_NrGesch"], true);
                table.Cell().Row(4).Column(4).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_NrZusGesch"]);
                table.Cell().Row(4).Column(6).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_Schluesseltresor"], true);
                table.Cell().Row(5).Column(3).ParameterBoolCell(ParameterDictionary["var_Sprechanlage3stellig"]);
                table.Cell().Row(5).Column(4).ParameterBoolCell(ParameterDictionary["var_Kabine"]);
                table.Cell().Row(5).Column(5).ParameterBoolCell(ParameterDictionary["var_Kabdach"]);
                table.Cell().Row(5).Column(6).ParameterBoolCell(ParameterDictionary["var_Schgrube"]);
                table.Cell().Row(6).Column(3).ColumnSpan(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).Text("Notruftaster: Kabine / Dach / Schachtgrube");
                table.Cell().Row(6).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_NotruftasterKabineDachSG"], null, true);
                table.Cell().Row(7).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Notruf").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(7).Column(3).ColumnSpan(4).MinHeight(30).ParameterStringCell(ParameterDictionary["var_NotrufSonstiges"], null, true, true);
            });
        });
    }

    void LiftSignalisationFT(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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


                table.Cell().Row(1).Column(1).RowSpan(21).RotateLeft().AlignMiddle().AlignCenter().Text("Signalisation Kabinentableau").FontColor(borderColor).Bold();
                table.Cell().Row(1).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Kabinentableau").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_KabTabKabinentableau"]);
                table.Cell().Row(1).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_FT_Info1"],null, false, true);
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_KabTabAufbau"]);
                table.Cell().Row(2).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_FT_Info2"]);
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_KabTabMaterial"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_FT_RAL"]);
                table.Cell().Row(4).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Standanzeiger").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(4).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_KabTabPunktmatrixTyp"]);
                table.Cell().Row(4).Column(5).ParameterStringCell(ParameterDictionary["var_KabTabFarbe"]);
                table.Cell().Row(4).Column(6).ParameterStringCell(ParameterDictionary["var_KabTabTFTBildschirmTyp"]);
                table.Cell().Row(5).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_KabTabPfeilescrollend"],true);
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_KabTabPfeilestat"],true);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_KabTabLCDAnzeigeTyp"]);
                });
                table.Cell().Row(6).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Gravuren Texte").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(6).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabAufzugimBrandfall"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabFirmenlogo"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabHinweisNotrufbenutzung"]);
                });
                table.Cell().Row(7).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabPiktogrEN8173"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabAufzugdaten"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabSchluesselschalter"]);
                });
                table.Cell().Row(8).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Gegensprechanlage").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(8).Column(3).ParameterBoolCell(ParameterDictionary["var_KabTabKabine"]);
                table.Cell().Row(8).Column(4).ParameterBoolCell(ParameterDictionary["var_KabTabSteuerung"]);
                table.Cell().Row(8).Column(5).ParameterBoolCell(ParameterDictionary["var_KabTabSchachtgr"]);
                table.Cell().Row(8).Column(6).ParameterBoolCell(ParameterDictionary["var_KabTabmitNotrufkombiniert"]);
                table.Cell().Row(9).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sprachcomputer").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(9).Column(3).ColumnSpan(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_24"]);
                table.Cell().Row(9).Column(5).ParameterBoolCell(ParameterDictionary["var_KabTabinLiefenth"]);
                table.Cell().Row(9).Column(6).ParameterBoolCell(ParameterDictionary["var_KabTabkundenseitig"]);
                table.Cell().Row(10).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Leuchtanzeige").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(10).Column(3).ColumnSpan(3).ParameterBoolCell(ParameterDictionary["var_KabTabMultifunktionsanzeigeinklTKSueLSumNL"]);
                table.Cell().Row(10).Column(6).ParameterBoolCell(ParameterDictionary["var_KabTabueberlastinklSummer"]);
                table.Cell().Row(11).Column(3).ParameterBoolCell(ParameterDictionary["var_KabTabBeschriftfeldinclTKSNL"]);
                table.Cell().Row(11).Column(4).ParameterBoolCell(ParameterDictionary["var_KabTabEvakuierung"]);
                table.Cell().Row(11).Column(5).ParameterBoolCell(ParameterDictionary["var_KabTabBrandfall"]);
                table.Cell().Row(11).Column(6).ParameterBoolCell(ParameterDictionary["var_KabTabFeuerwfahrt"]);
                table.Cell().Row(12).Column(3).ParameterBoolCell(ParameterDictionary["var_KabTabAnzeigeueberStandanzeige"]);
                table.Cell().Row(12).Column(4).ParameterBoolCell(ParameterDictionary["var_KabTabAussBetrieb"]);
                table.Cell().Row(12).Column(5).ColumnSpan(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_25"]);
                table.Cell().Row(13).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Vandalenklasse").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(13).Column(3).ParameterBoolCell(ParameterDictionary["var_KabTabFTVandalenklasse0"]);
                table.Cell().Row(13).Column(4).ParameterBoolCell(ParameterDictionary["var_KabTabFTVandalenklasse1"]);
                table.Cell().Row(13).Column(5).ParameterBoolCell(ParameterDictionary["var_KabTabFTVandalenklasse2"]);
                table.Cell().Row(13).Column(6).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_26"]);
                table.Cell().Row(14).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tasterprogramm").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(14).Column(3).ParameterStringCell(ParameterDictionary["var_KabTabTaster"]);
                table.Cell().Row(14).Column(4).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_Taster_Info"]);
                table.Cell().Row(15).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tasterplatten").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(15).Column(3).ParameterStringCell(ParameterDictionary["var_KabTabTasterplatten"]);
                table.Cell().Row(15).Column(4).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_KabTabTasterplattenmaterial"]);
                table.Cell().Row(15).Column(6).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_KabTabakustischeQuittung"],true);
                table.Cell().Row(16).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Farbe LED-Quittung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(16).Column(3).ParameterStringCell(ParameterDictionary["var_ColLedFT"]);
                table.Cell().Row(16).Column(4).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Quittung_Info"]);
                table.Cell().Row(16).Column(6).BorderTop(0.1f).BorderColor(borderColor).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_Schutzrosette"],true);
                table.Cell().Row(17).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Funktionstaster").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(17).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabTuerAuf"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabTuerZu"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabAufHalten"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabVentilatortaste"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_27"]);
                });
                table.Cell().Row(18).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabSelektivitaet"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_28"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_29"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_30"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_31"]);
                });
                table.Cell().Row(19).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schlüsselschalter").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(19).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabVorzugInnen"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabVentilatorSchalter"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabLichttest"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabPZStd"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_32"]);
                });
                table.Cell().Row(20).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Code- Kartenleser").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(20).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.AutoItem().ParameterBoolCell(ParameterDictionary["var_KabTabCodeKartenleser"],false,"");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_KabTabCodeKartenleserTyp"],null,true);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabCodeKinLiefenth"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabkundenseit"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_KabTabEinbauspaeter"]);
                });
                table.Cell().Row(21).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Kabinentableau").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(21).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_SonstigesKabTab"], null, true, true);
            });
        });
    }

    void LiftSignalisationAT(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(21).RotateLeft().AlignMiddle().AlignCenter().Text("Signalisation Außentableau").FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Außentableau").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AussTabMaterial"]);
                table.Cell().Row(1).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AT_RAL"], null, false, true);
                table.Cell().Row(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Befestigung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AussTabATBefestigung"]);
                table.Cell().Row(2).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AT_Info"]);
                table.Cell().Row(3).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Standanzeiger").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AussTabPunktmatrixTyp"]);
                table.Cell().Row(3).Column(5).ParameterStringCell(ParameterDictionary["var_AussTabFarbe"]);
                table.Cell().Row(3).Column(6).ParameterStringCell(ParameterDictionary["var_AussTabTFTBildschirmTyp"]);
                table.Cell().Row(4).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussTabPfeilescrollend"], true);
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussTabPfeilestat"], true);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_AussTabLCDAnzeigeTyp"]);
                });
                table.Cell().Row(5).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Gravuren Texte").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(5).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_AussTabAufzugimBrandfall"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_AussTabFirmenlogo"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_AussTabHinweisNotrufbenutzung"]);
                });
                table.Cell().Row(6).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_AussTabPiktogrEN8173"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_AussTabSchlschalt"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_33"]);
                });
                table.Cell().Row(7).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Leuchtanzeige").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(7).Column(3).ParameterBoolCell(ParameterDictionary["var_AussTabAuBetrieb"]);
                table.Cell().Row(7).Column(4).ParameterBoolCell(ParameterDictionary["var_AussTabEvakuierung"]);
                table.Cell().Row(7).Column(5).ParameterBoolCell(ParameterDictionary["var_AussTabbesetzt"]);
                table.Cell().Row(7).Column(6).ParameterBoolCell(ParameterDictionary["var_AussTabFeuerwehrfahrt"]);
                table.Cell().Row(8).Column(3).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussTabindividuelleGroee"],true);
                table.Cell().Row(8).Column(4).ParameterStringCell(ParameterDictionary["var_Groesse_Info_AT"]);
                table.Cell().Row(8).Column(5).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussTabanalogTast"],true);
                table.Cell().Row(8).Column(6).PaddingTop(5).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_34"],true);
                table.Cell().Row(9).Column(3).ParameterBoolCell(ParameterDictionary["var_AussTabAnzeigeueberStandanzeige"]);
                table.Cell().Row(9).Column(4).ParameterBoolCell(ParameterDictionary["var_AussTabBrandfall"]);
                table.Cell().Row(9).Column(5).ColumnSpan(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_35"]);
                table.Cell().Row(10).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Richtungs- Weiterfahrtspfeil").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(10).Column(3).ParameterBoolCell(ParameterDictionary["var_AussTabohnePfeile"]);
                table.Cell().Row(10).Column(4).ParameterBoolCell(ParameterDictionary["var_AussTabanalogTaster"]);
                table.Cell().Row(10).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AussTabLeuchtfeldGroee"]);
                table.Cell().Row(11).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Vandalenklasse").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(11).Column(3).ParameterBoolCell(ParameterDictionary["var_AussTabATVandalenklasse0"]);
                table.Cell().Row(11).Column(4).ParameterBoolCell(ParameterDictionary["var_AussTabATVandalenklasse1"]);
                table.Cell().Row(11).Column(5).ParameterBoolCell(ParameterDictionary["var_AussTabATVandalenklasse2"]);
                table.Cell().Row(11).Column(6).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_36"]);
                table.Cell().Row(12).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tasterprogramm").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(12).Column(3).ParameterStringCell(ParameterDictionary["var_AussTabTaster"]);
                table.Cell().Row(12).Column(4).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_Taster_Info_AT"]);
                table.Cell().Row(13).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Tasterplatten").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(13).Column(3).ParameterStringCell(ParameterDictionary["var_AussTabTasterplatten"]);
                table.Cell().Row(13).Column(4).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_AussTabTasterplattenmaterial"]);
                table.Cell().Row(13).Column(6).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussTabakustQuitt"], true);
                table.Cell().Row(14).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Farbe LED-Quittung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(14).Column(3).ParameterStringCell(ParameterDictionary["var_ColLedAT"]);
                table.Cell().Row(14).Column(4).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Quittung_Info_AT"]);
                table.Cell().Row(14).Column(6).BorderTop(0.1f).BorderColor(borderColor).PaddingTop(5).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_51"], true);
                table.Cell().Row(15).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schlüsselschalter").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(15).Column(3).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussTabAbschaltenHaltstelle"],true);
                table.Cell().Row(15).Column(4).ParameterStringCell(ParameterDictionary["var_AbschaltenWo"]);
                table.Cell().Row(15).Column(5).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_AussTabVorzugauenHaltestellen"],true);
                table.Cell().Row(15).Column(6).ParameterStringCell(ParameterDictionary["var_VorzugWo"]);
                table.Cell().Row(16).Column(3).ParameterBoolCell(ParameterDictionary["var_AussTabPZStd"]);
                table.Cell().Row(16).Column(4).ParameterBoolCell(ParameterDictionary["var_AussTabFederrueckz"]);
                table.Cell().Row(16).Column(5).ParameterBoolCell(ParameterDictionary["var_AussTabSchlAbzSt"]);
                table.Cell().Row(16).Column(6).ParameterStringCell(ParameterDictionary["var_StellungenPZ"],"Stellungen",true);
                table.Cell().Row(17).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_37"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_38"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_39"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_40"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_41"]);
                });
                table.Cell().Row(18).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Code- Kartenleser").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(18).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.AutoItem().ParameterBoolCell(ParameterDictionary["var_AussTabCodeKartenleser"], false, "");
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_AussTabCodeKartenleserTyp"], null, true);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_AussTabinLiefenth"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_AussTabkundenseit"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_AussTabEinbauspaeter"]);
                });
                table.Cell().Row(19).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Montage Außentableau").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(19).Column(3).ParameterBoolCell(ParameterDictionary["var_Tuerzargenmontage"]);
                table.Cell().Row(19).Column(4).ParameterBoolCell(ParameterDictionary["var_Mauerwerkmontage"]);
                table.Cell().Row(19).Column(5).ParameterBoolCell(ParameterDictionary["var_MWUmontage"],false, "Mauerumfassungszargen");
                table.Cell().Row(19).Column(6).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_42"]);
                table.Cell().Row(20).Column(3).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_43"]);
                table.Cell().Row(20).Column(4).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_44"]);
                table.Cell().Row(20).Column(5).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_45"]);
                table.Cell().Row(20).Column(6).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_46"]);
                table.Cell().Row(21).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Außentableau").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(21).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_SonstigesAT"], null, true, true);
            });
        });
    }

    void LiftSignalisationWA(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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


                table.Cell().Row(1).Column(1).RowSpan(13).RotateLeft().AlignMiddle().AlignCenter().Text("Signalisation Weiterfahrtsanzeigen").FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Weiterfahrtsanzeigen").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Material"]);
                table.Cell().Row(1).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_WFA_RAL"], null, false, true);
                table.Cell().Row(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Einbau").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_WfaAnbauortWFA"]);
                table.Cell().Row(2).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_WFA_Info"]);
                table.Cell().Row(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Befestigung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_WfaWFABefestigung"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_STA_Info_WFA"]);
                table.Cell().Row(4).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Standanzeiger").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(4).Column(3).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_WfaPunktmatrixTyp"]);
                table.Cell().Row(4).Column(5).ParameterStringCell(ParameterDictionary["var_WfaFarbe"]);
                table.Cell().Row(4).Column(6).ParameterStringCell(ParameterDictionary["var_WfaTFTBildschirmTyp"]);
                table.Cell().Row(5).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_WfaPfeilescrollend"], true);
                    row.RelativeItem().PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_WfaPfeilestat"], true);
                    row.RelativeItem().ParameterStringCell(ParameterDictionary["var_WfaLCDAnzeigeTyp"]);
                });
                table.Cell().Row(6).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Leuchtanzeige").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(6).Column(3).ParameterBoolCell(ParameterDictionary["var_WfaAuBetrieb"]);
                table.Cell().Row(6).Column(4).ParameterBoolCell(ParameterDictionary["var_WfaEvakuierung"]);
                table.Cell().Row(6).Column(5).ParameterBoolCell(ParameterDictionary["var_Wfabesetzt"]);
                table.Cell().Row(6).Column(6).ParameterBoolCell(ParameterDictionary["var_WfaFeuerwehrfahrt"]);
                table.Cell().Row(7).Column(3).ParameterBoolCell(ParameterDictionary["var_WfaIndividuelleGroesse"]);
                table.Cell().Row(7).Column(4).ParameterStringCell(ParameterDictionary["var_Groesse_Info_WFA"],null ,true);
                table.Cell().Row(7).Column(5).ParameterBoolCell(ParameterDictionary["var_WfaAnalogTast"]);
                table.Cell().Row(7).Column(6).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_47"]);
                table.Cell().Row(8).Column(3).ParameterBoolCell(ParameterDictionary["var_WfaAnzeigeueberStandanzeige"]);
                table.Cell().Row(8).Column(4).ParameterBoolCell(ParameterDictionary["var_WfaBrandfall"]);
                table.Cell().Row(8).Column(5).ColumnSpan(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_48"]);
                table.Cell().Row(9).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Richtungs- Weiterfahrtspfeil").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(9).Column(3).Border(0.1f).BorderColor(borderColor).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_WfaOhnePfeile"], true);
                table.Cell().Row(9).Column(4).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_WfaAnalogTaster"], true);
                table.Cell().Row(9).Column(5).ParameterStringCell(ParameterDictionary["var_WfaLeuchtfeldGroee"]);
                table.Cell().Row(9).Column(6).ParameterStringCell(ParameterDictionary["var_Quittung_Info_WFA"]);
                table.Cell().Row(10).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Gong").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(10).Column(3).Border(0.1f).BorderColor(borderColor).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_Wfa1Klang"], true);
                table.Cell().Row(10).Column(4).Border(0.1f).BorderColor(borderColor).PaddingTop(5).ParameterBoolCell(ParameterDictionary["var_Wfa2Klang"], true);
                table.Cell().Row(10).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_WfaGongposition"]);
                table.Cell().Row(11).Column(3).ColumnSpan(4).PaddingLeft(5).Text("Gong ist bei Einzelaufzügen nicht erforderlich wenn das Türgeräusch ausreichend laut ist");
                table.Cell().Row(12).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Aufzugsidentifikation").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(12).Column(3).ColumnSpan(4).Row(row =>
                {
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_WfaohneIdent"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_WfamitIdent"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_WfaV2Agraviert"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_WfaPVCEinl"]);
                    row.RelativeItem().ParameterCustomBoolCell(ParameterDictionary["var_BenDef_49"]);
                });
                table.Cell().Row(13).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges WFA / STA").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(13).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_SonstigesWFA"], null, true, true);
            });
        });
    }

    void LiftMaintenance(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(16).RotateLeft().AlignMiddle().AlignCenter().Text("Wartung").FontColor(borderColor).Bold();
                table.Cell().Row(1).RowSpan(5).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Leistungen angeboten").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_Funktionswartungjaehrl"]);
                table.Cell().Row(1).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Wartung_Info_1"], null, true, true);
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_WartungInspektionnAMEVjaehrlich"]);
                table.Cell().Row(2).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Wartung_Info_2"], null, true);
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_Vollwartungjaehrlich"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Wartung_Info_3"], null, true);
                table.Cell().Row(4).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_WartInspInstVerbesserNAMEVjaehrl"]);
                table.Cell().Row(4).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_Wartung_Info_4"], null, true);
                table.Cell().Row(5).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_Notrufaufschaltung"]);
                table.Cell().Row(5).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_ANNotbefreiungdurch"]);
                table.Cell().Row(6).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Wartung / Notruf").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(6).Column(3).ParameterBoolCell(ParameterDictionary["var_WartungenthMonate"]);
                table.Cell().Row(6).Column(4).ParameterStringCell(ParameterDictionary["var_WA_AnzMonate"], "Monate", true);
                table.Cell().Row(7).Column(3).ParameterBoolCell(ParameterDictionary["var_NotrufenthaltMonate"]);
                table.Cell().Row(7).Column(4).ParameterStringCell(ParameterDictionary["var_NO_AnzMonate"], "Monate", true);
                table.Cell().Row(7).Column(5).ColumnSpan(2).ParameterStringCell(ParameterDictionary["var_WANotbefreiungdurch"]);
                table.Cell().Row(8).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_sonstigesWart"]);
                table.Cell().Row(9).RowSpan(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("VOB-Abnahme / vereinbarte Gewähleistungsdauer").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(9).Column(3).ColumnSpan(4).Row(row => 
                { 
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_VOB2"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_VOB4"]);
                    row.RelativeItem().ParameterBoolCell(ParameterDictionary["var_VOB5"]);
                });
                table.Cell().Row(10).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_SonstigesVOB"], null, false, true);
            });
        });
    }

    void LiftMontage(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(16).RotateLeft().AlignMiddle().AlignCenter().Text("Montage / TÜV").FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Potentialausgleich").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_AnschludurchBE"]);
                table.Cell().Row(1).Column(5).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_Anschlubauseits"],true);
                table.Cell().Row(2).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schachttürmontage").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_VerfugenzumBaukoerper"]);
                table.Cell().Row(2).Column(5).ColumnSpan(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_56"]);
                table.Cell().Row(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Montagepersonal").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_eigenesPersonalerforderlich"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_Subunternehmererlaubt"]);
                table.Cell().Row(4).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Montage / TÜV").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(4).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_sonstigesMoTUEV"], null, true, true);
            });
        });
    }

    void LiftRWA(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
                canvas.DrawLine(100, 0, 100, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(16).RotateLeft().AlignMiddle().AlignCenter().Text("RWA").FontColor(borderColor).Bold();
                table.Cell().Row(1).RowSpan(3).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Schachtentrauchung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(3).ParameterStringCell(ParameterDictionary["var_Schachtentrauchung"]);
                table.Cell().Row(1).Column(4).ColumnSpan(3).ParameterStringCell(ParameterDictionary["var_RwaBezeichnung"], null, false, true);
                table.Cell().Row(2).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_LieferungMontageBE"]);
                table.Cell().Row(2).Column(5).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_LieferungMontagebauseits"]);
                table.Cell().Row(3).Column(3).ColumnSpan(2).ParameterBoolCell(ParameterDictionary["var_potfreierKontakt"]);
                table.Cell().Row(3).Column(5).ColumnSpan(2).ParameterCustomBoolCell(ParameterDictionary["var_BenDef_50"]);
                table.Cell().Row(4).Column(2).BorderBottom(0.1f).BorderColor(borderColor).PaddingLeft(5).AlignMiddle().Text("Sonstiges Rauch- und Wärmeabzugsanlage").FontSize(fontSizeXS).FontColor(borderColor).Bold();
                table.Cell().Row(4).Column(3).ColumnSpan(4).ParameterStringCell(ParameterDictionary["var_sonstigesRWA"], null, true,true);
            });
        });
    }

    void LiftVarious(IContainer container)
    {
        container.PaddingTop(3).Layers(layers =>
        {
            layers.Layer().Canvas((canvas, size) =>
            {
                using var paintBorderColor = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 1.5f,
                    IsStroke = true,
                };
                using var paintBorderColorSmall = new SKPaint
                {
                    Color = SKColor.Parse(borderColor),
                    IsAntialias = true,
                    StrokeWidth = 0.5f,
                    IsStroke = true,
                };
                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 4, 4, paintBorderColor);
                canvas.DrawLine(15, 0, 15, size.Height, paintBorderColorSmall);
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

                table.Cell().Row(1).Column(1).RowSpan(16).RotateLeft().AlignMiddle().AlignCenter().Text("Sonstiges").FontColor(borderColor).Bold();
                table.Cell().Row(1).Column(2).ColumnSpan(5).MinHeight(50).Padding(5).Text(ParameterDictionary["var_sonstigesAnlage"].Value);
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
                path.MoveTo(0.75f, 11.5f);
                path.LineTo(size.Width - 0.75f, 11.5f);
                path.ArcTo(size.Width - 0.75f, 0.75f, 0.75f, 0, 3.25f);
                path.ArcTo(0.75f, 0.75f, 0.75f, 11.5f, 3.25f);
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
                    table.Cell().Border(0.1f).BorderColor(borderColor).AlignCenter().Text(ParameterDictionary[haltestelle].Value);
                }
                table.Cell().Row(3).Column(1).PaddingLeft(10).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_B"], true, "Zugang B");
                foreach (var haltestelle in ParameterDictionary.Keys.Where(x => x.StartsWith("var_ZugangB")))
                {
                    table.Cell().Border(0.1f).BorderColor(borderColor).AlignCenter().Text(ParameterDictionary[haltestelle].Value);
                }
                table.Cell().Row(4).Column(1).PaddingLeft(10).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_C"], true, "Zugang C");
                foreach (var haltestelle in ParameterDictionary.Keys.Where(x => x.StartsWith("var_ZugangC")))
                {
                    table.Cell().Border(0.1f).BorderColor(borderColor).AlignCenter().Text(ParameterDictionary[haltestelle].Value);
                }
                table.Cell().Row(5).Column(1).PaddingLeft(10).ParameterBoolCell(ParameterDictionary["var_ZUGANSSTELLEN_D"], true, "Zugang D");
                foreach (var haltestelle in ParameterDictionary.Keys.Where(x => x.StartsWith("var_ZugangD")))
                {
                    table.Cell().Border(0.1f).BorderColor(borderColor).AlignCenter().Text(ParameterDictionary[haltestelle].Value);
                }
                table.Cell().Row(6).Column(1).PaddingLeft(15).Text("Etagenabstände");
                foreach (var etagenHoehe in ParameterDictionary.Keys.Where(x => x.StartsWith("var_Etagenhoehe")))
                {
                    if (string.IsNullOrWhiteSpace(ParameterDictionary[etagenHoehe].Value) || ParameterDictionary[etagenHoehe].Value == "0")
                        break;
                    table.Cell().Border(0.1f).BorderColor(borderColor).AlignCenter().Text(ParameterDictionary[etagenHoehe].Value);
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
                    table.Cell().Row(row).Column(col).Padding(0.5f).Border(1).BorderColor(borderColor);
                }
            });
        });
    }

    void CarFrameDetailData(IContainer container)
    {
        container.Table(table => 
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(130);
                columns.RelativeColumn(1);
                columns.ConstantColumn(15);
            });

            table.Cell().Row(1).Column(1).ColumnSpan(3).Text("Bausatz Parameter").FontSize(fontSizeS).FontColor(borderColor).Bold();
            table.Cell().Row(2).Column(1).ColumnSpan(3).Text("Basisdaten").FontSize(fontSizeXS).FontColor(borderColor).Bold();
            table.Cell().Row(3).Column(1).Text("Stichmaß:").FontSize(fontSizeXXS);
            table.Cell().Row(3).Column(2).AlignRight().Text(ParameterDictionary["var_Stichmass"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(3).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(4).Column(1).Text("Unterfahrt bis Puffer:").FontSize(fontSizeXXS);
            table.Cell().Row(4).Column(2).AlignRight().Text(ParameterDictionary["var_FUBP"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(4).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(5).Column(1).Text("Ünterfahrt mit Pufferhub:").FontSize(fontSizeXXS);
            table.Cell().Row(5).Column(2).AlignRight().Text(ParameterDictionary["var_RHO"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(5).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(6).Column(1).Text("Puffer Typ FK:").FontSize(fontSizeXXS);
            table.Cell().Row(6).Column(2).AlignRight().Text(ParameterDictionary["var_Puffertyp"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(7).Column(1).Text("Puffer Typ GGW:").FontSize(fontSizeXXS);
            table.Cell().Row(7).Column(2).AlignRight().Text(ParameterDictionary["var_Puffertyp_GGW"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(8).Column(1).Text("Pufferhöhe:").FontSize(fontSizeXXS);
            table.Cell().Row(8).Column(2).AlignRight().Text(ParameterDictionary["var_PUH"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(8).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(9).Column(1).Text("Fangrahmengewicht:").FontSize(fontSizeXXS);
            table.Cell().Row(9).Column(2).AlignRight().Text($"{ _calculationsModuleService.GetCarFrameWeight(ParameterDictionary)}").FontSize(fontSizeXXS);
            table.Cell().Row(9).Column(3).AlignLeft().PaddingLeft(2).Text("kg").FontSize(fontSizeXXS);
            table.Cell().Row(10).Column(1).Text("GGW-Rahmengewicht:").FontSize(fontSizeXXS);
            table.Cell().Row(10).Column(2).AlignRight().Text(ParameterDictionary["var_GGW_Rahmen_Gewicht"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(10).Column(3).AlignLeft().PaddingLeft(2).Text("kg").FontSize(fontSizeXXS);
            table.Cell().Row(11).Column(1).Text("GGW-Füllungsgewicht:").FontSize(fontSizeXXS);
            table.Cell().Row(11).Column(2).AlignRight().Text(ParameterDictionary["var_GGW_Fuellgewicht"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(11).Column(3).AlignLeft().PaddingLeft(2).Text("kg").FontSize(fontSizeXXS);
            table.Cell().Row(12).Column(1).Text("Schienenlänge Fahrkorb:").FontSize(fontSizeXXS);
            table.Cell().Row(12).Column(2).AlignRight().Text(ParameterDictionary["var_Schienenlaenge"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(12).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(13).Column(1).Text("Schienenlänge Gegengewicht:").FontSize(fontSizeXXS);
            table.Cell().Row(13).Column(2).AlignRight().Text(ParameterDictionary["var_Hilfsschienenlaenge"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(13).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(14).Column(1).Text("Startschiene Fahrkorb:").FontSize(fontSizeXXS);
            table.Cell().Row(14).Column(2).AlignRight().Text(ParameterDictionary["var_Startschiene"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(14).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(15).Column(1).Text("Startschiene Gegengewicht:").FontSize(fontSizeXXS);
            table.Cell().Row(15).Column(2).AlignRight().Text(ParameterDictionary["var_HilfsschienenlaengeStartstueck"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(15).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(16).Column(1).Text("Fahrkorb Endstück:").FontSize(fontSizeXXS);
            table.Cell().Row(16).Column(2).AlignRight().Text(ParameterDictionary["var_Endschiene"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(16).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(17).Column(1).Text("Gegengewicht Endstück:").FontSize(fontSizeXXS);
            table.Cell().Row(17).Column(2).AlignRight().Text(ParameterDictionary["var_HilfsschienenlaengeEndstueck"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(17).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(18).Column(1).ColumnSpan(3).Text("Seildaten").FontSize(fontSizeXS).FontColor(borderColor).Bold();
            table.Cell().Row(19).Column(1).Text("Treibscheibendurchmesser:").FontSize(fontSizeXXS);
            table.Cell().Row(19).Column(2).AlignRight().Text(ParameterDictionary["var_Treibscheibendurchmesser"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(19).Column(3).AlignLeft().PaddingLeft(2).Text("mm").FontSize(fontSizeXXS);
            table.Cell().Row(20).Column(1).ColumnSpan(3).Text($"Tragseiltyp:   {ParameterDictionary["var_Tragseiltyp"].Value}").FontSize(fontSizeXXS);
            table.Cell().Row(21).Column(1).Text("Anzahl der Tragseile:").FontSize(fontSizeXXS);
            table.Cell().Row(21).Column(2).AlignRight().Text(ParameterDictionary["var_NumberOfRopes"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(21).Column(3).AlignLeft().PaddingLeft(2).Text("stk.").FontSize(fontSizeXXS);
            table.Cell().Row(22).Column(1).Text("Mindestbruchlast Tragseil:").FontSize(fontSizeXXS);
            table.Cell().Row(22).Column(2).AlignRight().Text(ParameterDictionary["var_Mindestbruchlast"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(22).Column(3).AlignLeft().PaddingLeft(2).Text("N").FontSize(fontSizeXXS);
            table.Cell().Row(23).Column(1).Text("Sicherheitsfaktor Tragseil:").FontSize(fontSizeXXS);
            table.Cell().Row(23).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_RopeSafety"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(24).Column(1).ColumnSpan(3).Text("Belastungen / Kräfte").FontSize(fontSizeXS).FontColor(borderColor).Bold();
            table.Cell().Row(25).Column(1).Text("Belastung unter FK Schiene:").FontSize(fontSizeXXS);
            table.Cell().Row(25).Column(2).AlignRight().Text(ParameterDictionary["var_Belastung_pro_Schiene_auf_Grundelement"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(25).Column(3).AlignLeft().PaddingLeft(2).Text("N").FontSize(fontSizeXXS);
            table.Cell().Row(26).Column(1).Text("Belastung unter GGW Schiene:").FontSize(fontSizeXXS);
            table.Cell().Row(26).Column(2).AlignRight().Text(ParameterDictionary["var_Belastung_pro_Schiene_auf_Grundelement_GGW"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(26).Column(3).AlignLeft().PaddingLeft(2).Text("N").FontSize(fontSizeXXS);
            table.Cell().Row(27).Column(1).Text("Belastung unter FK Puffer:").FontSize(fontSizeXXS);
            table.Cell().Row(27).Column(2).AlignRight().Text(ParameterDictionary["var_Belastung_Pufferstuetze_auf_Grundelement"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(27).Column(3).AlignLeft().PaddingLeft(2).Text("N").FontSize(fontSizeXXS);
            table.Cell().Row(28).Column(1).Text("Belastung unter GGW Puffer:").FontSize(fontSizeXXS);
            table.Cell().Row(28).Column(2).AlignRight().Text(ParameterDictionary["var_Belastung_Pufferstuetze_auf_Grundelement_GGW"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(28).Column(3).AlignLeft().PaddingLeft(2).Text("N").FontSize(fontSizeXXS);
            table.Cell().Row(29).Column(1).Text("Kraft Fx auf FK-Schiene:").FontSize(fontSizeXXS);
            table.Cell().Row(29).Column(2).AlignRight().Text(ParameterDictionary["var_FxF"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(29).Column(3).AlignLeft().PaddingLeft(2).Text("N").FontSize(fontSizeXXS);
            table.Cell().Row(30).Column(1).Text("Kraft Fy auf FK-Schiene:").FontSize(fontSizeXXS);
            table.Cell().Row(30).Column(2).AlignRight().Text(ParameterDictionary["var_FyF"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(30).Column(3).AlignLeft().PaddingLeft(2).Text("N").FontSize(fontSizeXXS);
            table.Cell().Row(31).Column(1).Text("Kraft Fx auf GGW-Schiene:").FontSize(fontSizeXXS);
            table.Cell().Row(31).Column(2).AlignRight().Text(ParameterDictionary["var_FxFA_GGW"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(31).Column(3).AlignLeft().PaddingLeft(2).Text("N").FontSize(fontSizeXXS);
            table.Cell().Row(32).Column(1).Text("Kraft Fy auf GGW-Schiene:").FontSize(fontSizeXXS);
            table.Cell().Row(32).Column(2).AlignRight().Text(ParameterDictionary["var_FyFA_GGW"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(32).Column(3).AlignLeft().PaddingLeft(2).Text("N").FontSize(fontSizeXXS);
            table.Cell().Row(33).Column(1).ColumnSpan(3).Text("Anschlusswerte").FontSize(fontSizeXS).FontColor(borderColor).Bold();
            table.Cell().Row(34).Column(1).Text("Nennstrom Netz (aufgerundet):").FontSize(fontSizeXXS);
            table.Cell().Row(34).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_Nennstrom"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(34).Column(3).AlignLeft().PaddingLeft(2).Text("A").FontSize(fontSizeXXS);
            table.Cell().Row(35).Column(1).Text("Maximale Netzstromaufnahme (Netzstrom x 1,8):").FontSize(fontSizeXXS);
            table.Cell().Row(35).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_AnlaufstromMax"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(35).Column(3).AlignLeft().PaddingLeft(2).Text("A").FontSize(fontSizeXXS);
            table.Cell().Row(36).Column(1).Text("Leistung Netz (aufgerundet  + 2kW):").FontSize(fontSizeXXS);
            table.Cell().Row(36).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_Leistung"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(36).Column(3).AlignLeft().PaddingLeft(2).Text("KW").FontSize(fontSizeXXS);
            table.Cell().Row(37).Column(1).Text("Spannung Netz:").FontSize(fontSizeXXS);
            table.Cell().Row(37).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_Stromart"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(37).Column(3).AlignLeft().PaddingLeft(2).Text("V").FontSize(fontSizeXXS);
            table.Cell().Row(38).Column(1).Text("Leistung Motor:").FontSize(fontSizeXXS);
            table.Cell().Row(38).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_Motor_Pr"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(38).Column(3).AlignLeft().PaddingLeft(2).Text("KW").FontSize(fontSizeXXS);
            table.Cell().Row(39).Column(1).Text("Bemessungsspannung Motor:").FontSize(fontSizeXXS);
            table.Cell().Row(39).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_Motor_Ur"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(39).Column(3).AlignLeft().PaddingLeft(2).Text("V").FontSize(fontSizeXXS);
            table.Cell().Row(40).Column(1).Text("Bemessungsstrom Motor:").FontSize(fontSizeXXS);
            table.Cell().Row(40).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_Motor_Ir"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(40).Column(3).AlignLeft().PaddingLeft(2).Text("A").FontSize(fontSizeXXS);
            table.Cell().Row(41).Column(1).Text("Motorstrom bei Maximalmoment:").FontSize(fontSizeXXS);
            table.Cell().Row(41).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_Motor_FE_"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(41).Column(3).AlignLeft().PaddingLeft(2).Text("A").FontSize(fontSizeXXS);
            table.Cell().Row(42).Column(1).Text("Verlustleistung:").FontSize(fontSizeXXS);
            table.Cell().Row(42).Column(2).ColumnSpan(2).AlignRight().PaddingRight(2).Text(ParameterDictionary["var_ZA_IMP_VerlustLeistung"].Value).FontSize(fontSizeXXS);
            var maxFuse = _calculationsModuleService.GetMaxFuse(ParameterDictionary!["var_ZA_IMP_Regler_Typ"].Value);
            table.Cell().Row(43).Column(1).Text("Max. Schmelzsicherung:").FontSize(fontSizeXXS);
            table.Cell().Row(43).Column(2).AlignRight().Text(maxFuse.ToString()).FontSize(fontSizeXXS);
            table.Cell().Row(43).Column(3).AlignLeft().PaddingLeft(2).Text("A").FontSize(fontSizeXXS);
        });
    }

    void DriveDetailData(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(80);
                columns.RelativeColumn(1);
            });

            table.Cell().Row(1).Column(1).ColumnSpan(2).Text("Daten ZAlift Auslegung").FontSize(fontSizeXS).FontColor(borderColor).Bold();
            table.Cell().Row(2).Column(1).Text("Antrieb:").FontSize(fontSizeXXS);
            table.Cell().Row(2).Column(2).AlignRight().Text(ParameterDictionary["var_Antrieb"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(3).Column(1).Text("Aufhängung:").FontSize(fontSizeXXS);
            table.Cell().Row(3).Column(2).AlignRight().Text($"{ParameterDictionary["var_AufhaengungsartRope"].Value} : 1").FontSize(fontSizeXXS);
            table.Cell().Row(4).Column(1).Text("Umschlingungswinkel:").FontSize(fontSizeXXS);
            table.Cell().Row(4).Column(2).AlignRight().Text($"{ParameterDictionary["var_Umschlingungswinkel"].Value} grad").FontSize(fontSizeXXS);
            table.Cell().Row(5).Column(1).Text("Treibscheibendurchmesser:").FontSize(fontSizeXXS);
            table.Cell().Row(5).Column(2).AlignRight().Text($"{ParameterDictionary["var_Treibscheibendurchmesser"].Value} mm (RA :{ParameterDictionary["var_ZA_IMP_Treibscheibe_RIA"].Value} mm)").FontSize(fontSizeXXS);
            table.Cell().Row(6).Column(1).Text("Umlenkrollendurchmesser:").FontSize(fontSizeXXS);
            table.Cell().Row(6).Column(2).AlignRight().Text($"{ParameterDictionary["var_Umlenkrollendurchmesser"].Value} mm").FontSize(fontSizeXXS);
            table.Cell().Row(7).Column(1).Text("Anzahl Umlenkrollen:").FontSize(fontSizeXXS);
            table.Cell().Row(7).Column(2).AlignRight().Text($"{ParameterDictionary["var_AnzahlUmlenkrollen"].Value} Stk").FontSize(fontSizeXXS);
            table.Cell().Row(8).Column(1).Text("Anzahl Umlenkrollen FK:").FontSize(fontSizeXXS);
            table.Cell().Row(8).Column(2).AlignRight().Text($"{ParameterDictionary["var_AnzahlUmlenkrollenFk"].Value} Stk").FontSize(fontSizeXXS);
            table.Cell().Row(9).Column(1).Text("Anzahl Umlenkrollen GGW:").FontSize(fontSizeXXS);
            table.Cell().Row(9).Column(2).AlignRight().Text($"{ParameterDictionary["var_AnzahlUmlenkrollenGgw"].Value} Stk").FontSize(fontSizeXXS);
            table.Cell().Row(10).Column(1).Text("Frequenzumrichter:").FontSize(fontSizeXXS);
            table.Cell().Row(10).Column(2).AlignRight().Text(ParameterDictionary["var_ZA_IMP_Regler_Typ"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(11).Column(1).Text("Gebertyp des Motors:").FontSize(fontSizeXXS);
            table.Cell().Row(11).Column(2).AlignRight().Text(ParameterDictionary["var_MotorGeber"].Value).FontSize(fontSizeXXS);
            table.Cell().Row(12).Column(1).ColumnSpan(2).PaddingTop(5).Text("UCM - Daten").FontSize(fontSizeXS).FontColor(borderColor).Bold();
            table.Cell().Row(13).Column(1).Text("Erkennungsweg in mm:").FontSize(fontSizeXXS);
            table.Cell().Row(13).Column(2).AlignRight().Text($"{ParameterDictionary["var_Erkennungsweg"].Value} mm").FontSize(fontSizeXXS);
            table.Cell().Row(14).Column(1).Text("Totzeit gesamt:").FontSize(fontSizeXXS);
            table.Cell().Row(14).Column(2).AlignRight().Text($"{ParameterDictionary["var_Totzeit"].Value} ms").FontSize(fontSizeXXS);
            table.Cell().Row(15).Column(1).Text("V Geschwindigkeitsdetektor:").FontSize(fontSizeXXS);
            table.Cell().Row(15).Column(2).AlignRight().Text($"{ParameterDictionary["var_Vdetektor"].Value} m/s").FontSize(fontSizeXXS);
        });
    }
}
