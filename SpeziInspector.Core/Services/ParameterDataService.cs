using SpeziInspector.Core.Contracts.Services;
using SpeziInspector.Core.Helpers;
using SpeziInspector.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System;

namespace SpeziInspector.Core.Services
{
    public class ParameterDataService : IParameterDataService
    {
        public async Task<IEnumerable<Parameter>> LoadParameterAsync(string path)
        {
            XElement doc = XElement.Load(path);

            // Order By Display Order
            List<Parameter> parameterList =
              (from para in doc.Elements("parameters").Elements("ParamWithValue")
                   //orderby para.Element("name").Value
               select new Parameter(para.Element("typeCode").GetAs<string>(), para.Element("value").GetAs<string>())
               {
                   Name = para.Element("name").GetAs<string>(),
                   Comment = para.Element("comment").GetAs<string>(),
                   IsKey = para.Element("isKey").GetAs<bool>(),
                   IsDirty = false,
               }).ToList();

            await Task.CompletedTask;
            return parameterList;
        }

        public async Task SaveParameterAsync(Parameter parameter, string path)
        {
            XElement doc = XElement.Load(path);

            // Find a specific customer
            XElement xmlparameter =
              (from para in doc.Elements("parameters").Elements("ParamWithValue")
               where para.Element("name").Value == parameter.Name
               select para).SingleOrDefault();

            // Modify some of the node values

            switch (parameter.TypeCode.ToLower())
            {
                case "boolean":
                    xmlparameter.Element("value").Value = parameter.Value.ToString();
                    break;

                case "date":
                    var exceldate = parameter.Date.Value.DateTime.ToOADate().ToString();
                    xmlparameter.Element("value").Value = exceldate;
                    break;

                default:
                    xmlparameter.Element("value").Value = parameter.Value;
                    break;
            }

            xmlparameter.Element("comment").Value = parameter.Comment;
            xmlparameter.Element("isKey").Value = parameter.IsKey.ToString().ToLower();

            doc.Save(path); 
            
            Debug.WriteLine($"Parameter gespeichert: {parameter.Name}");
            Debug.WriteLine($"in vorhandene {path}");
            await Task.CompletedTask;
        }


        public async Task SaveAllParameterAsync(ObservableCollection<Parameter> ParamterList, string path)
        {
            XElement doc = XElement.Load(path);

            var unsavedParameter = ParamterList.Where(p => p.IsDirty);

            foreach (var parameter in unsavedParameter)
            {
                // Find a specific customer
                XElement xmlparameter =
                  (from para in doc.Elements("parameters").Elements("ParamWithValue")
                   where para.Element("name").Value == parameter.Name
                   select para).SingleOrDefault();

                // Modify some of the node values

                switch (parameter.TypeCode.ToLower())
                {
                    case "boolean":
                        xmlparameter.Element("value").Value = parameter.Value.ToString();
                        break;

                    case "date":
                        var exceldate = parameter.Date.Value.DateTime.ToOADate().ToString();
                        xmlparameter.Element("value").Value = exceldate;
                        break;

                    default:
                        xmlparameter.Element("value").Value = parameter.Value;
                        break;
                }

                xmlparameter.Element("comment").Value = parameter.Comment;
                xmlparameter.Element("isKey").Value = parameter.IsKey.ToString().ToLower();

                parameter.IsDirty = false;

                Debug.WriteLine($"Parameter gespeichert: {parameter.Name}");
                Debug.WriteLine($"in vorhandene {path}");
            }

            doc.Save(path);

            Debug.WriteLine($"Alle Parameter gespeichert:");
            Debug.WriteLine($"in {path}");
            await Task.CompletedTask;
        }
    }
}
