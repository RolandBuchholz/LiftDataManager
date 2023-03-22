﻿using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.Helpers;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace LiftDataManager.Core.Services;

public partial class ParameterDataService : IParameterDataService
{
    private readonly IValidationParameterDataService _validationParameterDataService;
    private readonly ParameterContext _parametercontext;
    private readonly ILogger<ParameterDataService> _logger;

    public ParameterDataService(IValidationParameterDataService validationParameterDataService,
                                 ParameterContext parametercontext, ILogger<ParameterDataService> logger)
    {
        _validationParameterDataService = validationParameterDataService;
        _parametercontext = parametercontext;
        _logger = logger;
    }

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

    public async Task<IEnumerable<Parameter>> InitializeParametereFromDbAsync()
    {
        List<Parameter> parameterList = new();

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
                var newParameter = new Parameter(par.Value!, par.ParameterTypeCodeId, par.ParameterTypId, _validationParameterDataService)
                {
                    Name = par.Name,
                    DisplayName = par.DisplayName,
                    ParameterCategory = (ParameterBase.ParameterCategoryValue)par.ParameterCategoryId,
                    DefaultUserEditable = par.DefaultUserEditable,
                    Comment = par.Comment,
                    IsKey = par.IsKey,
                    IsDirty = false
                };

                if (par.DropdownList is not null)
                {
                    var dropdownList = dropdownValues.FirstOrDefault(x => string.Equals(x.Key, par.DropdownList));

                    if (dropdownList is not null)
                    {
                        var dropdownListValues = dropdownList.Select(x => x.Name);
                        newParameter.DropDownList.Add("(keine Auswahl)");
                        if (dropdownListValues is not null)
                            foreach (var item in dropdownListValues)
                            {
                                newParameter.DropDownList.Add(item!);
                            }
                    }
                }
                parameterList.Add(newParameter);
            }
        }


        await Task.CompletedTask;
        _logger.LogInformation(60100, "Parameter from database initialized");
        return parameterList;
    }

    public async Task<IEnumerable<TransferData>> LoadParameterAsync(string path)
    {
        XElement doc = XElement.Load(path);
        var TransferDataList = (from para in doc.Elements("parameters").Elements("ParamWithValue")
                                select new TransferData(
                                                     para.Element("name")!.GetAs<string>()!,
                                                     para.Element("value")!.GetAs<string>()!,
                                                     para.Element("comment")!.GetAs<string>()!,
                                                     para.Element("isKey")!.GetAs<bool>()))
                                  .ToList();
        await Task.CompletedTask;
        _logger.LogInformation(60101, "Parameter from {path} loaded", path);
        return TransferDataList;
    }

    public async Task<string> SaveParameterAsync(Parameter parameter, string path)
    {
        FileInfo AutoDeskTransferInfo = new(path);
        if (AutoDeskTransferInfo.IsReadOnly)
        {
            _logger.LogError(61101, "Saving failed AutoDeskTransferXml is readonly");
            return $"AutoDeskTransferXml schreibgeschützt kein speichern möglich.\n";
        }

        string infotext;

        XElement doc = XElement.Load(path);

        // Find a specific customer
        XElement? xmlparameter =
          (from para in doc.Elements("parameters").Elements("ParamWithValue")
           where para.Element("name")!.Value == parameter.Name
           select para).SingleOrDefault();

        // Modify some of the node values
        if (xmlparameter is not null)
        {
            xmlparameter.Element("comment")!.Value = parameter.Comment is null ? string.Empty : parameter.Comment;
            xmlparameter.Element("isKey")!.Value = parameter.IsKey ? "true" : "false";
            xmlparameter.Element("value")!.Value = parameter.Value is null ? string.Empty : parameter.Value;
            LogSavedParameter(parameter.DisplayName!, parameter.Value!);
            infotext = $"Parameter gespeichet: {parameter.Name} => {parameter.Value}  \n";
            infotext += $"----------\n";
        }
        else
        {
            _logger.LogError(61101, "Saving failed AutoDeskTransferXml");
            infotext = $"Speichern fehlgeschlagen |{parameter.Name}|\n";
            infotext += $"----------\n";
        }

        doc.Save(path);
        await Task.CompletedTask;


        return infotext;
    }

    public async Task<string> SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParamterDictionary, string path, bool adminmode)
    {
        FileInfo AutoDeskTransferInfo = new(path);
        if (AutoDeskTransferInfo.IsReadOnly)
        {
            _logger.LogError(61101, "Saving failed AutoDeskTransferXml is readonly");
            return $"AutoDeskTransferXml schreibgeschützt kein speichern möglich.\n";
        }

        var infotext = $"Folgende Parameter wurden in {path} gespeichet \n";

        XElement doc = XElement.Load(path);

        var unsavedParameter = ParamterDictionary.Values.Where(p => p.IsDirty);

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
                    xmlparameter.Element("value")!.Value = parameter.Value is null ? string.Empty : parameter.Value;
                    xmlparameter.Element("comment")!.Value = parameter.Comment is null ? string.Empty : parameter.Comment;
                    xmlparameter.Element("isKey")!.Value = parameter.IsKey ? "true" : "false";
                    parameter.IsDirty = false;
                    LogSavedParameter(parameter.DisplayName!, parameter.Value!);
                    infotext += $"Parameter gespeichet: {parameter.Name} => {parameter.Value} \n";

                }
                else
                {
                    _logger.LogError(61101, "Saving failed AutoDeskTransferXml");
                    infotext += $"Achtung: Speichern fehlgeschlagen |{parameter.Name}|\n";
                }
            }
            else
            {
                _logger.LogWarning(61001, "Saving failed { parameter.Name} >Saving is only possible in adminmode<", parameter.Name);
                infotext += $"----------\n";
                infotext += $"Parameter: {parameter.Name} ist scheibgeschützt! \n";
                infotext += $"Speichern nur im Adminmode möglich!\n";
                infotext += $"----------\n";
            }
        }

        doc.Save(path);
        await Task.CompletedTask;
        infotext += $"----------\n";
        return infotext;
    }

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

    [LoggerMessage(60104, LogLevel.Information,
        "Parameter: {parameterName} Value: {parameterValue} gespeichert")]
    partial void LogSavedParameter(string parameterName, string parameterValue);
}