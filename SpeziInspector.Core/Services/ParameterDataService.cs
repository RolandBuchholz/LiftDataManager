using SpeziInspector.Core.Contracts.Services;
using SpeziInspector.Core.Helpers;
using SpeziInspector.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public void SaveParameterAsync(Parameter parameter, string path)
        {
            XElement doc = XElement.Load(path);
            Debug.WriteLine($"Parameter gespeichert: {parameter.Name}");
            Debug.WriteLine($"in {path}");

        }


        public void SaveAllParameterAsync(ObservableCollection<Parameter> ParamterList, string path)
        {
            XElement doc = XElement.Load(path);
            Debug.WriteLine($"Alle Parameter gespeichert:");
            Debug.WriteLine($"in {path}");
        }
    }
}
