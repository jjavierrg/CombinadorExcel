using System.Collections.Generic;

namespace ExcelCombinator.Interfaces
{
    public interface IParser
    {
        IEnumerable<IRelation> Columns { get; set; }
        IEnumerable<IRelation> KeysColumns { get; set; }
        string FilePath { get; set; }
        string SheetName { get; set; }
        IParserOptions ParseOptions { get; set; }
    }
}