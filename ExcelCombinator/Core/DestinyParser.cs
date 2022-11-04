using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using ExcelCombinator.Interfaces;
using OfficeOpenXml;

namespace ExcelCombinator.Core
{
    public class DestinyParser : Parser, IDestinyParser
    {
        public DestinyParser(IEventAggregator eventAggregator, INormalizer normalizer) : base(eventAggregator, normalizer) { }

        public bool Process(IDictionary<IKey, IDictionary<string, object>> values)
        {
            try
            {
                // Validations
                if (string.IsNullOrEmpty(FilePath)) throw new Exception("No destiny file path specified");
                if (!File.Exists(FilePath)) throw new Exception("Can not access to destiny file path");
                if (string.IsNullOrEmpty(SheetName)) throw new Exception("No destiny sheet name specified");
                if (Columns == null || !Columns.Any()) throw new Exception("No destiny columns specified");
                if (KeysColumns == null || !KeysColumns.Any()) throw new Exception("No destiny keys specified");
                if (values == null) throw new Exception("No origin data read");

                NotifyIsBusy(true, "Parsing origin file");
                using (var xlPackage = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var excelWorksheet = xlPackage.Workbook.Worksheets.FirstOrDefault(x => string.Equals(x.Name, SheetName, StringComparison.OrdinalIgnoreCase));
                    if (excelWorksheet == null) throw new Exception("No destiny worksheet found");

                    var totalRows = excelWorksheet.Dimension.End.Row;
                    for (var rowNum = 2; rowNum <= totalRows; rowNum++)
                    {
                        var key = IoC.Get<IKey>();
                        try
                        {
                            foreach (var keyColumn in KeysColumns)
                            {
                                var keyVal = excelWorksheet.Cells[keyColumn.Destiny + rowNum].GetValue<string>();
                                if (NormalizeKeys)
                                    keyVal = _normalizer.Normalize(keyVal);

                                key.AddKeyValue(keyVal);
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        if (!values.ContainsKey(key))
                            continue;

                        foreach (var column in Columns)
                        {
                            var columnsData = values[key];
                            if (!columnsData.ContainsKey(column.Origin))
                                continue;

                            var originValue = columnsData[column.Origin];
                            if (originValue == null) continue;

                            excelWorksheet.SetValue(column.Destiny + rowNum, originValue);
                        }
                    }

                    xlPackage.Save();
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
