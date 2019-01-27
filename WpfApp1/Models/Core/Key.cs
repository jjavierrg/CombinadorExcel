using System;
using System.Collections.Generic;
using System.Linq;
using ExcelCombinator.Models.Interfaces;

namespace ExcelCombinator.Models.Core
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
            if (!(obj is Key other)) return false;
            if (Keys == null && other.Keys == null) return true;
            if (Keys == null || other.Keys == null) return false;

            return Keys.All(x => other.Keys.Any(s => s.Equals(x, StringComparison.OrdinalIgnoreCase)));
        }

        protected bool Equals(Key other)
        {
            return Equals(Keys, other.Keys);
        }

        public override int GetHashCode()
        {
            return (Keys != null ? Keys.GetHashCode() : 0);
        }

        public void AddKeyValue(string value)
        {
            Keys.Add(value);
        }

        public void AddKeyValues(IEnumerable<string> values)
        {
            foreach (var value in values)
                Keys.Add(value);
        }
    }
}
