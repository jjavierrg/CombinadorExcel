using System.Collections.Generic;
using OfficeOpenXml;

namespace ExcelCombinator.Models.Interfaces
{
    public interface IDestinyParser: IParser
    {
        bool Process(IDictionary<IKey, IDictionary<string, string>> values);
    }
}