﻿using System;
using ExcelCombinator.Interfaces;

namespace ExcelCombinator.Core
{
    public class ColumnRelations: IRelation
    {
        public string Origin { get; set; }
        public string Destiny{ get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as ColumnRelations;
            if (other == null) return false;

            if (!Origin.Equals(other.Origin, StringComparison.OrdinalIgnoreCase)) return false;
            if (!Destiny.Equals(other.Destiny, StringComparison.OrdinalIgnoreCase)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Origin.GetHashCode();
                hash = hash * 23 + Destiny.GetHashCode();
                return hash;
            }
        }
    }

    public class KeyColumn : ColumnRelations { }
    public class SubstitutionColumn : ColumnRelations { }
}
