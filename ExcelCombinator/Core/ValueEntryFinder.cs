using ExcelCombinator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelCombinator.Core
{
    public abstract class ValueEntryFinder : IValueEntryFinder
    {
        protected Func<IRelationEntry, IKey, bool> predicate = (IRelationEntry key, IKey other) => other.Keys.Any(o => o.OriginColumn == key.OriginColumn && o.DestinyColumn == key.DestinyColumn && string.Equals(o.Value?.ToString() ?? "", key.Value?.ToString() ?? "", StringComparison.OrdinalIgnoreCase));
        public IList<IRelationEntry> GetValueForEntry(IDictionary<IKey, IList<IRelationEntry>> values, IKey key)
        {
            if (values == null || values.Count == 0)
                return null;

            return values.FirstOrDefault(x => IsSameKey(x.Key, key)).Value;
        }

        protected abstract bool IsSameKey(IKey key1, IKey key2);

    }

    public class AndValueEntryFinder : ValueEntryFinder
    {

        protected override bool IsSameKey(IKey key1, IKey key2)
        {
            if (key1.Keys == null || key2.Keys == null)
                return false;

            if (key1.Keys.Count != key2.Keys.Count)
                return false;

            return key1.Keys.All(x => predicate(x, key2));
        }
    }

    public class OrValueEntryFinder : ValueEntryFinder
    {
        protected override bool IsSameKey(IKey key1, IKey key2)
        {
            if (key1.Keys == null || key2.Keys == null)
                return false;

            return key1.Keys.Any(x => predicate(x, key2));
        }
    }
}
