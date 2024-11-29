using Cogs.Collections;
using HtmlAgilityPack;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.Models.ComponentModels;
using Microsoft.Extensions.Logging;
using MsgReader.Outlook;
using PdfSharp;
using PdfSharp.Fonts;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using PdfSharp.Snippets.Font;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;

namespace LiftDataManager.Core.Services;

/// <summary>
/// A <see langword="class"/> that implements the <see cref="IParameterDataService"/> <see langword="interface"/> using LiftDataManager parameter APIs.
/// </summary>
public partial class ParameterDataService : IParameterDataService
{
    private readonly IValidationParameterDataService _validationParameterDataService;
    private readonly ParameterContext _parametercontext;
    private readonly ILogger<ParameterDataService> _logger;
    private ObservableDictionary<string, Parameter>? _parameterDictionary;
    private PeriodicTimer? _autoSaveTimer;
    private readonly string user;
    private bool _saveAllParameterIsRunninng;

    public ParameterDataService(IValidationParameterDataService validationParameterDataService,
                                 ParameterContext parametercontext, ILogger<ParameterDataService> logger)
    {
        _validationParameterDataService = validationParameterDataService;
        _parametercontext = parametercontext;
        _logger = logger;
        user = string.IsNullOrWhiteSpace(System.Security.Principal.WindowsIdentity.GetCurrent().Name) ? "no user detected" : System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("PPS\\", "");
    }
    /// <inheritdoc/>
    public async Task InitializeParameterDataServicerAsync(ObservableDictionary<string, Parameter> parameterDictionary) 
    {
        _parameterDictionary = parameterDictionary;
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task ResetAsync()
    {
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool CanConnectDataBase()
    {
        if (!_parametercontext.Database.CanConnect())
        {
            return false;
        }
        if (_parametercontext.Database.GetConnectionString() is not null)
        {
            return _parametercontext.Database.GetConnectionString()!.Contains("LiftDataParameter");
        }
        return false;
    }

    /// <inheritdoc/>
    public string GetCurrentUser()
    {
        return user;
    }

    /// <inheritdoc/>
    public ObservableDictionary<string, Parameter> GetParameterDictionary() 
    { 
        if (_parameterDictionary is not null)
        {
            return _parameterDictionary;
        }
        else
        {
            _logger.LogCritical(60100, "Parameter Dictionary is null");
            return [];
        }
    }

    /// <inheritdoc/>
    public LiftHistoryEntry GenerateLiftHistoryEntry(Parameter parameter)
    {
        return new LiftHistoryEntry(parameter.Name,
            parameter.DisplayName,
            parameter.Value is not null ? parameter.Value : string.Empty,
            parameter.IsAutoUpdated ? "LDM-AutoUpdated" : user,
            parameter.Comment is not null ? parameter.Comment : string.Empty);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Parameter>> InitializeParametereFromDbAsync()
    {
        List<Parameter> parameterList = [];

        var parameterDtos = _parametercontext.ParameterDtos!
                                             .AsNoTracking()
                                             .ToList();

        var dropdownValues = _parametercontext.DropdownValues!
                              .AsNoTracking()
                              .ToList()
                              .GroupBy(x => x.Base);

        if (parameterDtos is not null)
        {
            foreach (var par in parameterDtos)
            {
                var newParameter = new Parameter(par.Value!, par.ParameterTypeCodeId, par.ParameterTypId, par.Comment!, _validationParameterDataService)
                {
                    Name = par.Name,
                    DisplayName = par.DisplayName,
                    ParameterCategory = (ParameterCategoryValue)par.ParameterCategoryId,
                    DefaultUserEditable = par.DefaultUserEditable,
                    IsKey = par.IsKey,
                    IsDirty = false
                };

                if (par.DropdownList is not null)
                {
                    var dropdownList = dropdownValues.FirstOrDefault(x => string.Equals(x.Key, par.DropdownList))?.OrderBy(o => o.OrderSelection);
                    if (dropdownList is not null)
                    {
                        newParameter.DropDownList.Add(new SelectionValue());
                        foreach (var item in dropdownList)
                        {
                            newParameter.DropDownList.Add(new SelectionValue(item.Id, item.Name, item.DisplayName)
                            {
                                IsFavorite = item.IsFavorite,
                                OrderSelection = item.OrderSelection,
                                SchindlerCertified = item.SchindlerCertified,
                            });
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(par.Value))
                    {
                        newParameter.DropDownListValue = LiftParameterHelper.GetDropDownListValue(newParameter.DropDownList, par.Value);
                    }
                }
                parameterList.Add(newParameter);
            }
        }

        await Task.CompletedTask;
        _logger.LogInformation(60100, "Parameter from database initialized");
        return parameterList;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TransferData>> LoadParameterAsync(string path)
    {
        XElement doc = XElement.Load(path);
        var transferDataList = (from para in doc.Elements("parameters").Elements("ParamWithValue")
                                select new TransferData(
                                                     para.Element("name")!.GetAs<string>()!,
                                                     para.Element("value")!.GetAs<string>()!,
                                                     para.Element("comment")!.GetAs<string>()!,
                                                     para.Element("isKey")!.GetAs<bool>()))
                                  .ToList();
        await Task.CompletedTask;
        _logger.LogInformation(60101, "Get TranferData form {path}", path);
        return transferDataList;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TransferData>> LoadPdfOfferAsync(string path)
    {
        var transferDataList = new List<TransferData>();
        if (Capabilities.Build.IsCoreBuild)
            GlobalFontSettings.FontResolver ??= new FailsafeFontResolver();

        using (var pdfDocument = PdfReader.Open(path, PdfDocumentOpenMode.Import))
        {
            var cfPOptions = new CarFrameProgramOptions();
            PdfAcroForm pdfDocumentAcroForm;

            try
            {
                pdfDocumentAcroForm = pdfDocument.AcroForm;
            }
            catch (Exception)
            {
                _logger.LogWarning(60202, "Pdfdocument has no AcroForm Parameter: {path} failed", path);
                return transferDataList;
            }

            var fields = pdfDocumentAcroForm.Fields;
            foreach (var pdfName in fields.Names)
            {
                var field = fields[pdfName];
                if (field is null)
                    continue;
                KeyValuePair<string, string> parameter = field.Value is null ? new KeyValuePair<string, string>(string.Empty, string.Empty) : ValidatePdfValue(field);
                if (!parameter.Key.StartsWith("var_"))
                {
                    if (cfPOptions.IsCFPOption(parameter.Key))
                    {
                        var option = ValidatePdfValue(field);
                        if (double.TryParse(option.Value, out double optionValue))
                        {
                            cfPOptions.SetOption(parameter.Key, (int)optionValue);
                        }
                    }
                    continue;
                }
                transferDataList.Add(new TransferData(parameter.Key, parameter.Value, string.Empty, false));
            }
            var options = new JsonSerializerOptions { WriteIndented = true };
            var cfPOptionsJson = JsonSerializer.Serialize(cfPOptions, options).Replace("\r\n", "\n");
            if (!string.IsNullOrWhiteSpace(cfPOptionsJson))
            {
                transferDataList.Add(new TransferData("var_CFPOption", cfPOptionsJson, string.Empty, false));
            }
        }
        await Task.CompletedTask;
        _logger.LogInformation(60101, "Parameter from Pdf: {path} loaded", path);
        return transferDataList;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TransferData>> LoadMailOfferAsync(string path)
    {
        var transferDataList = new List<TransferData>();
        if (string.IsNullOrWhiteSpace(path))
        {
            return transferDataList;
        }
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        string? htmlBody;
        using (var msg = new Storage.Message(path))
        {
            var from = msg.Sender.Email;
            _logger.LogInformation(60100, "Mailofferimport sender: {from}", from);
            var subject = msg.Subject;
            _logger.LogInformation(60100, "Mailofferimport subject: {subject}", subject);
            htmlBody = msg.BodyHtml;
        }

        var mailOfferHtml = new HtmlDocument();
        mailOfferHtml.LoadHtml(htmlBody);
        var htmlNodes = mailOfferHtml.DocumentNode.SelectNodes("//tr");
        double carFloorHeight = 86d;
        double carRoofHeight = 76d;
        Dictionary<string, (int, string)> importdataDictionary = new()
            {
                {"var_AnPersonZ1", (1, "Name;Vorname;Firma") },
                {"var_AnPersonZ2", (0, "Straße")},
                {"var_AnPersonZ3", (1, "Postleitzahl;Ort")},
                {"var_AnPersonPhone", (0, "Telefon")},
                {"var_AnPersonMail", (0, "Mail")},
                {"var_Kennwort" , (0, "Projekt")},
                {"var_ZUGANSSTELLEN_B" , (2, "Weitere Zugangsstellen")},
                {"var_ZUGANSSTELLEN_C" , (2, "Weitere Zugangsstellen")},
                {"var_ZUGANSSTELLEN_D" , (2, "Weitere Zugangsstellen")},
                {"var_Bausatzlage" , (0, "Bausatzlage")},
                {"var_Q" , (0, "Tragkraft")},
                {"var_Fahrkorbtyp" , (3, "Kabinengewicht")},
                {"var_KabinengewichtCAD" , (0, "Kabinengewicht")},
                {"var_FH" , (4, "Förderhöhe")},
                {"var_v" , (0, "Geschwindigkeit")},
                {"var_KBI" , (0, "Kabinenbreite innen")},
                {"var_KTI" , (0, "Kabinentiefe innen")},
                {"var_KHI" , (0, "Kabinenhöhe innen")},
                {"var_SB" , (0, "Schachtbreite")},
                {"var_ST" , (0, "Schachttiefe")},
                {"var_SG" , (0, "Schachtgrube")},
                {"var_SK" , (0, "Schachtkopf")},
                {"var_Aufzugstyp" , (5, "Antriebsart")},
                {"var_Maschinenraum" , (6, "Maschinenraum")},
                {"var_Bausatz" , (7, "Fangrahmenart")},
                {"var_Bodentyp" , (8, "Kabinenbodendicke")},
                {"var_KU" , (8, "Kabinenbodendicke")},
                {"var_KD" , (9, "Kabinendeckendicke")},
                {"var_KBA" , (10, "Kabinenbreite aussen")},
                {"var_KTA" , (10, "Kabinentiefe aussen")},
                {"var_KHA" , (10, "Kabinenhöhe aussen")},
                {"var_B1" , (0, "Erster Bügel")},
                {"var_B2" , (0, "Größter Schienenbügelabstand")},
                {"var_Kommentare" , (0, "Anmerkungen")}
            };
        foreach (var dataSet in importdataDictionary)
        {
            var value = string.Empty;
            switch (dataSet.Value.Item1)
            {
                case 1:
                    // concatenate strings
                    var concatenatedString = string.Empty;
                    var parameters = dataSet.Value.Item2.Split(';');
                    if (parameters.Length > 0)
                    {
                        concatenatedString = string.Join(' ', parameters.Select(x => GetValueFromHtmlNode(htmlNodes.FirstOrDefault(y => y.InnerText.StartsWith(x)))));
                    }
                    value = concatenatedString;
                    break;
                case 2:
                    // set entrances
                    var entrances = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                    value = entrances.Contains(dataSet.Key[^1..]).ToString();
                    break;
                case 3:
                    // set carTyp
                    value = string.IsNullOrWhiteSpace(GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2))))
                            ? string.Empty
                            : "Fremdkabine";
                    break;
                case 4:
                    // convert mm to m
                    var travelmeter = "0";
                    var travelmm = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                    if (!string.IsNullOrWhiteSpace(travelmm))
                    {
                        if (double.TryParse(travelmm, out double doubleValue))
                        {
                            travelmeter = (doubleValue / 1000).ToString();
                        }
                    }
                    value = travelmeter;
                    break;
                case 5:
                    // set liftTyp
                    var selectedLiftTyp = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                    value = selectedLiftTyp switch
                    {
                        "Hydraulikaufzug" => "Personen- / Lasten Hydraulik-Aufzug",
                        "Seilaufzug" => "Personen Seil-Aufzug",
                        _ => string.Empty,
                    };
                    break;
                case 6:
                    // set machineroom
                    var machineroom = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                    value = machineroom switch
                    {
                        "mit Maschinenraum oben über" => "oben über",
                        "mit Maschinenraum oben neben" => "oben neben",
                        "mit Maschinenraum unten neben" => "unten neben",
                        "ohne Maschinenraum" => "ohne",
                        _ => string.Empty,
                    };
                    break;
                case 7:
                    // set carFrameTyp
                    var carFrameTyp = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                    var drivetyp = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Antriebsart")));
                    if (!string.IsNullOrWhiteSpace(drivetyp))
                    {
                        value = carFrameTyp switch
                        {
                            "Rucksackausführung" => drivetyp.StartsWith("H") ? "Sonderbausatz Hydr. Rucksack 2:1" : "Sonderbausatz Seil Rucksack MRL",
                            "Zentral / Tandem" => drivetyp.StartsWith("H") ? "Sonderbausatz Hydr. Tandem 2:1" : "Sonderbausatz Seil Zentralrahmen",
                            _ => string.Empty,
                        };
                    }
                    break;
                case 8:
                    // set carFloorTyp
                    switch (dataSet.Key)
                    {
                        case "var_Bodentyp":
                            value = "extern";
                            break;
                        case "var_KU":
                            var floorHeightString = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                            if (!string.IsNullOrWhiteSpace(floorHeightString))
                            {
                                if (double.TryParse(floorHeightString, out double doubleValue))
                                {
                                    carFloorHeight = doubleValue;
                                }
                            }
                            value = carFloorHeight.ToString();
                            break;
                        default:
                            break;
                    }
                    break;
                case 9:
                    //set carroof
                    var roofHeightString = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                    if (!string.IsNullOrWhiteSpace(roofHeightString))
                    {
                        if (double.TryParse(roofHeightString, out double doubleValue))
                        {
                            carRoofHeight = doubleValue;
                        }
                    }
                    value = carRoofHeight.ToString();
                    break;
                case 10:
                    //set carWalloutside
                    var outsideCar = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                    if (string.IsNullOrWhiteSpace(outsideCar))
                    {
                        switch (dataSet.Key)
                        {
                            case "var_KBA" or "var_KTA":
                                var carInside = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Key == "var_KBA"
                                                                                                     ? "Kabinenbreite innen"
                                                                                                     : "Kabinentiefe innen")));
                                if (!string.IsNullOrWhiteSpace(carInside))
                                {
                                    if (double.TryParse(carInside, out double doubleValue))
                                    {
                                        value = (doubleValue + 50d).ToString();
                                    }
                                }
                                break;
                            case "var_KHA":
                                var carHeightOutside = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                                if (string.IsNullOrWhiteSpace(carHeightOutside))
                                {
                                    var carHeightInside = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith("Kabinenhöhe innen")));
                                    if (double.TryParse(carHeightInside, out double doubleValue))
                                    {
                                        value = (carFloorHeight + doubleValue + carRoofHeight).ToString();
                                    }
                                }
                                else
                                {
                                    value = carHeightOutside;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        value = outsideCar;
                    }
                    break;
                default:
                    // return standard value
                    value = GetValueFromHtmlNode(htmlNodes.FirstOrDefault(x => x.InnerText.StartsWith(dataSet.Value.Item2)));
                    break;
            }
            transferDataList.Add(new TransferData(dataSet.Key,
                                                  string.IsNullOrWhiteSpace(value) ? string.Empty : value,
                                                  string.Empty,
                                                  false));
        }

        await Task.CompletedTask;
        return transferDataList;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<LiftHistoryEntry>> LoadLiftHistoryEntryAsync(string path, bool includeCategory = false)
    {
        var historyEntrys = new List<LiftHistoryEntry>();
        Dictionary<string, int> parameterCategory = [];

        if (!ValidatePath(path, true))
        {
            return historyEntrys;
        }
        var historyPath = path.Replace("AutoDeskTransfer.xml", "LiftHistory.json");
        if (!Path.Exists(historyPath))
        {
            return historyEntrys;
        }
        var entrys = File.ReadAllLines(historyPath);

        if (includeCategory)
        {
            var parDto = _parametercontext.ParameterDtos;
            if (parDto is not null)
            {
                foreach (var item in parDto)
                {
                    parameterCategory.Add(item.Name, item.ParameterCategoryId);
                }
            }
        }

        foreach (var entry in entrys)
        {
            if (string.IsNullOrWhiteSpace(entry))
                continue;
            LiftHistoryEntry? lifthistoryentry;
            try
            {
                lifthistoryentry = JsonSerializer.Deserialize<LiftHistoryEntry>(entry);
            }
            catch
            {
                _logger.LogError(61101, "Deserialize failed LiftHistory failed");
                continue;
            }
            if (lifthistoryentry is null)
            {
                continue;
            }
            if (!includeCategory)
            {
                historyEntrys.Add(lifthistoryentry);
            }
            else
            {
                lifthistoryentry.Category = GetLiftParameterCategory(parameterCategory, lifthistoryentry.Name);
                historyEntrys.Add(lifthistoryentry);
            }
        }
        await Task.CompletedTask;
        return historyEntrys;
    }

    /// <inheritdoc/>
    public async Task<Tuple<string, string, string?>> SaveParameterAsync(Parameter parameter, string path)
    {
        if (!ValidatePath(path, false))
        {
            _logger.LogError(61001, "{path} Path of AutoDeskTransferXml not vaild", path);
            return new Tuple<string, string, string?>("Error", "Error", "AutoDeskTransferXml Pfad ist nicht gültig");
        }

        XElement doc = XElement.Load(path);
        XElement? xmlparameter =
          (from para in doc.Elements("parameters").Elements("ParamWithValue")
           where para.Element("name")!.Value == parameter.Name
           select para).SingleOrDefault();

        List<LiftHistoryEntry> historyEntrys = [];

        if (xmlparameter is not null)
        {
            AddParameterToXml(parameter, xmlparameter);
            historyEntrys.Add(GenerateLiftHistoryEntry(parameter));
        }
        else
        {
            var updateXmlresult = AddNewParameterToXml(parameter, doc);
            if (updateXmlresult)
            {
                historyEntrys.Add(GenerateLiftHistoryEntry(parameter));
            }
            else
            {
                _logger.LogError(61101, "Saving failed {Name}", parameter.Name);
                return new Tuple<string, string, string?>("Error", "Error", $"Speichern fehlgeschlagen |{parameter.Name}|");
            }   
        }
        await AddParameterListToHistoryAsync(historyEntrys, path, false);
        doc.Save(path);
        return await Task.FromResult(new Tuple<string, string, string?>(parameter.Name, parameter.DisplayName, parameter.Value));
    }

    /// <inheritdoc/>
    public async Task<List<Tuple<string, string, string?>>> SaveAllParameterAsync(string path, bool adminmode)
    {
        var saveResult = new List<Tuple<string, string, string?>>();
        if (string.IsNullOrWhiteSpace(path) || _parameterDictionary is null)
        {
            saveResult.Add(new Tuple<string, string, string?>("Error", "Error", "ParameterDictionary oder Pfad ist null"));
            _saveAllParameterIsRunninng = false;
            return saveResult;
        }
        _saveAllParameterIsRunninng = true;
        if (!ValidatePath(path, false))
        {
            _logger.LogError(61001, "{path} Path of AutoDeskTransferXml not vaild", path);
            saveResult.Add(new Tuple<string, string, string?>("Error", "Error", "AutoDeskTransferXml Pfad ist nicht gültig"));
            _saveAllParameterIsRunninng = false;
            return saveResult;
        }
        XElement doc = XElement.Load(path);
        var unsavedParameter = _parameterDictionary.Values.Where(p => p.IsDirty);

        List<LiftHistoryEntry> historyEntrys = [];

        foreach (var parameter in unsavedParameter)
        {
            if (parameter.DefaultUserEditable || adminmode)
            {
                // Find a specific customer
                XElement? xmlparameter =
                  (from para in doc.Elements("parameters").Elements("ParamWithValue")
                   where para.Element("name")!.Value == parameter.Name
                   select para).SingleOrDefault();

                // Modify some of the node values
                if (xmlparameter is not null)
                {
                    AddParameterToXml(parameter, xmlparameter);
                    parameter.IsDirty = false;
                    historyEntrys.Add(GenerateLiftHistoryEntry(parameter));
                    saveResult.Add(new Tuple<string, string, string?>(parameter.Name, parameter.DisplayName, parameter.Value));
                }
                else
                {
                    var updateXmlresult = AddNewParameterToXml(parameter, doc);
                    if (updateXmlresult)
                    {
                        parameter.IsDirty = false;
                        historyEntrys.Add(GenerateLiftHistoryEntry(parameter));
                        saveResult.Add(new Tuple<string, string, string?>(parameter.Name, parameter.DisplayName, parameter.Value));
                    }
                    else
                    {
                        _logger.LogError(61101, "Saving failed {Name}", parameter.Name);
                        saveResult.Add(new Tuple<string, string, string?>("Error", "Error", $"Speichern fehlgeschlagen |{parameter.Name}|"));
                    }
                }
            }
            else
            {
                _logger.LogWarning(61001, "Saving failed {Name} >Saving is only possible in adminmode<", parameter.Name);
                saveResult.Add(new Tuple<string, string, string?>("Error", "Error", $"Parameter: {parameter.Name} ist scheibgeschützt!\nSpeichern nur im Adminmode möglich!"));
            }
        }
        await AddParameterListToHistoryAsync(historyEntrys, path, false);
        doc.Save(path);
        _saveAllParameterIsRunninng = false;
        await Task.CompletedTask;
        return saveResult;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<InfoCenterEntry>> UpdateParameterDictionary(string path, IEnumerable<TransferData> data, bool updateXml = true)
    {
        var infoCenterEntrys = new List<InfoCenterEntry>();
        if (_parameterDictionary is null)
        {
            return infoCenterEntrys;
        }
        foreach (var item in data)
        {
            if (_parameterDictionary.TryGetValue(item.Name, out Parameter value))
            {
                var updatedParameter = value;
                updatedParameter.DataImport = true;
                if (updatedParameter.ParameterTyp == ParameterTypValue.Boolean)
                {
                    updatedParameter.Value = string.IsNullOrWhiteSpace(item.Value) ? "False" : LiftParameterHelper.FirstCharToUpperAsSpan(item.Value);
                }
                else if (updatedParameter.ParameterTyp == ParameterTypValue.Date)
                {
                    if (string.IsNullOrWhiteSpace(item.Value) || item.Value == "0")
                    {
                        updatedParameter.Value = string.Empty;
                    }
                    else if (item.Value.Contains('.'))
                    {
                        updatedParameter.Value = item.Value;
                    }
                    else
                    {
                        try
                        {
                            updatedParameter.Value = DateTime.FromOADate(Convert.ToDouble(item.Value, CultureInfo.GetCultureInfo("de-DE").NumberFormat)).ToShortDateString();
                            infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterMessage) { Message = $"{updatedParameter.Name} => Exceldatum in String konvertiert" });
                        }
                        catch
                        {
                            updatedParameter.Value = string.Empty;
                        }
                        updatedParameter.IsDirty = true;
                    }
                }
                else
                {
                    updatedParameter.Value = item.Value is not null ? item.Value : string.Empty;
                }

                updatedParameter.Comment = item.Comment;
                updatedParameter.IsKey = item.IsKey;
                if (updatedParameter.ParameterTyp == ParameterTypValue.DropDownList)
                {
                    updatedParameter.DropDownListValue = LiftParameterHelper.GetDropDownListValue(updatedParameter.DropDownList, updatedParameter.Value);
                }
                if (updatedParameter.HasErrors)
                {
                    updatedParameter.HasErrors = false;
                }
                updatedParameter.DataImport = false;
            }
            else
            {
                LogUnsupportedParameter(item.Name);
                infoCenterEntrys.Add(new InfoCenterEntry(InfoCenterEntryState.InfoCenterWarning)
                {
                    Message = $"Parameter {item.Name} wird nicht unterstützt\n" +
                                                                                                             "Überprüfen Sie die AutodeskTransfer.XML Datei"
                });
            }
        }
        if (updateXml)
        {
            FileInfo AutoDeskTransferInfo = new(path);
            if (AutoDeskTransferInfo.IsReadOnly)
            {
                _logger.LogInformation(60136, "Autodesktranfer.xml ist schreibgeschützt");
            }
            else
            {
                var parameterList = _parameterDictionary.Values.ToList();

                XElement? doc = null;
                bool isXmlOutdated = false;
                var dataParameterList = data.Select(x => x.Name).ToList();

                for (int i = 0; i < parameterList.Count; i++)
                {
                    if (!dataParameterList.Contains(parameterList[i].Name!))
                    {
                        isXmlOutdated = true;
                        doc ??= XElement.Load(path);

                        XElement? previousxmlparameter = (from para in doc.Elements("parameters").Elements("ParamWithValue")
                                                          where para.Element("name")!.Value == parameterList[i - 1].Name
                                                          select para).SingleOrDefault();
                        if (previousxmlparameter is not null)
                        {
                            var newXmlTree = new XElement("ParamWithValue",
                                        new XElement("name", parameterList[i].Name),
                                        new XElement("typeCode", parameterList[i].TypeCode.ToString()),
                                        new XElement("value", parameterList[i].Value),
                                        new XElement("comment", parameterList[i].Comment),
                                        new XElement("isKey", parameterList[i].IsKey));

                            previousxmlparameter.AddAfterSelf(new XElement(newXmlTree));
                        }
                    }
                }
                if (isXmlOutdated && doc is not null)
                {
                    doc.Save(path);
                }
                _logger.LogInformation(60136, "Autodesktranfer.xml gespeichert");
            }
        }
        await Task.CompletedTask;
        return infoCenterEntrys;
    }

    /// <inheritdoc/>
    public async Task<bool> AddParameterListToHistoryAsync(List<LiftHistoryEntry> historyEntrys, string path, bool clearHistory)
    {
        var historyPath = path.Replace("AutoDeskTransfer.xml", "LiftHistory.json");

        if (Path.Exists(historyPath))
        {
            FileInfo LiftHistoryInfo = new(historyPath);
            if (LiftHistoryInfo.IsReadOnly)
            {
                _logger.LogError(61101, "Logging failed LiftHistory is readonly");
                return false;
            }
            if (clearHistory)
            {
                LiftHistoryInfo.Delete();
                _logger.LogInformation(60148, "Reset LiftHistory");
            }
        }

        using var writer = new StreamWriter(historyPath, true);
        foreach (var entry in historyEntrys)
        {
            await writer.WriteLineAsync(JsonSerializer.Serialize(entry)).ConfigureAwait(false);
        }
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAutodeskTransferAsync(string path, List<ParameterDto> parameterDtos)
    {
        try
        {
            XElement root = new("ParamWithValueList",
                new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XElement("version", "20080502"),
                new XElement("parameterTypes",
                    new XElement("ParamType", new XElement("typeName", "Inventor"), new XElement("typeCode", "0")),
                    new XElement("ParamType", new XElement("typeName", "String"), new XElement("typeCode", "1")),
                    new XElement("ParamType", new XElement("typeName", "Boolean"), new XElement("typeCode", "2"))),
                new XElement("parameters"),
                new XCData(@"<Element>CopyAllowedTrue</Element>")
                );
            var declaration = new XDeclaration("1.0", "UTF-8", "yes");
            var autodeskTransfer = new XDocument(root)
            {
                Declaration = declaration
            };

            var parameters = autodeskTransfer.Root!.Element("parameters");

            var paramWithValues = new List<XElement>();

            foreach (var parameterDto in parameterDtos)
            {
                var paramWithValue =
                    new XElement("ParamWithValue",
                    new XElement("name", parameterDto.Name),
                    new XElement("typeCode", parameterDto.ParameterTypeCode!.Name),
                    new XElement("value", parameterDto.Value),
                    new XElement("comment", parameterDto.Comment),
                    new XElement("isKey", parameterDto.IsKey.ToString().ToLower()));

                paramWithValues.Add(paramWithValue);
            }

            parameters!.Add(paramWithValues);
            autodeskTransfer.Save(path, SaveOptions.None);
            _logger.LogInformation(60104, "New AutodeskTransferXml created: {path}", path);
        }
        catch (Exception)
        {
            _logger.LogError(61105, "Saving failed AutoDeskTransferXml");
            return false;
        }

        await Task.CompletedTask;
        return true;
    }

    /// <inheritdoc/>
    public async Task<List<InfoCenterEntry>> SyncFromAutodeskTransferAsync(string path, ObservableDictionary<string, Parameter> ParameterDictionary)
    {
        List<InfoCenterEntry> syncedParameter = [];
        List<LiftHistoryEntry> syncedLiftHistoryEntries = [];
        var updatedAutodeskTransfer = await LoadParameterAsync(path);

        foreach (var param in updatedAutodeskTransfer)
        {
            if (ParameterDictionary.TryGetValue(param.Name, out var dictionary))
            {
                var oldValue = ParameterDictionary[dictionary.Name].Value;
                if (dictionary.Value != param.Value)
                {
                    if (string.IsNullOrWhiteSpace(param.Value) && string.IsNullOrWhiteSpace(dictionary.Value))
                        continue;
                    if (ParameterDictionary[dictionary.Name].ParameterTyp == ParameterTypValue.Boolean)
                    {
                        ParameterDictionary[dictionary.Name].Value = string.Equals(param.Value, "True", StringComparison.CurrentCultureIgnoreCase) ? "True" : "False";
                    }
                    else if (ParameterDictionary[dictionary.Name!].ParameterTyp == ParameterTypValue.DropDownList)
                    {
                        ParameterDictionary[dictionary.Name].Value = param.Value;

                        ParameterDictionary[dictionary.Name].DropDownListValue = LiftParameterHelper.GetDropDownListValue(ParameterDictionary[dictionary.Name].DropDownList, ParameterDictionary[dictionary.Name].Value);
                    }
                    else
                    {
                        ParameterDictionary[dictionary.Name].Value = param.Value;
                    }
                    ParameterDictionary[dictionary.Name].IsDirty = false;
                    syncedLiftHistoryEntries.Add(GenerateLiftHistoryEntry(ParameterDictionary[dictionary.Name]));
                    syncedParameter.Add(new(InfoCenterEntryState.None)
                    {
                        ParameterName = dictionary.DisplayName,
                        UniqueName = dictionary.Name,
                        OldValue = oldValue,
                        NewValue = dictionary.Value,
                    });
                }
            }
        }
        if (syncedLiftHistoryEntries.Count > 0)
        {
            _ = AddParameterListToHistoryAsync(syncedLiftHistoryEntries, path, false);
        }
        return syncedParameter;
    }

    /// <inheritdoc/>
    public async Task StartAutoSaveTimerAsync(int period, string? fullPath, bool adminMode)
    {
        if (string.IsNullOrWhiteSpace(fullPath) || _parameterDictionary is null)
        {
            return;
        }
        _autoSaveTimer?.Dispose();
        _autoSaveTimer = new PeriodicTimer(TimeSpan.FromMinutes(period));
        while (await _autoSaveTimer.WaitForNextTickAsync())
        {
            if (!_saveAllParameterIsRunninng)
            {
                _logger.LogInformation(60100, "AutoSave started");
                if (_parameterDictionary.Values.Any(p => p.IsDirty))
                {
                    await SaveAllParameterAsync(fullPath, adminMode);
                }
            }
        }
    }
    /// <inheritdoc/>
    public async Task StopAutoSaveTimerAsync()
    {
        _autoSaveTimer?.Dispose();
        _logger.LogInformation(60100, "AutoSave disabled");
        await Task.CompletedTask;
    }

    private bool ValidatePath(string path, bool readOnlyAllowed)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _logger.LogError(61001, "Path is null or whiteSpace");
            return false;
        }

        if (!path.StartsWith("C:\\Work"))
        {
            _logger.LogError(61001, "Path is not in workspace");
            return false;
        }

        if (!path.EndsWith("AutoDeskTransfer.xml"))
        {
            _logger.LogError(61001, "Path is not a AutoDeskTransfer.xml");
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

    private bool AddNewParameterToXml(Parameter parameter, XElement doc)
    {
        var parameters = doc.Element("parameters");
        if (parameters is null)
        {
            return false;
        }
        try
        {
            var paramWithValue =
            new XElement("ParamWithValue",
            new XElement("name", parameter.Name),
            new XElement("typeCode", parameter.TypeCode),
            new XElement("value", parameter.Value),
            new XElement("comment", parameter.Comment),
            new XElement("isKey", parameter.IsKey.ToString().ToLower()));
            parameters.Add(paramWithValue);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(61100, ex, "add Parameter to xml failed");
            return false;
        }
    }

    private static KeyValuePair<string, string> ValidatePdfValue(PdfAcroField field)
    {
        var name = field.Name;

        if (field.Value is null || field.Value.ToString() == "<FEFF>")
            return new KeyValuePair<string, string>(name, string.Empty);

        var fieldTypName = field.GetType().Name;

        var pdfValue = fieldTypName switch
        {
            "PdfTextField" => ((PdfSharp.Pdf.PdfString)field.Value).Value,
            "PdfCheckBoxField" => string.Equals(field.Value.ToString(), "/Yes", StringComparison.CurrentCultureIgnoreCase) ? "True" : "False",
            "PdfRadioButtonField" => string.Equals(field.Value.ToString(), "/Off", StringComparison.CurrentCultureIgnoreCase) ? string.Empty : field.Value.ToString()?.Replace("/", ""),
            _ => string.Empty,
        };

        if (name.StartsWith("var_FH"))
        {
            if (string.IsNullOrWhiteSpace(pdfValue))
                pdfValue = "0";
            if (double.TryParse(pdfValue, out double travel))
            {
                pdfValue = (travel / 1000).ToString();
            }
            else
            {
                pdfValue = "0";
            }
        }

        if (string.IsNullOrWhiteSpace(pdfValue))
            return new KeyValuePair<string, string>(name, string.Empty);

        return name switch
        {
            var n when n.StartsWith("var_Aufzugstyp") => new KeyValuePair<string, string>(name, GetLiftTyp(name, pdfValue)),
            var n when n.StartsWith("var_Zugang") => new KeyValuePair<string, string>(name.Replace("Zugang_bei", "ZUGANSSTELLEN"), pdfValue),
            var n when n.StartsWith("var_KabTueF") => new KeyValuePair<string, string>(name.Replace("KabTueF", "KabinengewichtCAD"), pdfValue),
            var n when n.StartsWith("var_AuslieferungAm") => new KeyValuePair<string, string>(name, LiftParameterHelper.GetShortDateFromCalendarWeek(pdfValue)),
            var n when n.StartsWith("var_ErstelltAm") => new KeyValuePair<string, string>(name, LiftParameterHelper.GetShortDate(pdfValue)),
            _ => new KeyValuePair<string, string>(name, pdfValue.Trim('(', ')')),
        };
    }

    private void AddParameterToXml(Parameter parameter, XElement xmlparameter)
    {
        if (parameter.ParameterTyp is not ParameterTypValue.NumberOnly)
        {
            xmlparameter.Element("value")!.Value = parameter.Value is null ? string.Empty : parameter.Value;
        }
        else
        {
            xmlparameter.Element("value")!.Value = parameter.Value is null ? string.Empty : parameter.Value.Replace(".", ",");
        }

        xmlparameter.Element("comment")!.Value = parameter.Comment is null ? string.Empty : parameter.Comment;
        xmlparameter.Element("isKey")!.Value = parameter.IsKey ? "true" : "false";
        LogSavedParameter(parameter.DisplayName!, parameter.Value!);
    }

    private static string GetLiftTyp(string name, string value)
    {
        var typ = value == "1" ? "Personen" : "Lasten";
        var drive = name.Replace("var_Aufzugstyp", "") switch
        {
            "_TG2" or "_BR1" or "_BR2" or "_BT1" or "_BT2" => "Hydraulik",
            "_BRR" or "_ZZE_S" or "_EZE_SR" => "Seil",
            _ => "Seil"
        };
        return $"{typ} {drive}-Aufzug";
    }

    private static string GetValueFromHtmlNode(HtmlNode? node)
    {
        var childNodes = node?.ChildNodes.Where(x => x.HasAttributes);
        if (childNodes is not null && childNodes.Count() == 2)
        {
            return childNodes.Last().InnerText.Trim();
        }
        return string.Empty;
    }

    private static ParameterCategoryValue GetLiftParameterCategory(Dictionary<string, int> parameterDto, string parameterName)
    {
        if (parameterDto.TryGetValue(parameterName, out int category))
        {
            return (ParameterCategoryValue)category;
        }
        return ParameterCategoryValue.AllgemeineDaten;
    }

    [LoggerMessage(60104, LogLevel.Information,
        "Parameter: {parameterName} Value: {parameterValue} gespeichert")]
    partial void LogSavedParameter(string parameterName, string parameterValue);

    [LoggerMessage(61035, LogLevel.Warning,
    "Unsupported Parameter: {parameterName}")]
    partial void LogUnsupportedParameter(string parameterName);
}