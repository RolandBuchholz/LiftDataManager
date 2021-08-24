using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpeziInspector.Core.Helpers
{
    public static class ExcelHelper
    {
        public static List<string> ReadExcelParameterListe(string excelFilePath, string sheetname, string auswahlParameterName)
        {
            List<string> rowList = new List<string>();
            ISheet sheet;
            using (var stream = new FileStream(excelFilePath, FileMode.Open))
            {
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                sheet = xssWorkbook.GetSheet(sheetname);

                int startRow = 0;
                int startCell = 0;
                bool dataParsen = false;

                for (int i = (sheet.FirstRowNum); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
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

                while (dataParsen)
                {
                    IRow row = sheet.GetRow(startRow + 1);
                    if (row == null)
                    {
                        return rowList;
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
            }
            return rowList;
        }
    }
}
