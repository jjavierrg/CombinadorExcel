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
        private Dictionary<IKey, IList<IRelationEntry>> _values = new Dictionary<IKey, IList<IRelationEntry>>();
        public IDictionary<IKey, IList<IRelationEntry>> Values => _values;

        public OriginParser(IEventAggregator eventAggregator, INormalizer normalizer) : base(eventAggregator, normalizer) { }

        public bool Parse()
        {
            _values = new Dictionary<IKey, IList<IRelationEntry>>();

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
                                var keyEntry = ExtractRelationEntry(excelWorksheet, keyColumn, rowNum);
                                key.AddKeyValue(keyEntry);
                            }

                            if (!_values.ContainsKey(key))
                                _values.Add(key, new List<IRelationEntry>());

                            foreach (var column in Columns)
                            {
                                var valueEntry = IoC.Get<IRelationEntry>();
                                valueEntry.OriginColumn = column.Origin;
                                valueEntry.DestinyColumn = column.Destiny;
                                valueEntry.Value = excelWorksheet.Cells[column.Origin + rowNum].Value;

                                _values[key].Add(valueEntry);
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
