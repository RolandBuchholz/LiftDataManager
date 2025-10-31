using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer.SafetyComponentRecordModels;
using LiftDataManager.Core.Models.PdfDocuments;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Diagnostics;

namespace LiftDataManager.Core.Services;
/// <summary>
/// A <see langword="class"/> that implements the <see cref="IPdfService"/> <see langword="interface"/> using Quest Pdf APIs.
/// </summary>
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

        if (!path.EndsWith("AutoDeskTransfer.xml"))
        {
            _logger.LogError(61101, "Path is not a AutoDeskTransfer.xml");
            return false;
        }

        if (!readOnlyAllowed)
        {
            FileInfo AutoDeskTransferInfo = new(path);
            if (AutoDeskTransferInfo.IsReadOnly)
            {
                _logger.LogError(61201, "Saving failed AutoDeskTransferXml is readonly");
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public bool MakeSinglePdfDocument(string pdfModel, ObservableDictionary<string, Parameter> ParameterDictionary, string? path, bool showPdf, bool lowPrintColor, bool lowHighlightColor)
    {
        IDocument? document = pdfModel switch
        {
            "KabinenLüftungViewModel" => new KabinenLüftungDocument(ParameterDictionary, _calculationsModuleService, lowPrintColor),
            "NutzlastberechnungViewModel" => new NutzlastberechnungDocument(ParameterDictionary, _calculationsModuleService, lowPrintColor),
            "KabinengewichtViewModel" => new KabinengewichtDocument(ParameterDictionary, _calculationsModuleService, lowPrintColor),
            "EinreichunterlagenViewModel" => new EinreichunterlagenDocument(ParameterDictionary, _calculationsModuleService, lowPrintColor),
            "Spezifikation" => new SpezifikationDocument(ParameterDictionary, _calculationsModuleService, lowPrintColor, lowHighlightColor),
            _ => null,
        };

        //document?.ShowInPreviewerAsync();

        if (document is not null)
        {
            if (showPdf)
            {
                var filePath = Path.Combine(Path.GetTempPath(), $@"{Guid.NewGuid()}.pdf");
                if (!GeneratePdfDocument(document, filePath, pdfModel))
                {
                    return false;
                }
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
                {
                    return false;
                }

                var fileName = document.GetMetadata().Title;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return false;
                }
                string? filePath;
                if (document.GetType() == typeof(SpezifikationDocument))
                {
                    filePath = Path.Combine(Path.GetDirectoryName(path)!, $@"{fileName}.pdf");
                }
                else
                {
                    filePath = Path.Combine(Path.GetDirectoryName(path)!, "Berechnungen", "PDF", $@"{fileName}.pdf");
                }

                if (Path.Exists(Path.GetDirectoryName(filePath)))
                {
                    if (File.Exists(filePath))
                    {
                        FileInfo PdfFileInfo = new(filePath);
                        if (PdfFileInfo.IsReadOnly)
                        {
                            PdfFileInfo.IsReadOnly = false;
                        }
                        if (PdfFileInfo.IsLocked())
                        {
                            var acrobatProcesses = Process.GetProcessesByName("Acrobat");
                            if (acrobatProcesses.Length == 1)
                            {
                                acrobatProcesses[0].CloseMainWindow();
                            }
                            else if (acrobatProcesses.Length > 1)
                            {
                                foreach (var process in acrobatProcesses)
                                {
                                    process.Kill();
                                }
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                }
                if (!GeneratePdfDocument(document, filePath, pdfModel))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <inheritdoc/>
    public bool GenerateSafetyComponentsReport(ObservableDictionary<string, Parameter> parameterDictionary, LiftCommission? liftCommission) 
    {
        if (liftCommission == null)
        { 
            return false; 
        }
        IDocument? document = new SafetyComponentsDocument(parameterDictionary, liftCommission);

        if (document is not null)
        {
            var filePath = Path.Combine(Path.GetTempPath(), $@"{Guid.NewGuid()}.pdf");
            if (!GeneratePdfDocument(document, filePath, "SafetyComponentsReport"))
            {
                return false;
            }
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                }
            };
            process.Start();
        }
        return true;
    }

    /// <inheritdoc/>
    public bool MakeDefaultSetofPdfDocuments(ObservableDictionary<string, Parameter> ParameterDictionary, string? path)
    {
        string[] setOfPdfs =
        [
            "KabinenLüftungViewModel",
            "NutzlastberechnungViewModel",
            "KabinengewichtViewModel",
            "Spezifikation"
        ];

        foreach (var pdf in setOfPdfs)
        {
            MakeSinglePdfDocument(pdf, ParameterDictionary, path, false, false, false);
        }

        return true;
    }

    private bool GeneratePdfDocument(IDocument document, string? filePath, string? pdfModel)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        try
        {
            document.GeneratePdf(filePath);
            _logger.LogInformation(60000, "Create Pdf : {document}", document.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(61301, "Create Pdf failed : {document}", document.ToString());
            var errorDocument = new ErrorPdfDocument(ex, pdfModel);
            errorDocument.GeneratePdf(filePath);
        }
        return true;
    }
}
