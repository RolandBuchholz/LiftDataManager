using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Helpers;
using Microsoft.Extensions.Logging;
using PDFTests.Services.DocumentGeneration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Diagnostics;

namespace LiftDataManager.Core.Services;

public class PdfService : IPdfService
{
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly ILogger<PdfService> _logger;

    public PdfService(ICalculationsModule calculationsModuleService, ILogger<PdfService> logger)
    {
        _calculationsModuleService = calculationsModuleService;
        _logger = logger;
    }

    private bool ValidatePath(string? path, bool readOnlyAllowed)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _logger.LogError(61001, "Path is null or whiteSpace");
            return false;
        }

        if (!path.StartsWith("C:\\Work"))
        {
            _logger.LogError(61001, "Path is not in workspace", path);
            return false;
        }

        if (!path.EndsWith("AutoDeskTransfer.xml"))
        {
            _logger.LogError(61001, "Path is not a AutoDeskTransfer.xml", path);
            return false;
        }

        if (!readOnlyAllowed)
        {
            FileInfo AutoDeskTransferInfo = new(path);
            if (AutoDeskTransferInfo.IsReadOnly)
            {
                _logger.LogError(61101, "Saving failed AutoDeskTransferXml is readonly");
                return false;
            }
        }

        return true;
    }

    public bool MakeSinglePdfDocument(string pdfModel, ObservableDictionary<string, Parameter> paramterDictionary, string? path, bool showPdf, bool lowPrintColor, bool lowHighlightColor)
    {
        IDocument? document = pdfModel switch
        {
            "KabinenLüftungViewModel" => new KabinenLüftungDocument(paramterDictionary, _calculationsModuleService, lowPrintColor),
            "NutzlastberechnungViewModel" => new NutzlastberechnungDocument(paramterDictionary, _calculationsModuleService, lowPrintColor),
            "KabinengewichtViewModel" => new KabinengewichtDocument(paramterDictionary, _calculationsModuleService, lowPrintColor),
            "Spezifikation" => new SpezifikationDocument(paramterDictionary, _calculationsModuleService, lowPrintColor, lowHighlightColor),
            _ => null,
        };

        //document?.ShowInPreviewerAsync();

        if (document is not null)
        {
            if (showPdf)
            {
                var filePath = Path.Combine(Path.GetTempPath(), $@"{Guid.NewGuid()}.pdf");
                document.GeneratePdf(filePath);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(filePath)
                    {
                        UseShellExecute = true
                    }
                };

                process.Start();
            }
            else
            {
                if (!ValidatePath(path, false))
                    return false;

                var fileName = document.GetMetadata().Title;
                string? filePath;
                if (document.GetType() == typeof(SpezifikationDocument))
                {
                    filePath = Path.Combine(Path.GetDirectoryName(path!)!, $@"{fileName}.pdf");
                }
                else
                {
                    filePath = Path.Combine(Path.GetDirectoryName(path!)!, "Berechnungen", "PDF", $@"{fileName}.pdf");
                }

                if (Path.Exists(Path.GetDirectoryName(filePath)))
                {
                    if (File.Exists(filePath))
                    {
                        FileInfo PdfFileInfo = new(filePath);
                        if (PdfFileInfo.IsReadOnly)
                            PdfFileInfo.IsReadOnly = false;
                        if (PdfFileInfo.IsLocked())
                        {
                            var acrobatProcesses = Process.GetProcessesByName("Acrobat");
                            if (acrobatProcesses.Any())
                            {
                                foreach (var process in acrobatProcesses)
                                {
                                    process.CloseMainWindow();
                                }
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath!)!);
                }

                document.GeneratePdf(filePath);
            }
        }
        return true;
    }

    public bool MakeDefaultSetofPdfDocuments(ObservableDictionary<string, Parameter> paramterDictionary, string? path)
    {
        string[] setOfPdfs = new string[]
        {
            "KabinenLüftungViewModel",
            "NutzlastberechnungViewModel",
            "KabinengewichtViewModel",
            "Spezifikation"
        };

        foreach (var pdf in setOfPdfs)
        {
            MakeSinglePdfDocument(pdf, paramterDictionary, path, false, false, false);
        }

        return true;
    }
}
