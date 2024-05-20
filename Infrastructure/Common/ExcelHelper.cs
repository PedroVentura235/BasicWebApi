using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using BasicWebApi.Application.Common.Interfaces;
using System.Globalization;

namespace BasicWebApi.Infrastructure.Common;

public class ExcelHelper : IExcelHelper
{
    public List<T> Import<T>(string filePath) where T : new()
    {
        string extension = Path.GetExtension(filePath);
        IWorkbook workbook;
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            if (extension == ".xlsx")
                workbook = new XSSFWorkbook(stream);
            else
                workbook = new HSSFWorkbook(stream);
        }

        ISheet sheet = workbook.GetSheetAt(0);

        IRow rowHeader = sheet.GetRow(0);
        Dictionary<string, int> colIndexList = new();
        foreach (ICell cell in rowHeader.Cells)
        {
            string colName = cell.StringCellValue;
            if (String.IsNullOrEmpty(colName))
                colName = colIndexList.Last().Key + "1";
            colIndexList.Add(colName.TrimEnd(), cell.ColumnIndex);
        }

        List<T> listResult = new();
        int currentRow = 1;
        while (currentRow <= sheet.LastRowNum)
        {
            IRow row = sheet.GetRow(currentRow);
            if (row == null) break;

            T obj = new();

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.Name.Equals("Id"))
                    continue;
                if (!colIndexList.ContainsKey(property.Name))
                    throw new Exception($"Column {property.Name} not found.");
                //if (!columnPropertyMapping.ContainsKey(property.Name))
                //    throw new Exception($"Column mapping for property {property.Name} not found.");

                //var colName = columnPropertyMapping[property.Name];
                //if (!colIndexList.ContainsKey(colName))
                //    throw new Exception($"Column {colName} not found.");

                var colIndex = colIndexList[property.Name];
                var cell = row.GetCell(colIndex);

                if (cell == null)
                {
                    property.SetValue(obj, null);
                }
                else if (property.PropertyType == typeof(string))
                {
                    cell.SetCellType(CellType.String);
                    property.SetValue(obj, cell.StringCellValue);
                }
                else if (property.PropertyType == typeof(int))
                {
                    cell.SetCellType(CellType.Numeric);
                    property.SetValue(obj, Convert.ToInt32(cell.NumericCellValue));
                }
                else if (property.PropertyType == typeof(decimal))
                {
                    cell.SetCellType(CellType.Numeric);
                    property.SetValue(obj, Convert.ToDecimal(cell.NumericCellValue));
                }
                else if (property.PropertyType == typeof(double))
                {
                    cell.StringCellValue.Replace(',', '.');
                    cell.SetCellValue(cell.StringCellValue.Replace(',', '.'));
                    //cell.SetCellType(CellType.Numeric);
                    if (!string.IsNullOrEmpty(cell.StringCellValue))
                        property.SetValue(obj, Convert.ToDouble(cell.StringCellValue, CultureInfo.InvariantCulture));
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    property.SetValue(obj, cell.DateCellValue);
                }
                else if (property.PropertyType == typeof(DateOnly))
                {
                    property.SetValue(obj, cell.DateOnlyCellValue);
                }
                else if (property.PropertyType == typeof(TimeOnly))
                {
                    property.SetValue(obj, cell.TimeOnlyCellValue);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    cell.SetCellType(CellType.Boolean);
                    property.SetValue(obj, cell.BooleanCellValue);
                }
                else
                {
                    property.SetValue(obj, Convert.ChangeType(cell.StringCellValue, property.PropertyType));
                }
            }

            listResult.Add(obj);
            currentRow++;
        }

        return listResult;
    }
}