using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiftDataManager.Core.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace LiftDataManager.Core.Helpers
{
    public static class ExcelHelper
    {
        public static async Task<List<AuswahlParameter>> ReadExcelParameterListeAsync(string excelFilePath, string[,] importAusawahlParameter)
        {
            List<string> rowList = new List<string>();
            ISheet sheet;
            List<AuswahlParameter> _data = new List<AuswahlParameter>();
            await using (var stream = new FileStream(excelFilePath, FileMode.Open))
            {
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                string sheetname;
                int startRow = 0;
                int startCell = 0;
                bool dataParsen = false;

                for (int i = 0; i < (importAusawahlParameter.Length) / 3; i++)
                {
                    sheetname = importAusawahlParameter[i, 0];
                    sheet = xssWorkbook.GetSheet(sheetname);
                    string auswahlParameterName = importAusawahlParameter[i, 1];

                    for (int j = (sheet.FirstRowNum); j <= sheet.LastRowNum; j++)
                    {
                        IRow row = sheet.GetRow(j);
                        if (row == null) continue;
                        var startIndex = row.Cells.SingleOrDefault(d => d.CellType == CellType.String && d.StringCellValue == auswahlParameterName);
                        if (startIndex != null)
                        {
                            startRow = startIndex.RowIndex;
                            startCell = startIndex.ColumnIndex;
                            dataParsen = true;
                            break;
                        }
                    }

                    rowList.Clear();

                    while (dataParsen)
                    {
                        IRow row = sheet.GetRow(startRow + 1);
                        if (row == null)
                        {
                            dataParsen = false;
                            break;
                        }
                        if (row.GetCell(startCell) != null)
                        {
                            if (!string.IsNullOrEmpty(row.GetCell(startCell).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(startCell).ToString()))
                            {
                                rowList.Add(row.GetCell(startCell).ToString());
                            }
                            if (startRow < sheet.LastRowNum - 1)
                            {
                                startRow++;
                            }
                            else
                            {
                                dataParsen = false;
                            }
                        }
                        else
                        {
                            dataParsen = false;
                        }
                    }

                    AuswahlParameter _auswahlParameter = new();
                    _auswahlParameter.Name = importAusawahlParameter[i, 2];

                    _auswahlParameter.Auswahlliste.Add("(keine Auswahl)");

                    foreach (string par in rowList)
                    {
                        _auswahlParameter.Auswahlliste.Add(par);
                    }
                    _data.Add(_auswahlParameter);
                }
            }
            return _data;
        }
    }
}
