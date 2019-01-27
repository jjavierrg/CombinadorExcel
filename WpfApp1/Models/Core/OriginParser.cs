using System;
using System.Collections.Generic;
using System.Linq;
using ExcelCombinator.Models.Interfaces;
using OfficeOpenXml;

namespace ExcelCombinator.Models.Core
{
    public class OriginParser: Parser, IOriginParser
    {
        private Dictionary<IKey, Dictionary<string, string>> _values;
        public IDictionary<IKey, Dictionary<string, string>> Values => _values;

        public void Parse(ExcelWorksheets excel)
        {
            _values = new Dictionary<IKey, Dictionary<string, string>>();
            if (Columns == null || !Columns.Any()) throw new Exception("No origin columns specified");
            if (KeysColumns == null || !KeysColumns.Any()) throw new Exception("No origin keys specified"); ;

            var excelWorksheet = excel.FirstOrDefault(x => string.Equals(x.Name, SheetName, StringComparison.OrdinalIgnoreCase));
            if (excelWorksheet == null) throw new Exception("No origin worksheet found");

            var totalRows = excelWorksheet.Dimension.End.Row;
            for (var rowNum = 2; rowNum <= totalRows; rowNum++)
            {
                var key = new Key();
                foreach (var keyColumn in KeysColumns)
                    key.Keys.Add(excelWorksheet.Cells[keyColumn.Origin + rowNum].GetValue<string>());

                if (!_values.ContainsKey(key))
                    _values.Add(key, new Dictionary<string, string>());

                foreach (var column in Columns)
                    _values[key].Add(column.Origin, excelWorksheet.Cells[column.Origin + rowNum].GetValue<string>());
            }
        }
    }
}
