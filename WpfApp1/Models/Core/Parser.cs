using System.Collections.Generic;
using System.Collections.ObjectModel;
using ExcelCombinator.Models.Interfaces;

namespace ExcelCombinator.Models.Core
{
    public class Parser : IParser
    {
        public IEnumerable<IRelation> Columns { get; set; }
        public IEnumerable<IRelation> KeysColumns { get; set; }
        public string SheetName { get; set; }

        public Parser()
        {
            Columns = new ObservableCollection<IRelation>();
            KeysColumns = new ObservableCollection<IRelation>();
        }
    }
}
