using System.Collections.Generic;
using OfficeOpenXml;

namespace ExcelCombinator.Models.Interfaces
{
    public interface IDestinyParser: IParser
    {
        void Process(ExcelWorksheets excel, IDictionary<IKey, IDictionary<string, string>> values);
    }
}