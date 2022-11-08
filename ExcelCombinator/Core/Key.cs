using ExcelCombinator.Interfaces;
using System.Collections.Generic;

namespace ExcelCombinator.Core
{
    public class RelationEntry : IRelationEntry
    {
        public string OriginColumn { get ; set; }
        public string DestinyColumn { get; set; }
        public object Value { get; set; }
    }

    public class Key :IKey
    {
        public IList<IRelationEntry> Keys { get; }

        public Key()
        {
            Keys = new List<IRelationEntry>();
        }

        public void AddKeyValue(IRelationEntry value)
        {
            Keys.Add(value);
        }
    }
}
