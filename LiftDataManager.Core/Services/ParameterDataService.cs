using Cogs.Collections;
using LiftDataManager.Core.Contracts.Services;
using LiftDataManager.Core.Helpers;
using LiftDataManager.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LiftDataManager.Core.Services
{
    public class ParameterDataService : IParameterDataService
    {
        public async Task<IEnumerable<Parameter>> LoadParameterAsync(string path)
        {
            XElement doc = XElement.Load(path);

            List<Parameter> parameterList =
              (from para in doc.Elements("parameters").Elements("ParamWithValue")
               select new Parameter(para.Element("name").GetAs<string>(),para.Element("typeCode").GetAs<string>(), para.Element("value").GetAs<string>())
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
            //FileInfo AutoDeskTransferInfo = new FileInfo(path);
            //if (AutoDeskTransferInfo.IsReadOnly)
            //{
            //    AutoDeskTransferInfo.IsReadOnly = false;
            //}

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
            await Task.CompletedTask;

            string infotext = $"Parameter gespeichet: { parameter.Name} => { parameter.Value}  \n";
            infotext += $"----------\n";
            return infotext;
        }


        public async Task<string> SaveAllParameterAsync(ObservableDictionary<string, Parameter> ParamterDictionary, string path)
        {
            //FileInfo AutoDeskTransferInfo = new FileInfo(path);
            //if (AutoDeskTransferInfo.IsReadOnly)
            //{
            //    AutoDeskTransferInfo.IsReadOnly = false;
            //}

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

                infotext += $"Parameter gespeichet: {parameter.Name} => {parameter.Value} \n";
            }

            doc.Save(path);
            await Task.CompletedTask;
            infotext += $"----------\n";
            return infotext;
        }
    }
}
