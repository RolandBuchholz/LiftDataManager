﻿using Humanizer;
using LiftDataManager.Core.DataAccessLayer;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class ImportLiftDataDialogViewModel : ObservableObject
{
    private const string defaultImage = "/Images/TonerSaveOFF.png";
    private const string pdfImage = "/Images/PdfTransparent.png";
    private const string mailImage = "/Images/Mail.png";
    private readonly ParameterContext _parametercontext;
    private readonly IParameterDataService _parameterDataService;
    private readonly IVaultDataService _vaultDataService;
    private readonly ISettingService _settingService;
    public string? FullPathXml { get; set; }
    public string? DragAndDropFileType { get; set; }
    public SpezifikationTyp? CurrentSpezifikationTyp { get; set; }

    public ObservableCollection<string> CarFrames { get; set; } = [];

    private readonly ILogger<ImportLiftDataDialogViewModel> _logger;
    public ImportLiftDataDialogViewModel(IParameterDataService parameterDataService, IVaultDataService vaultDataService, ISettingService settingService,
                                         ParameterContext parametercontext, ILogger<ImportLiftDataDialogViewModel> logger)
    {
        _parameterDataService = parameterDataService;
        _vaultDataService = vaultDataService;
        _settingService = settingService;
        _parametercontext = parametercontext;
        _logger = logger;
    }

    [RelayCommand]
    public async Task ImportLiftDataDialogLoadedAsync(ImportLiftDataDialog sender)
    {
        FullPathXml = sender.FullPathXml;
        CurrentSpezifikationTyp = sender.CurrentSpezifikationTyp;
        SpezifikationName = sender.SpezifikationName;
        VaultDisabled = sender.VaultDisabled;
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task DragAndDropDragEnterAsync(DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count > 0)
            {
                if (items[0] is not StorageFile storageFile)
                {
                    return;
                }
                DragAndDropFileType = storageFile.FileType;
            }
        }
    }

    [RelayCommand]
    public async Task DragAndDropDragOverAsync(DragEventArgs e)
    {
        if (DragAndDropFileType is null)
        {
            return;
        }

        if (DragAndDropFileType.Equals(".pdf", StringComparison.CurrentCultureIgnoreCase))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Anfrageformular";
            e.DragUIOverride.SetContentFromBitmapImage(
                new BitmapImage(new Uri("ms-appx:///Images/PdfTransparent.png", UriKind.RelativeOrAbsolute)));
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = false;
        }
        else if (DragAndDropFileType.Equals(".msg", StringComparison.CurrentCultureIgnoreCase))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Mailanfrage";
            e.DragUIOverride.SetContentFromBitmapImage(
                new BitmapImage(new Uri("ms-appx:///Images/E-mail.png", UriKind.RelativeOrAbsolute)));
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = false;
        }
        else
        {
            e.AcceptedOperation = DataPackageOperation.None;
            e.DragUIOverride.Caption = "Nicht unterstütztes Dateiformat";
            e.DragUIOverride.SetContentFromBitmapImage(
                new BitmapImage(new Uri("ms-appx:///Assets/Fluent/cancel.png", UriKind.RelativeOrAbsolute)));
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
        }
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task DragAndDropDropedAsync(DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count > 0)
            {
                if (items[0] is not StorageFile storageFile)
                {
                    return;
                }
                var fileFullPath = storageFile.Path;
                if (string.IsNullOrWhiteSpace(fileFullPath))
                {
                    StorageFolder temp = ApplicationData.Current.TemporaryFolder;
                    var tempStorageFile = await storageFile.CopyAsync(temp, storageFile.Name, NameCollisionOption.ReplaceExisting);
                    fileFullPath = tempStorageFile.Path;
                }
                ImportSpezifikationName = fileFullPath;
            }
        }
    }

    [ObservableProperty]
    public partial string? SpezifikationName { get; set; }

    [ObservableProperty]
    public partial bool VaultDisabled { get; set; }

    [ObservableProperty]
    public partial string? DataImportDescription { get; set; } = "Daten aus einer vorhandenen Spezifikation importieren.";

    [ObservableProperty]
    public partial string DragAndDropDescription { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? DataImportDescriptionImage { get; set; } = defaultImage;

    [ObservableProperty]
    public partial string? ImportSpezifikationName { get; set; }
    partial void OnImportSpezifikationNameChanged(string? value)
    {
        UpdateDataImportStatusText();
    }

    [ObservableProperty]
    public partial string? DataImportStatusText { get; set; } = "Keine Daten für Import vorhanden";

    [ObservableProperty]
    public partial InfoBarSeverity DataImportStatus { get; set; } = InfoBarSeverity.Informational;

    [ObservableProperty]
    public partial bool ShowImportCarFrames { get; set; }

    [ObservableProperty]
    public partial bool ShowImportOptions { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowDragAndDropPanel { get; set; }

    [ObservableProperty]
    public partial string? SelectedImportCarFrame { get; set; }

    [ObservableProperty]
    public partial bool ImportDriveData { get; set; }

    [ObservableProperty]
    public partial bool ImportCFPData { get; set; }

    [ObservableProperty]
    public partial bool ImportCFPDataBaseOverrides { get; set; }

    [ObservableProperty]
    public partial SpezifikationTyp? ImportSpezifikationTyp { get; set; } = SpezifikationTyp.Order;
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
                DragAndDropDescription = string.Empty;
                ShowDragAndDropPanel = false;
                ShowImportOptions = true;
            })
            .When(SpezifikationTyp.Offer).Then(() =>
            {
                DataImportDescription = $"Daten aus einer vorhandenen {value}sspezifikation importieren.";
                DataImportDescriptionImage = defaultImage;
                DragAndDropDescription = string.Empty;
                ShowDragAndDropPanel = false;
                ShowImportOptions = true;
            })
            .When(SpezifikationTyp.Planning).Then(() =>
            {
                DataImportDescription = $"Daten aus einer vorhandenen {value}sspezifikation importieren.";
                DataImportDescriptionImage = defaultImage;
                DragAndDropDescription = string.Empty;
                ShowDragAndDropPanel = false;
                ShowImportOptions = true;
            })
            .When(SpezifikationTyp.Request).Then(() =>
            {
                DataImportDescription = "Daten aus einem Anfrage Formular importieren.";
                DataImportDescriptionImage = pdfImage;
                DragAndDropDescription = "Anfrage Formular hierher ziehen oder Anfrage Formular zum hochladen auswählen.";
                ShowDragAndDropPanel = true;
                ShowImportOptions = false;
            })
            .When(SpezifikationTyp.MailRequest).Then(() =>
            {
                DataImportDescription = "Daten aus einer E-Mail Anfrage importieren.";
                DataImportDescriptionImage = mailImage;
                DragAndDropDescription = "Mail aus Outlook hierher ziehen oder gespeicherte Mail zum hochladen auswählen.";
                ShowDragAndDropPanel = true;
                ShowImportOptions = false;
            })
            .Default(() =>
            {
                DataImportDescription = "Daten aus einer vorhandenen Spezifikation importieren.";
                DataImportDescriptionImage = defaultImage;
                DragAndDropDescription = string.Empty;
                ShowDragAndDropPanel = false;
                ShowImportOptions = true;
            });
        _logger.LogInformation(60132, "ImportSpezifikationTyp changed {Typ}", value);
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartDataImportCommand))]
    public partial bool CanImportSpeziData { get; set; }
    partial void OnCanImportSpeziDataChanged(bool value)
    {
        UpdateDataImportStatusText();
    }

    [ObservableProperty]
    public partial bool LiftDataReadyForImport { get; set; }
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
            "var_GeaendertVon",
            "var_ImportiertVon",
            "var_CFPdefiniert"
        };

        if (!ImportDriveData)
        {
            {
                var cfpParameter = _parametercontext.ParameterDtos?.Where(x => x.ParameterCategoryId == 15);
                if (cfpParameter is not null)
                {
                    foreach (var parameter in cfpParameter)
                    {
                        ignoreImportParameters.Add(parameter.Name);
                    }
                }
            }
        }
        if (!ImportCFPData)
        {
            var cfpParameter = _parametercontext.ParameterDtos?.Where(x => x.ParameterCategoryId == 13 || x.ParameterCategoryId == 16);
            if (cfpParameter is not null)
            {
                foreach (var parameter in cfpParameter)
                {
                    ignoreImportParameters.Add(parameter.Name);
                }
            }
        }
        if (!ImportCFPDataBaseOverrides)
        {
            ignoreImportParameters.Add("var_dbChanges");
        }
        List<TransferData>? importParameter = [];
        if (ImportSpezifikationTyp == SpezifikationTyp.MailRequest)
        {
            var importMailOffer = await _parameterDataService.LoadMailOfferAsync(ImportSpezifikationName);
            if (!importMailOffer.Any())
            {
                DataImportStatus = InfoBarSeverity.Warning;
                DataImportStatusText = $"Die ausgewählte Outlook-Datei enthält keine Daten für den Import.\n" +
                                       $"{ImportSpezifikationName}";
                return;
            }
            importParameter.AddRange(importMailOffer);
            if (!string.IsNullOrWhiteSpace(ImportSpezifikationName))
            {
                CopyOffer(ImportSpezifikationName);
            }
        }
        else if (ImportSpezifikationTyp == SpezifikationTyp.Request)
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
            if (SelectedImportCarFrame is null)
            {
                CarFrames.Clear();
                if (isMultiCarframe)
                {
                    CarFrames.Add("Tiger TG2");
                    CarFrames.Add("BR1 1:1");
                    CarFrames.Add("BR2 2:1");
                    CarFrames.Add("Jupiter BT1");
                    CarFrames.Add("Jupiter BT2");
                    CarFrames.Add("Seil-Rucksack BRR");
                    CarFrames.Add("Seil-Zentral ZZE-S");
                }
                else
                {
                    CarFrames.Add("Seil-Rucksack EZE-SR");
                    CarFrames.Add("Seil-Zentral ZZE-S");
                }
            }
            ShowImportCarFrames = SelectedImportCarFrame is null;

            if (SelectedImportCarFrame is null)
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
                "Seil-Rucksack EZE-SR" => "_EZE_SR",
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
            if (!string.IsNullOrWhiteSpace(ImportSpezifikationName))
            {
                CopyOffer(ImportSpezifikationName);
            }
        }
        else
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
        if (importParameter is null)
        {
            return;
        }

        if (ImportSpezifikationTyp == SpezifikationTyp.MailRequest || ImportSpezifikationTyp == SpezifikationTyp.Request)
        {
            sender.ImportSpezifikationName = Path.GetFileName(ImportSpezifikationName);
        }
        else
        {
            sender.ImportSpezifikationName = ImportSpezifikationName;
        }

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
        filePicker.FileTypeFilter.Add(SetFileTypeFilter());
        StorageFile file = await filePicker.PickSingleFileAsync();
        ImportSpezifikationName = (file is not null) ? file.Path : string.Empty;
    }

    private string SetFileTypeFilter()
    {
        if (_settingService.VaultDisabled)
        {
            return ".xml";
        }
        if (ImportSpezifikationTyp == SpezifikationTyp.Request)
        {
            return ".pdf";
        }
        return ".pdf";
    }

    private void CopyOffer(string importSpezifikationName)
    {
        if (string.IsNullOrWhiteSpace(FullPathXml) ||
            !File.Exists(importSpezifikationName))
        {
            return;
        }

        var currentFileName = Path.GetFileName(importSpezifikationName);
        var newFileName = currentFileName.StartsWith($"{SpezifikationName}-") ? currentFileName : $"{SpezifikationName}-{currentFileName}";
        var newFullPath = Path.Combine(Path.GetDirectoryName(FullPathXml)!, "SV", newFileName);

        if (string.Equals(FullPathXml, newFullPath, StringComparison.CurrentCultureIgnoreCase))
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
            try
            {
                File.Copy(importSpezifikationName, newFullPath, true);
            }
            catch
            {
                _logger.LogError(60132, "Copy Offer: {name} failed", importSpezifikationName);
            }
        }
    }

    private void UpdateDataImportStatusText()
    {
        string? importName;
        if (ImportSpezifikationTyp == SpezifikationTyp.MailRequest || ImportSpezifikationTyp == SpezifikationTyp.Request)
        {
            importName = Path.GetFileName(ImportSpezifikationName);
        }
        else if (_settingService.VaultDisabled)
        {
            if(!string.IsNullOrWhiteSpace(ImportSpezifikationName) && ImportSpezifikationName.EndsWith(".xml"))
            {
                importName = Path.GetFileName(ImportSpezifikationName);
                ImportSpezifikationName = importName.Replace("-AutoDeskTransfer.xml", "");
            }
            else
            {
                importName = ImportSpezifikationName;
            }
        }
        else
        {
            importName = ImportSpezifikationName;
        }
        DataImportStatusText = CanImportSpeziData ? $"{importName} kann importiert werden." : "Keine Daten für Import vorhanden";
    }
}