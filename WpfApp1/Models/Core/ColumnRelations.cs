using ExcelCombinator.Models.Interfaces;

namespace ExcelCombinator.Models.Core
{
    public class ColumnRelations: IRelation
    {
        public string Origin { get; set; }
        public string Destiny{ get; set; }
    }
}
