    using System;
using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Caliburn.Micro;
    using ExcelCombinator.Models.Interfaces;
using OfficeOpenXml;

namespace ExcelCombinator.Models.Core
{
    public class OriginParser: Parser, IOriginParser
    {
        private Dictionary<IKey, IDictionary<string, string>> _values = new Dictionary<IKey, IDictionary<string, string>>();
        public IDictionary<IKey, IDictionary<string, string>> Values => _values;

        public OriginParser(IEventAggregator eventAggregator) : base(eventAggregator) { }

        public bool Parse()
        {
            _values = new Dictionary<IKey, IDictionary<string, string>>();

            try
            {
                // Validations
                if (string.IsNullOrEmpty(FilePath)) throw new Exception("No origin file path specified");
                if (!File.Exists(FilePath)) throw new Exception("Can not acces to origin file path");
                if (string.IsNullOrEmpty(SheetName)) throw new Exception("No origin sheet name specified");
                if (Columns == null || !Columns.Any()) throw new Exception("No origin columns specified");
                if (KeysColumns == null || !KeysColumns.Any()) throw new Exception("No origin keys specified");

                NotifyIsBusy(true, "Parsing origin file");
                using (var xlPackage = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var excelWorksheet = xlPackage.Workbook.Worksheets.FirstOrDefault(x => string.Equals(x.Name, SheetName, StringComparison.OrdinalIgnoreCase));
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

                return true;
            }
            catch (Exception ex)
            {
                NotifyException(ex);
                return false;
            }
            finally
            {
                NotifyIsBusy(false, string.Empty);
            }
        }
    }
}
