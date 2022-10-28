using System.Xml.Linq;
using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.DataAccessLayer;
using LiftDataManager.Core.Helpers;

namespace LiftDataManager.Core.Services;

public class ParameterDataService : IParameterDataService
{
    private readonly IValidationParameterDataService _validationParameterDataService;
    private readonly ParameterContext _parametercontext;

    public ParameterDataService( IValidationParameterDataService validationParameterDataService, ParameterContext parametercontext)
    {
        _validationParameterDataService = validationParameterDataService;
        _parametercontext = parametercontext;
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
                var dropdownList = dropdownValues.FirstOrDefault(x => string.Equals(x.Key,par.DropdownList));

                if (dropdownList != null)
                {
                    var dropdownListValues = dropdownList.Select(x => x.Name);
                    newParameter.DropDownList.Add("(keine Auswahl)");
                    if (dropdownListValues is not null)
                        newParameter.DropDownList.AddRange(dropdownListValues!);
                }
            }
            parameterList.Add(newParameter);    
        }

        await Task.CompletedTask;

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
        return TransferDataList;
    }

    public async Task<string> SaveParameterAsync(Parameter parameter, string path)
    {
        FileInfo AutoDeskTransferInfo = new(path);
        if (AutoDeskTransferInfo.IsReadOnly)
        {
            return $"AutoDeskTransferXml schreibgeschützt kein speichern möglich.\n";
        }

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
        }

        doc.Save(path);
        await Task.CompletedTask;

        var infotext = $"Parameter gespeichet: {parameter.Name} => {parameter.Value}  \n";
        infotext += $"----------\n";
        return infotext;
    }

    public async Task<string> SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParamterDictionary, string path, bool adminmode)
    {
        FileInfo AutoDeskTransferInfo = new(path);
        if (AutoDeskTransferInfo.IsReadOnly)
        {
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
                }

                parameter.IsDirty = false;

                infotext += $"Parameter gespeichet: {parameter.Name} => {parameter.Value} \n";
            }
            else
            {
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
}
