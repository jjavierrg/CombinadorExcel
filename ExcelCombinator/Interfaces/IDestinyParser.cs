using System.Collections.Generic;

namespace ExcelCombinator.Interfaces
{
    public interface IDestinyParser: IParser
    {
        bool Process(IDictionary<IKey, IDictionary<string, object>> values);
    }
}