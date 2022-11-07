using OfficeOpenXml.FormulaParsing.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExcelCombinator.Interfaces
{
    public interface IParserOptions
    {
        bool NormalizeFields { get; }
    }

    public interface IParseMotor
    {
        Task<bool> Parse(string originPath, string originSheet, string destinyPath, string destinySheet, IEnumerable<IRelation> columns, IEnumerable<IRelation> keys, IParserOptions options);
    }
}