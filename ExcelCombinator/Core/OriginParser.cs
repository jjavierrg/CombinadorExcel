using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using ExcelCombinator.Interfaces;
using OfficeOpenXml;

namespace ExcelCombinator.Core
{
    public class OriginParser: Parser, IOriginParser
    {
        private Dictionary<IKey, IDictionary<string, object>> _values = new Dictionary<IKey, IDictionary<string, object>>();
        public IDictionary<IKey, IDictionary<string, object>> Values => _values;

        public OriginParser(IEventAggregator eventAggregator, INormalizer normalizer) : base(eventAggregator, normalizer) { }

        public bool Parse()
        {
            _values = new Dictionary<IKey, IDictionary<string, object>>();

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
                        var key = IoC.Get<IKey>();

                        try
                        {
                            foreach (var keyColumn in KeysColumns)
                            {
                                var KeyVal = excelWorksheet.Cells[keyColumn.Origin + rowNum].GetValue<string>();
                                if (NormalizeKeys)
                                    KeyVal = _normalizer.Normalize(KeyVal);

                                key.AddKeyValue(KeyVal);
                            }

                            if (!_values.ContainsKey(key))
                                _values.Add(key, new Dictionary<string, object>());

                            foreach (var column in Columns)
                            {
                                var value  = excelWorksheet.Cells[column.Origin + rowNum].Value;
                                _values[key].Add(column.Origin, value);
                            }
                        }
                        catch (Exception)
                        {
                            if (key != null && _values.ContainsKey(key))
                                _values.Remove(key);
                        }
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
