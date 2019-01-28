using System.Collections.Generic;

namespace ExcelCombinator.Interfaces
{
    public interface IKey
    {
        void AddKeyValue(string value);
        void AddKeyValues(IEnumerable<string> values);
    }
}