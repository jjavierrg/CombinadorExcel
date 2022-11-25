using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Caliburn.Micro;
using ExcelCombinator.CoreHelpers;
using ExcelCombinator.Interfaces;
using OfficeOpenXml;

namespace ExcelCombinator.Core
{

    public class ParserOptions : IParserOptions
    {
        public bool NormalizeFields { get; set; }
        public bool RequireAllKeys { get; set; }
        public bool ClearColumnIfNullMatch { get; set; }
    }

    public class Parser : IParser
    {
        public IEnumerable<IRelation> Columns { get; set; }
        public IEnumerable<IRelation> KeysColumns { get; set; }
        public string FilePath { get; set; }
        public string SheetName { get; set; }

        public IParserOptions ParseOptions { get; set; }

        private readonly IEventAggregator _eventAggregator;
        protected readonly INormalizer _normalizer;

        public Parser(IEventAggregator eventAggregator, INormalizer normalizer)
        {
            Columns = new ObservableCollection<IRelation>();
            KeysColumns = new ObservableCollection<IRelation>();
            _eventAggregator = eventAggregator;
            _normalizer = normalizer;
        }

        protected IRelationEntry ExtractRelationEntry(ExcelWorksheet excelWorksheet, IRelation relation, int rowNum, bool extractFromOriginColumn = true)
        {
            var keyEntry = IoC.Get<IRelationEntry>();
            keyEntry.OriginColumn = relation.Origin;
            keyEntry.DestinyColumn = relation.Destiny;

            var column = extractFromOriginColumn ? relation.Origin : relation.Destiny;

            var keyValue = excelWorksheet.Cells[column + rowNum].GetValue<string>();

            if (ParseOptions.NormalizeFields)
                keyValue = _normalizer.Normalize(keyValue);

            keyEntry.Value = keyValue;
            return keyEntry;
        }

        protected void NotifyIsBusy(bool value, string message)
        {
            _eventAggregator.PublishOnUIThread(new BusyMessage { IsBusy = value, Message = message });
        }

        protected void NotifyException(Exception ex)
        {
            _eventAggregator.PublishOnUIThread(ex);
        }
    }
}
