using System;
using System.Collections.Generic;
using System.Linq;
using ExcelCombinator.Models.Interfaces;
using OfficeOpenXml;

namespace ExcelCombinator.Models.Core
{
    public class DestinyParser: Parser, IDestinyParser
    {
        public void Process(ExcelWorksheets excel, IDictionary<IKey, IDictionary<string, string>> values)
        {
            if (Columns == null || !Columns.Any()) throw new Exception("No destiny columns specified");
            if (KeysColumns == null || !KeysColumns.Any()) throw new Exception("No destiny keys specified");
            if (values == null) throw new Exception("No origin data read");

            var excelWorksheet = excel.FirstOrDefault(x => string.Equals(x.Name, SheetName, StringComparison.OrdinalIgnoreCase));
            if (excelWorksheet == null) throw new Exception("No destiny worksheet found");

            var totalRows = excelWorksheet.Dimension.End.Row;
            for (var rowNum = 2; rowNum <= totalRows; rowNum++)
            {
                IKey key = new Key();
                foreach (var keyColumn in KeysColumns)
                    key.AddKeyValue(excelWorksheet.Cells[keyColumn.Destiny + rowNum].GetValue<string>());

                if (!values.ContainsKey(key))
                    continue;


                foreach (var column in Columns)
                {
                    var columnsData = values[key];
                    if (!columnsData.ContainsKey(column.Origin))
                        continue;

                    var originValue = columnsData[column.Origin];
                    if (string.IsNullOrEmpty(originValue)) continue;

                    excelWorksheet.SetValue(column.Destiny + rowNum, originValue);
                }
            }
        }
    }
}
