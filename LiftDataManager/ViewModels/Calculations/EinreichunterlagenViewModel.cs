using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Text.Json;

namespace LiftDataManager.ViewModels;

public partial class EinreichunterlagenViewModel : DataViewModelBase, INavigationAwareEx, IRecipient<PropertyChangedMessage<string>>, IRecipient<RefreshModelStateMessage>
{
    private readonly ParameterContext _parametercontext;
    private readonly ICalculationsModule _calculationsModuleService;
    private readonly IPdfService _pdfService;
    private static readonly JsonSerializerOptions writeOptions = new()
    {
        WriteIndented = true
    };

    public EinreichunterlagenViewModel(IParameterDataService parameterDataService, IDialogService dialogService, IInfoCenterService infoCenterService,
                                       ISettingService settingService, ILogger<DataViewModelBase> baseLogger, ICalculationsModule calculationsModuleService, ParameterContext parametercontext, IPdfService pdfService) :
         base(parameterDataService, dialogService, infoCenterService, settingService, baseLogger)
    {
        _parametercontext = parametercontext;
        _calculationsModuleService = calculationsModuleService;
        _pdfService = pdfService;
        LiftDocumentation ??= new();
        LiftSafetyComponents ??= [];
        UCMPComponents ??= [];
        LiftDescriptionInfos ??= [];
    }

    public override void Receive(PropertyChangedMessage<string> message)
    {
        if (message is null ||
           !(message.Sender.GetType() == typeof(Parameter)))
        {
            return;
        }
        if (message.PropertyName == "var_SeilschlossTyp" ||
            message.PropertyName == "var_Stromanschluss" ||
            message.PropertyName == "var_Netzform")
        {
            CheckLiftDescriptionInfos();
        }
        SetInfoSidebarPanelText(message);
        SetModelStateAsync().SafeFireAndForget(onException: ex => LogTaskException(ex));
    }

    [RelayCommand]
    public void CreatePdf()
    {
        if (ParameterDictionary is not null)
        {
            _pdfService.MakeSinglePdfDocument(nameof(EinreichunterlagenViewModel), ParameterDictionary, FullPathXml, true, false, false);
        }
    }

    [RelayCommand(CanExecute = nameof(CanCreateTechnicalLiftDocumentation))]
    public async Task CreateTechnicalLiftDocumentationAsync()
    {
        await _dialogService.MessageDialogAsync("Einreichunterlagen","Aktuell ist es noch nicht möglich automatische Einreichunterlagen zu erstellen.");
    }

    private void LiftDocumentation_OnTechnicalLiftDocumentationChanged(object? sender, TechnicalLiftDocumentationEventArgs e)
    {
        UpdateLiftDocumentation();
        CheckLiftDescriptionInfos();
        if (e.PropertyName == "ProtectedSpaceTypPit" || e.PropertyName == "ProtectedSpaceTypHead")
        {
            UpdateProtectedSpaceTyp();
        }
    }

