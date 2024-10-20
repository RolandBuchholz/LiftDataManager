using Cogs.Collections;
using Microsoft.Extensions.Logging;

namespace LiftDataManager.ViewModels.Dialogs;

public partial class AppClosingDialogViewModel : ObservableRecipient
{
    private readonly IParameterDataService _parameterDataService;
    private readonly IVaultDataService _vaultDataService;
    private readonly ILogger<AppClosingDialogViewModel> _logger;
    private readonly IPdfService _pdfService;

    public AppClosingDialogViewModel(IParameterDataService parameterDataService, IVaultDataService vaultDataService, IPdfService pdfService, ILogger<AppClosingDialogViewModel> logger)
    {
        _parameterDataService = parameterDataService;
        _vaultDataService = vaultDataService;
        _pdfService = pdfService;
        _logger = logger;
    }

    public bool IgnoreSaveWarning { get; set; }

    public bool Dirty { get; set; }

    private CurrentSpeziProperties? _currentSpeziProperties;

    private ObservableDictionary<string, Parameter> _parameterDictionary;

    [ObservableProperty]
    private string? specificationStatus;

    [ObservableProperty]
    private string? description;

    [RelayCommand]
    public async Task AppClosingDialogLoadedAsync(AppClosingDialog sender)
    {
        IgnoreSaveWarning = sender.IgnoreSaveWarning;
        _currentSpeziProperties = Messenger.Send<SpeziPropertiesRequestMessage>();
        if (_currentSpeziProperties is null)
        {
            return;
        }
        if (!_currentSpeziProperties.CheckOut)
        {
            sender.IgnoreSaveWarning = true;
            sender.Hide();
            return;
        }
        _parameterDictionary = _parameterDataService.GetParameterDictionary();
        if (_parameterDictionary != null)
        {
            Dirty = _parameterDictionary.Values.Any(p => p.IsDirty);
        }
        if (Dirty)
        {
            SpecificationStatus = "Warnung nicht gespeicherte Parameter gefunden!";
            Description = """
                           Es sind nicht gespeicherte Parameter vorhanden.

                           Wollen Sie diese speichern?
                           """;
            sender.PrimaryButtonText = "Parameter speichern";
            sender.SecondaryButtonText = "Parameter verwerfen";
        }
        else
        {
            SpecificationStatus = "Warnung Autodesktranfer.xml wurde noch nicht hochgeladen!";
            Description = """
                           Der Auftrag wurde noch nicht ins Vault hochgeladen.

                           Wollen Sie den Auftrag ins Vault hochladen und einchecken?
                           """;
            sender.PrimaryButtonText = "Daten einchecken";
            sender.SecondaryButtonText = "Daten nicht einchecken";
        }
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task PrimaryButtonClicked(AppClosingDialog sender)
    {
        if (_currentSpeziProperties is null || _currentSpeziProperties.FullPathXml is null)
        {
            return;
        }
        if (Dirty)
        {
            await _parameterDataService.SaveAllParameterAsync(_currentSpeziProperties.FullPathXml, _currentSpeziProperties.Adminmode);
        }
        else
        {
            var spezifikationName = Path.GetFileName(_currentSpeziProperties.FullPathXml).Replace("-AutoDeskTransfer.xml", "");
            if (string.IsNullOrWhiteSpace(spezifikationName))
            {
                _logger.LogError(61037, "SpezifikationName are null or empty");
                return;
            }
            var pdfcreationResult = _pdfService.MakeDefaultSetofPdfDocuments(_parameterDictionary, _currentSpeziProperties.FullPathXml);
            _logger.LogInformation(60137, "Pdf CreationResult: {pdfcreationResult}", pdfcreationResult);
            await _vaultDataService.SetFileAsync(spezifikationName);
            sender.IgnoreSaveWarning = true;
        }
    }
}