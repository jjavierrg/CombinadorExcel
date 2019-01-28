using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExcelCombinator.Models.Interfaces;
using OfficeOpenXml;

namespace ExcelCombinator.Models.Core
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

        public async Task<bool> Parse(string originPath, string destinyPath, IEnumerable<IRelation> columns, IEnumerable<IRelation> keys)
        {
            return await Task.Run<bool>(() =>
            {
                _originParser.FilePath = originPath;
                _originParser.Columns = columns;
                _originParser.KeysColumns = keys;

                if (!_originParser.Parse()) return false;

                _destinyParser.FilePath = originPath;
                _destinyParser.Columns = columns;
                _destinyParser.KeysColumns = keys;
                return _destinyParser.Process(_originParser.Values);
            });
        }
    }
}
