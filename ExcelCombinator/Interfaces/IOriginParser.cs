using ExcelCombinator.Core;
using System.Collections.Generic;

namespace ExcelCombinator.Interfaces
{
    public interface IOriginParser: IParser
    {
        bool Parse();
        IDictionary<IKey, IList<IRelationEntry>> Values { get; }
    }
}