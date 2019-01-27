using System.Collections.Generic;

namespace ExcelCombinator.Models.Interfaces
{
    public interface IKey
    {
        void AddKeyValue(string value);
        void AddKeyValues(IEnumerable<string> values);
    }
}