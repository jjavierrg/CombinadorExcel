using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using ExcelCombinator.CoreHelpers;
using ExcelCombinator.Models.Interfaces;
using OfficeOpenXml;

namespace ExcelCombinator.Models.Core
{
    public class Parser : IParser
    {
        public IEnumerable<IRelation> Columns { get; set; }
        public IEnumerable<IRelation> KeysColumns { get; set; }
        public string FilePath { get; set; }
        public string SheetName { get; set; }
        private readonly IEventAggregator _eventAggregator;

        public Parser(IEventAggregator eventAggregator)
        {
            Columns = new ObservableCollection<IRelation>();
            KeysColumns = new ObservableCollection<IRelation>();
            _eventAggregator = eventAggregator;
        }

        protected void NotifyIsBusy(bool value, string message)
        {
            _eventAggregator.PublishOnUIThread(new BusyMessage {IsBusy = value, Message = message});
        }

        protected void NotifyException(Exception ex)
        {
            _eventAggregator.PublishOnUIThread(ex);
        }
    }
}
