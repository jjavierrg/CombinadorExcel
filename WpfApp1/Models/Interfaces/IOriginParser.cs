using System.Collections.Generic;
using OfficeOpenXml;

namespace ExcelCombinator.Models.Interfaces
{
    public interface IOriginParser: IParser
    {
        bool Parse();
        IDictionary<IKey, IDictionary<string, string>> Values { get; }
    }
}