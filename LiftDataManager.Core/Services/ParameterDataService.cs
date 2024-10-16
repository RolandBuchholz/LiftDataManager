using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.Models.ComponentModels;
using Microsoft.Extensions.Logging;
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
    public bool CanConnectDataBase()
    {
        if (!_parametercontext.Database.CanConnect())
            return false;
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
            _logger.LogError(61001, "{ path} Path of AutoDeskTransferXml not vaild", path);
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
            _logger.LogError(61101, "Saving failed AutoDeskTransferXml");
            return new Tuple<string, string, string?>("Error", "Error", $"Speichern fehlgeschlagen |{parameter.Name}|");
        }
        await AddParameterListToHistoryAsync(historyEntrys, path, false);
        doc.Save(path);
        return await Task.FromResult(new Tuple<string, string, string?>(parameter.Name, parameter.DisplayName, parameter.Value));
    }

    /// <inheritdoc/>
    public async Task<List<Tuple<string, string, string?>>> SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParameterDictionary, string path, bool adminmode)
    {
        _saveAllParameterIsRunninng = true;
        var saveResult = new List<Tuple<string, string, string?>>();
        if (!ValidatePath(path, false))
        {
            _logger.LogError(61001, "{ path} Path of AutoDeskTransferXml not vaild", path);
            saveResult.Add(new Tuple<string, string, string?>("Error", "Error", "AutoDeskTransferXml Pfad ist nicht gültig"));
            _saveAllParameterIsRunninng = false;
            return saveResult;
        }
        XElement doc = XElement.Load(path);
        var unsavedParameter = ParameterDictionary.Values.Where(p => p.IsDirty);

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
                    _logger.LogError(61101, "Saving failed AutoDeskTransferXml");
                    saveResult.Add(new Tuple<string, string, string?>("Warning", "Warning", $"Speichern fehlgeschlagen |{parameter.Name}|"));
                }
            }
            else
            {
                _logger.LogWarning(61001, "Saving failed { parameter.Name} >Saving is only possible in adminmode<", parameter.Name);
                saveResult.Add(new Tuple<string, string, string?>("Warning", "Warning", $"Parameter: {parameter.Name} ist scheibgeschützt!\nSpeichern nur im Adminmode möglich!"));
            }
        }
        await AddParameterListToHistoryAsync(historyEntrys, path, false);
        doc.Save(path);
        _saveAllParameterIsRunninng = false;
        await Task.CompletedTask;
        return saveResult;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<InfoCenterEntry>> UpdateParameterDictionary(string path, IEnumerable<TransferData> data, ObservableDictionary<string, Parameter> parameterDictionary, bool updateXml = true)
    {
        var infoCenterEntrys = new List<InfoCenterEntry>();
        foreach (var item in data)
        {
            if (parameterDictionary.TryGetValue(item.Name, out Parameter value))
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
                var parameterList = parameterDictionary.Values.ToList();

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
    public async Task StartAutoSaveTimer()
    {
        _autoSaveTimer?.Dispose();
        int period = 3;
        //var autoSavePeriod = _settingService.AutoSavePeriod;
        //if (!string.IsNullOrWhiteSpace(autoSavePeriod))
        //{
        //    period = Convert.ToInt32(autoSavePeriod.Replace(" min", ""));
        //}

        _autoSaveTimer = new PeriodicTimer(TimeSpan.FromSeconds(period));
        while (await _autoSaveTimer.WaitForNextTickAsync())
        {
            if (!_saveAllParameterIsRunninng)
            {
                _logger.LogInformation(60100, "AutoSave started");
                //    var dirty = GetCurrentSpeziProperties().ParameterDictionary!.Values.Any(p => p.IsDirty);
                //    if (CheckOut && dirty)
                //    {
                //        var currentSpeziProperties = GetCurrentSpeziProperties();
                //        await _parameterDataService!.SaveAllParameterAsync(currentSpeziProperties.ParameterDictionary!, currentSpeziProperties.FullPathXml!, currentSpeziProperties.Adminmode);
                //    }
            }
        }
    }
    /// <inheritdoc/>
    public async Task StopAutoSaveTimer()
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