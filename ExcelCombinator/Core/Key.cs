using System;
using System.Collections.Generic;
using System.Linq;
using ExcelCombinator.Interfaces;

namespace ExcelCombinator.Core
{
    public class Key :IKey
    {
        public IList<string> Keys { get; }

        public Key()
        {
            Keys = new List<string>();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Key;
            if (other == null) return false;

            if (Keys == null && other.Keys == null) return true;
            if (Keys == null || other.Keys == null) return false;
            if (Keys.Count != other.Keys.Count) return false;
            if (Keys.Count == 0 && other.Keys.Count == 0) return true;

            return Keys.All(x => other.Keys.Any(s => (s ?? string.Empty).Equals((x ?? string.Empty), StringComparison.OrdinalIgnoreCase)));
        }

        protected bool Equals(Key other)
        {
            return Equals(Keys, other.Keys);
        }

        public override int GetHashCode()
        {
            return (Keys != null ? string.Join("", Keys).GetHashCode() : 0);
        }

        public void AddKeyValue(string value)
        {
            Keys.Add(value);
        }

        public void AddKeyValues(IEnumerable<string> values)
        {
            foreach (var value in values)
                AddKeyValue(value);
        }
    }
}
