using System.Collections.Generic;

namespace ExcelCombinator.Interfaces
{
    public interface IValueEntryFinder
    {
        IList<IRelationEntry> GetValueForEntry(IDictionary<IKey, IList<IRelationEntry>> values, IKey key);
    }
}