using System.Collections.Generic;

namespace ExcelCombinator.Interfaces
{
    public interface IRelationEntry
    {
        string OriginColumn { get; set; }
        string DestinyColumn { get; set; }
        object Value { get; set; }
    }

    public interface IKey
    {
        IList<IRelationEntry> Keys { get; }
        void AddKeyValue(IRelationEntry value);
    }
}