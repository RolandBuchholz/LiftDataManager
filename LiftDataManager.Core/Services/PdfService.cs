using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using Microsoft.Extensions.Logging;
using PDFTests.Services.DocumentGeneration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
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

    public bool MakeSinglePdfDocument(string pdfModel, ObservableDictionary<string, Parameter> paramterDictionary, string? path, bool showPdf)
    {
        IDocument? document = pdfModel switch
        {
            "KabinenLüftungViewModel" => new KabinenLüftungDocument(paramterDictionary, _calculationsModuleService),
            "NutzlastberechnungViewModel" => new NutzlastberechnungDocument(paramterDictionary, _calculationsModuleService),
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

                var filePath = Path.Combine(Path.GetDirectoryName(path!)!, "Berechnungen","PDF", $@"{fileName}.pdf");

                if (Path.Exists(Path.GetDirectoryName(filePath)))
                {
                    if (File.Exists(filePath))
                    {
                        FileInfo PdfFileInfo = new(filePath);
                        if (PdfFileInfo.IsReadOnly)
                        {
                            PdfFileInfo.IsReadOnly = false;
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
            "NutzlastberechnungViewModel"
        };

        foreach (var pdf in setOfPdfs)
        {
            MakeSinglePdfDocument(pdf, paramterDictionary, path, false);
        }

        return true;
    }
}
