using System.Collections.Generic;
using OfficeOpenXml;

namespace ExcelCombinator.Models.Interfaces
{
    public interface IOriginParser: IParser
    {
        void Parse(ExcelWorksheets excel);
        IDictionary<IKey, Dictionary<string, string>> Values { get; }
    }
}