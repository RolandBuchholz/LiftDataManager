using System.Xml.Linq;
using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Helpers;

namespace LiftDataManager.Core.Services;

public class ParameterDataService : IParameterDataService
{
    private readonly IAuswahlParameterDataService _auswahlParameterDataService;
    private readonly IValidationParameterDataService _validationParameterDataService;

    public ParameterDataService(IAuswahlParameterDataService auswahlParameterDataService, IValidationParameterDataService validationParameterDataService)
    {
        _auswahlParameterDataService = auswahlParameterDataService;
        _validationParameterDataService = validationParameterDataService;
        _validationParameterDataService = validationParameterDataService;
    }

    public async Task<IEnumerable<Parameter>> LoadParameterAsync(string path)
    {
        XElement doc = XElement.Load(path);

        List<Parameter> parameterList =
          (from para in doc.Elements("parameters").Elements("ParamWithValue")
           select new Parameter(
                                para.Element("name")!.GetAs<string>()!,
                                para.Element("typeCode")!.GetAs<string>()!,
                                para.Element("value")!.GetAs<string>()!,
                                _auswahlParameterDataService,
                                _validationParameterDataService)
           {
               Comment = !string.IsNullOrWhiteSpace(para.Element("comment")?.GetAs<string>()) ? para.Element("comment")!.GetAs<string>() : string.Empty,
               IsKey =  para.Element("isKey")!.GetAs<bool>(),
               IsDirty = false,
           }).ToList();

        await Task.CompletedTask;
        return parameterList;
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

    public async Task<string> SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParamterDictionary, string path)
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

        doc.Save(path);
        await Task.CompletedTask;
        infotext += $"----------\n";
        return infotext;
    }
}