    public TechnicalLiftDocumentation LiftDocumentation { get; set; }
    public List<LiftSafetyComponent> LiftSafetyComponents { get; set; }
    public List<LiftSafetyComponent> UCMPComponents { get; set; }
    public ObservableRangeCollection<LiftDescriptionInfoEntry> LiftDescriptionInfos { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateTechnicalLiftDocumentationCommand))]
    public partial bool CanCreateTechnicalLiftDocumentation { get; set; }
    public string DriveTyp => _calculationsModuleService.GetDriveTyp(ParameterDictionary["var_Getriebe"].Value, LiftParameterHelper.GetLiftParameterValue<int>(ParameterDictionary, "var_AufhaengungsartRope"));
    public double CWTBalancePercent => LiftParameterHelper.GetLiftParameterValue<double>(ParameterDictionary, "var_GGWNutzlastausgleich") * 100;
    public string DriveControl => _calculationsModuleService.GetDriveControl(ParameterDictionary["var_Aggregat"].Value);
    public string DrivePosition => _calculationsModuleService.GetDrivePosition(ParameterDictionary["var_Maschinenraum"].Value);
    public string SameShaftWith => string.IsNullOrWhiteSpace(ParameterDictionary["var_GemeinsamerSchachtMit"].Value) ?
                                                    "Vorstehender Aufzug ist mit keinem weiteren Aufzug im gleichen Schacht errichtet." :
                                                    $"Vorstehender Aufzug ist mit dem Aufzug - den Aufzügen - Fabrik.-Nr.: {ParameterDictionary["var_GemeinsamerSchachtMit"].Value} im gleichem Schacht errichtet.";
    public string TuevExamination => ParameterDictionary["var_Aufzugstyp"].Value != "Umbau" ? "gemäß Anhang VIII (Modul G)" : "Prüfung nach § 15 BetrSichV";
    public int CarDoorCount => _calculationsModuleService.GetNumberOfCardoors(ParameterDictionary);
    public string DoorTyp => !string.IsNullOrWhiteSpace(ParameterDictionary["var_Tuertyp"].Value) ? ParameterDictionary["var_Tuertyp"].Value!.Replace(" -", "") : string.Empty;
    public string DateTimeNow => DateTime.Now.ToShortDateString();

    public string ViewingOpening => LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_TuerSchauOeffnungKT") || LiftParameterHelper.GetLiftParameterValue<bool>(ParameterDictionary, "var_TuerSchauOeffnungST") ?
                                    "Schauöffnungen (aus 10 mm dickem VSG - Glas) in den Fahr/Schachttüren vorhanden." :
                                    "Schauöffnungen in den Fahr/Schachttüren - nicht vorhanden.";
    public string LiftType => _calculationsModuleService.GetLiftTyp(ParameterDictionary["var_Aufzugstyp"].Value);
    public bool IsRopeLift => _calculationsModuleService.IsRopeLift(ParameterDictionary["var_Bausatz"].DropDownListValue);
    public string? DriveName => IsRopeLift ? ParameterDictionary["var_Antrieb"]?.Value : $"{ParameterDictionary["var_Antrieb"]?.Value} - {ParameterDictionary["var_Hydraulikventil"]?.Value} - {ParameterDictionary["var_Pumpenbezeichnung"]?.Value}";
    public string CWTRailName => IsRopeLift ? "Gegengewicht:" : "Jochschiene:";
    public string CarGuideRailSurface => _calculationsModuleService.GetGuideRailSurface(ParameterDictionary["var_FuehrungsschieneFahrkorb"].DropDownListValue, ParameterDictionary["var_Fuehrungsart"].DropDownListValue);
    public string CWTGuideRailSurface => _calculationsModuleService.GetGuideRailSurface(ParameterDictionary["var_FuehrungsschieneGegengewicht"].DropDownListValue, ParameterDictionary["var_Fuehrungsart_GGW"].DropDownListValue);

    [ObservableProperty]
    public partial string ProtectedSpaceTypPitImage { get; set; } = "/Images/NoImage.png";

    [ObservableProperty]
    public partial string ProtectedSpaceTypPitDescription { get; set; } = "Kein Schutzraum gewählt";

    [ObservableProperty]
    public partial string ProtectedSpaceTypHeadImage { get; set; } = "/Images/NoImage.png";

    [ObservableProperty]
    public partial string ProtectedSpaceTypHeadDescription { get; set; } = "Kein Schutzraum gewählt";
    private void CheckLiftDescriptionInfos()
    {
        LiftDescriptionInfos.Clear();
        List<LiftDescriptionInfoEntry> liftDescriptionInfos = [];
        if (LiftDocumentation.ManufactureYear == 0)
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Warning, "Allgemeine Angaben", "Kein Baujahr gewählt"));
        }
        if (LiftDocumentation.MonthOfConstruction == 0 || LiftDocumentation.YearOfConstruction == 0)
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Warning, "Allgemeine Angaben", "Kein Errichtungsdatum gewählt"));
        }
        if(LiftDocumentation.ManufactureYear !=0 &&
           LiftDocumentation.MonthOfConstruction != 0 &&
           LiftDocumentation.YearOfConstruction != 0)
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Success, "Allgemeine Angaben", "Baujahr/Errichtungsdatum vollständig"));
        }

        if (LiftDocumentation.ProtectedSpaceTypHead == 0 ||
            LiftDocumentation.ProtectedSpaceTypPit == 0 ||
            LiftDocumentation.SafetySpaceHead == 0 ||
            LiftDocumentation.ProtectedSpaceTypPit == 0)
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Warning, "Fahrschacht - Fahrschachtzugänge", "Schutzraumangaben(Typ / Höhen) unvollständig"));
        }
        else
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Success, "Fahrschacht - Fahrschachtzugänge", "Schutzraumangaben vollständig"));
        }

        if (!string.IsNullOrWhiteSpace(ParameterDictionary["var_Tragseiltyp"].Value) &&
            ParameterDictionary["var_SeilschlossTyp"].DropDownListValue is null)
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Warning, "Tragmittel", "Seilschlosstyp nicht angegeben"));
        }
        else
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Success, "Tragmittel", "Seilangaben vollständig"));
        }

        if (_calculationsModuleService.IsOverspeedGovernorWeightRequired(ParameterDictionary["var_Geschwindigkeitsbegrenzer"].DropDownListValue) &&
            ParameterDictionary["var_SpanngewichtTyp"].DropDownListValue is null)
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Warning, "Geschwindigkeitsbegenzer", "Geschwindigkeitsbegenzer/Spanngewicht unvollständig"));
        }
        else
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Success, "Geschwindigkeitsbegenzer", "Geschwindigkeitsbegenzer vollständig"));
        }

        if (ParameterDictionary["var_Stromanschluss"].DropDownListValue is null ||
            ParameterDictionary["var_Netzform"].DropDownListValue is null)
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Warning, "Elektrische Ausrüstung", "Angaben Spannung/Netzform des Anschlussnetzes unvollständig"));
        }
        else
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Success, "Elektrische Ausrüstung", "Angaben Spannung/Netzform des Anschlussnetzes vollständig"));
        }

        if (ParameterDictionary["var_UCMP_DetektierendesElement"].DropDownListValue is null ||
            ParameterDictionary["var_UCMP_AusloesendesElement"].DropDownListValue is null ||
            ParameterDictionary["var_UCMP_BremsendesElement"].DropDownListValue is null)
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Warning, "UCMP-Daten", "UCMP-Daten (Detektierendes, Auslösendes und Bremsendes Element) unvollständig"));
        }
        else
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Success, "UCMP-Daten", "UCMP-Daten vollständig"));
        }

        if (!LiftDocumentation.Layoutdrawing &&
            !LiftDocumentation.RiskAssessment &&
            !LiftDocumentation.Calculations &&
            !LiftDocumentation.CircuitDiagrams &&
            !LiftDocumentation.TestingInstructions &&
            !LiftDocumentation.FactoryCertificate &&
            !LiftDocumentation.OperatingInstructions &&
            !LiftDocumentation.MaintenanceInstructions &&
            !LiftDocumentation.OtherDocuments)
        {
            liftDescriptionInfos.Add(new LiftDescriptionInfoEntry(InfoBarSeverity.Informational, "Anlagen", "Keine Anlagen gewählt"));
        }

        LiftDescriptionInfos.AddRange<LiftDescriptionInfoEntry>(liftDescriptionInfos.OrderByDescending(x => x.State));
        CanCreateTechnicalLiftDocumentation = !LiftDescriptionInfos.Any(x => x.State == InfoBarSeverity.Warning);
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
        LiftDocumentation.OnTechnicalLiftDocumentationChanged += LiftDocumentation_OnTechnicalLiftDocumentationChanged;
        UpdateProtectedSpaceTyp();
    }

    private void UpdateLiftDocumentation()
    {
        ParameterDictionary["var_Einreichunterlagen"].AutoUpdateParameterValue(JsonSerializer.Serialize(LiftDocumentation, writeOptions).Replace("\r\n", "\n"));
    }

    private void UpdateProtectedSpaceTyp()
    {
        ProtectedSpaceTypPitImage = TechnicalLiftDocumentation.GetProtectedSpaceTypImage(LiftDocumentation.ProtectedSpaceTypPit);
        ProtectedSpaceTypPitDescription = TechnicalLiftDocumentation.GetProtectedSpaceTypDescription(LiftDocumentation.ProtectedSpaceTypPit);
        ProtectedSpaceTypHeadImage = TechnicalLiftDocumentation.GetProtectedSpaceTypImage(LiftDocumentation.ProtectedSpaceTypHead);
        ProtectedSpaceTypHeadDescription = TechnicalLiftDocumentation.GetProtectedSpaceTypDescription(LiftDocumentation.ProtectedSpaceTypHead);
    }

    private void SetListofSafetyComponents()
    {
        LiftSafetyComponents = _calculationsModuleService.GetLiftSafetyComponents(ParameterDictionary);
        UCMPComponents = _calculationsModuleService.GetUCMPComponents(ParameterDictionary);
    }

    public void OnNavigatedTo(object parameter)
    {
        NavigatedToBaseActions();
        if (CurrentSpeziProperties is not null)
        {
            SetLiftDocumentation();
            SetListofSafetyComponents();
            CheckLiftDescriptionInfos();
        }
    }

    public void OnNavigatedFrom()
    {
        LiftDocumentation.OnTechnicalLiftDocumentationChanged -= LiftDocumentation_OnTechnicalLiftDocumentationChanged;
        NavigatedFromBaseActions();
    }
}
