﻿using Humanizer;
using Microsoft.Extensions.Logging;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class ImportLiftDataDialogViewModel : ObservableObject
{
    private const string defaultImage = "/Images/TonerSaveOFF.png";
    private const string pdfImage = "/Images/PdfTransparent.png";
    private readonly IParameterDataService _parameterDataService;
    private readonly IVaultDataService _vaultDataService;
    public string? FullPathXml { get; set; }
    public SpezifikationTyp? CurrentSpezifikationTyp { get; set; }

    private readonly ILogger<ImportLiftDataDialogViewModel> _logger;
    public ImportLiftDataDialogViewModel(IParameterDataService parameterDataService, IVaultDataService vaultDataService, ILogger<ImportLiftDataDialogViewModel> logger)
    {
        _parameterDataService = parameterDataService;
        _vaultDataService = vaultDataService;
        _logger = logger;
    }

    [RelayCommand]
    public async Task ImportLiftDataDialogLoadedAsync(ImportLiftDataDialog sender)
    {
        FullPathXml = sender.FullPathXml;
        CurrentSpezifikationTyp = sender.CurrentSpezifikationTyp;
        SpezifikationName = sender.SpezifikationName;
        await Task.CompletedTask;
    }

    [ObservableProperty]
    private string? spezifikationName;

    [ObservableProperty]
    private string? dataImportDescription = "Daten aus einer vorhandenen Spezifikation importieren.";

    [ObservableProperty]
    private string? dataImportDescriptionImage = defaultImage;

    [ObservableProperty]
    private string? importSpezifikationName;
    partial void OnImportSpezifikationNameChanged(string? value)
    {
        UpdateDataImportStatusText();
    }

    [ObservableProperty]
    private string? dataImportStatusText = "Keine Daten für Import vorhanden";

    [ObservableProperty]
    private InfoBarSeverity dataImportStatus = InfoBarSeverity.Informational;

    [ObservableProperty]
    private bool showImportCarFrames;

    [ObservableProperty]
    private string? selectedImportCarFrame;

    [ObservableProperty]
    private SpezifikationTyp? importSpezifikationTyp = SpezifikationTyp.Order;
    partial void OnImportSpezifikationTypChanged(SpezifikationTyp? value)
    {
        ImportSpezifikationName = string.Empty;

        if (value is null)
        {
            return;
        }

        value
            .When(SpezifikationTyp.Order).Then(() =>
            {
                DataImportDescription = $"Daten aus einer vorhandenen {value}sspezifikation importieren.";
                DataImportDescriptionImage = defaultImage;
            })
            .When(SpezifikationTyp.Offer).Then(() =>
            {
                DataImportDescription = $"Daten aus einer vorhandenen {value}sspezifikation importieren.";
                DataImportDescriptionImage = defaultImage;
            })
            .When(SpezifikationTyp.Planning).Then(() =>
            {
                DataImportDescription = $"Daten aus einer vorhandenen {value}sspezifikation importieren.";
                DataImportDescriptionImage = defaultImage;
            })
            .When(SpezifikationTyp.Request).Then(() =>
            {
                DataImportDescription = "Daten aus einem Anfrage Formular importieren.";
                DataImportDescriptionImage = pdfImage;
            })
            .Default(() =>
            {
                DataImportDescription = "Daten aus einer vorhandenen Spezifikation importieren.";
                DataImportDescriptionImage = defaultImage;
            });
        _logger.LogInformation(60132, "ImportSpezifikationTyp changed {Typ}", value);
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartDataImportCommand))]
    private bool canImportSpeziData;
    partial void OnCanImportSpeziDataChanged(bool value)
    {
        UpdateDataImportStatusText();
    }

    [ObservableProperty]
    private bool liftDataReadyForImport;
    partial void OnLiftDataReadyForImportChanged(bool value)
    {
        DataImportStatusText = value ? "Daten zur Übernahme bereit" : DataImportStatusText;
    }

    [RelayCommand(CanExecute = nameof(CanImportSpeziData))]
    private async Task StartDataImportAsync(ImportLiftDataDialog sender)
    {
        if (string.IsNullOrWhiteSpace(SpezifikationName) ||
            string.IsNullOrWhiteSpace(ImportSpezifikationName))
        {
            return;
        }

        if (string.Equals(SpezifikationName, ImportSpezifikationName))
        {
            DataImportStatus = InfoBarSeverity.Error;
            DataImportStatusText = "Datenimport kann nicht in sich selbst importiert werden!";
            return;
        }
        DataImportStatus = InfoBarSeverity.Informational;
        DataImportStatusText = "Datenimport gestartet";

        var ignoreImportParameters = new List<string>
        {
            "var_Index",
            "var_FabrikNummer",
            "var_AuftragsNummer",
            "var_Kennwort",
            "var_ErstelltVon",
            "var_FabriknummerBestand",
            "var_FreigabeErfolgtAm",
            "var_Demontage",
            "var_FertigstellungAm",
            "var_GeaendertAm",
            "var_GeaendertVon"
        };

        List<TransferData>? importParameter = [];
        if (ImportSpezifikationTyp != SpezifikationTyp.Request)
        {
            ignoreImportParameters.Add("var_ErstelltAm");
            ignoreImportParameters.Add("var_AuslieferungAm");
            if (ImportSpezifikationTyp is null)
            {
                return;
            }
            var downloadResult = await _vaultDataService.GetAutoDeskTransferAsync(ImportSpezifikationName, ImportSpezifikationTyp, true);

            var downloadInfo = downloadResult.Item2;

            if (downloadInfo is null)
            {
                DataImportStatus = InfoBarSeverity.Error;
                DataImportStatusText = "Datenimport fehlgeschlagen";
                return;
            }
            if (downloadInfo.ExitState is not ExitCodeEnum.NoError)
            {
                DataImportStatus = InfoBarSeverity.Warning;
                DataImportStatusText = downloadInfo.ExitState.Humanize();
                return;
            }
            if (downloadInfo.FullFileName is null)
            {
                DataImportStatus = InfoBarSeverity.Error;
                DataImportStatusText = "Datenimport fehlgeschlagen Dateipfad der Importdatei konnte nicht gefunden werden";
                return;
            }

            var data = await _parameterDataService.LoadParameterAsync(downloadInfo.FullFileName);
            foreach (var parameter in data)
            {
                if (ignoreImportParameters.Contains(parameter.Name))
                {
                    continue;
                }
                importParameter.Add(parameter);
            }
        }
        else
        {
            var importParameterPdf = await _parameterDataService.LoadPdfOfferAsync(ImportSpezifikationName);

            if (!importParameterPdf.Any())
            {
                DataImportStatus = InfoBarSeverity.Warning;
                DataImportStatusText = $"Die ausgewählte PDF-Datei enthält keine Daten für den Import.\n" +
                                       $"{ImportSpezifikationName}";
                return;
            }

            var isMultiCarframe = importParameterPdf.Any(x => x.Name == "var_Firma_TG2");
            ShowImportCarFrames = isMultiCarframe;
            if (!isMultiCarframe)
            {
                SelectedImportCarFrame = null;
            }

            if (isMultiCarframe && SelectedImportCarFrame is null)
            {
                DataImportStatus = InfoBarSeverity.Warning;
                DataImportStatusText = "Mehrere Bausatztypen für DatenImport vorhanden.\n" +
                                       "Wählen sie den gewünschten Bausatztyp aus!";
                return;
            }

            var carTypPrefix = SelectedImportCarFrame switch
            {
                "Tiger TG2" => "_TG2",
                "BR1 1:1" => "_BR1",
                "BR2 2:1" => "_BR2",
                "Jupiter BT1" => "_BT1",
                "Jupiter BT2" => "_BT2",
                "Seil-Rucksack BRR" => "_BRR",
                "Seil-Zentral ZZE-S" => "_ZZE_S",
                _ => "_EZE_SR"
            };

            var cleanImport = new List<TransferData>();

            foreach (var item in importParameterPdf)
            {
                if (item.Name.EndsWith(carTypPrefix) || item.Name == "var_CFPOption")
                {
                    item.Name = item.Name.Replace(carTypPrefix, "");
                    cleanImport.Add(item);
                }
            }

            var carTyp = carTypPrefix switch
            {
                "_TG2" => "TG2-15 MK2",
                "_BR1" => "BR1-15 MK2",
                "_BR2" => "BR2-15 MK2",
                "_BT1" => "BT1-40",
                "_BT2" => "BT2-40",
                "_BRR" => "BRR-15 MK2",
                "_ZZE_S" => "ZZE-S1600",
                _ => "EZE-SR3200 SAO"
            };

            cleanImport.Add(new TransferData("var_Bausatz", carTyp, string.Empty, false));
            cleanImport.Add(new TransferData("var_Fahrkorbtyp", "Fremdkabine", string.Empty, false));
            importParameter = cleanImport;

            CopyPdfOffer(ImportSpezifikationName);
        }

        if (importParameter is null)
        {
            return;
        }
        sender.ImportSpezifikationName = ImportSpezifikationName;
        sender.ImportPamameter = importParameter;
        LiftDataReadyForImport = true;
        DataImportStatus = InfoBarSeverity.Success;
    }

    [RelayCommand]
    private async Task PickFilePathAsync()
    {
        var filePicker = App.MainWindow.CreateOpenFilePicker();
        filePicker.ViewMode = PickerViewMode.Thumbnail;
        filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        filePicker.FileTypeFilter.Add(".pdf");
        StorageFile file = await filePicker.PickSingleFileAsync();
        ImportSpezifikationName = (file is not null) ? file.Path : string.Empty;
    }

    private void CopyPdfOffer(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            return;
        }

        var currentFileName = Path.GetFileName(fullPath);
        var newFileName = currentFileName.StartsWith($"{SpezifikationName}-") ? currentFileName : $"{SpezifikationName}-{currentFileName}";
        var newFullPath = Path.Combine(Path.GetDirectoryName(fullPath)!, "SV", newFileName);

        if (string.Equals(fullPath, newFullPath, StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(newFullPath) && Path.IsPathFullyQualified(newFullPath))
        {
            if (File.Exists(newFullPath))
            {
                FileInfo pdfOfferFileInfo = new(newFullPath);
                if (pdfOfferFileInfo.IsReadOnly)
                {
                    pdfOfferFileInfo.IsReadOnly = false;
                }
            }
            File.Copy(fullPath, newFullPath, true);
        }
    }

    private void UpdateDataImportStatusText()
    {
        DataImportStatusText = CanImportSpeziData ? $"{ImportSpezifikationName} kann importiert werden." : "Keine Daten für Import vorhanden";
    }
}