using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Helpers;
using LiftDataManager.Core.Models;

namespace LiftDataManager.Core.Services
{
    public class ParameterDataService : IParameterDataService
    {
        public async Task<IEnumerable<Parameter>> LoadParameterAsync(string path)
        {
            XElement doc = XElement.Load(path);

            List<Parameter> parameterList =
              (from para in doc.Elements("parameters").Elements("ParamWithValue")
               select new Parameter(para.Element("name").GetAs<string>(), para.Element("typeCode").GetAs<string>(), para.Element("value").GetAs<string>())
               {
                   Comment = para.Element("comment").GetAs<string>(),
                   IsKey = para.Element("isKey").GetAs<bool>(),
                   IsDirty = false,
               }).ToList();

            await Task.CompletedTask;
            return parameterList;
        }

        public async Task<string> SaveParameterAsync(Parameter parameter, string path)
        {
            XElement doc = XElement.Load(path);

            // Find a specific customer
            XElement xmlparameter =
              (from para in doc.Elements("parameters").Elements("ParamWithValue")
               where para.Element("name").Value == parameter.Name
               select para).SingleOrDefault();

            // Modify some of the node values

            xmlparameter.Element("value").Value = parameter.Value is null ? string.Empty : parameter.Value;
            xmlparameter.Element("comment").Value = parameter.Comment is null ? string.Empty : parameter.Comment;
            xmlparameter.Element("isKey").Value = parameter.IsKey ? "true" : "false";

            doc.Save(path);
            await Task.CompletedTask;

            string infotext = $"Parameter gespeichet: {parameter.Name} => {parameter.Value}  \n";
            infotext += $"----------\n";
            return infotext;
        }


        public async Task<string> SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParamterDictionary, string path)
        {
            string infotext = $"Folgende Parameter wurden in {path} gespeichet \n";

            XElement doc = XElement.Load(path);

            var unsavedParameter = ParamterDictionary.Values.Where(p => p.IsDirty);

            foreach (var parameter in unsavedParameter)
            {
                // Find a specific customer
                XElement xmlparameter =
                  (from para in doc.Elements("parameters").Elements("ParamWithValue")
                   where para.Element("name").Value == parameter.Name
                   select para).SingleOrDefault();

                // Modify some of the node values
                xmlparameter.Element("value").Value = parameter.Value is null ? string.Empty : parameter.Value;
                xmlparameter.Element("comment").Value = parameter.Comment is null ? string.Empty : parameter.Comment;
                xmlparameter.Element("isKey").Value = parameter.IsKey ? "true" : "false";

                parameter.IsDirty = false;

                infotext += $"Parameter gespeichet: {parameter.Name} => {parameter.Value} \n";
            }

            doc.Save(path);
            await Task.CompletedTask;
            infotext += $"----------\n";
            return infotext;
        }
    }
}
