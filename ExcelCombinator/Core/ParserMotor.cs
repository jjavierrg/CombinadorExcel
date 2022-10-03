using System.Collections.Generic;
using System.Threading.Tasks;
using ExcelCombinator.Interfaces;

namespace ExcelCombinator.Core
{
    public class ParserMotor: IParseMotor
    {
        private readonly IOriginParser _originParser;
        private readonly IDestinyParser _destinyParser;

        public ParserMotor(IOriginParser originParser, IDestinyParser destinyParser)
        {
            _originParser = originParser;
            _destinyParser = destinyParser;
        }

        public async Task<bool> Parse(string originPath, string originSheet, string destinyPath, string destinySheet, IEnumerable<IRelation> columns, IEnumerable<IRelation> keys)
        {
            return await Task.Run<bool>(() =>
            {
                _originParser.FilePath = originPath;
                _originParser.SheetName = originSheet;
                _originParser.Columns = columns;
                _originParser.KeysColumns = keys;

                if (!_originParser.Parse()) return false;

                _destinyParser.FilePath = destinyPath;
                _destinyParser.SheetName = destinySheet;
                _destinyParser.Columns = columns;
                _destinyParser.KeysColumns = keys;
                return _destinyParser.Process(_originParser.Values);
            });
        }
    }
}
