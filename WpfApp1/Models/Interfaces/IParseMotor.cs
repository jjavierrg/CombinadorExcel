using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ExcelCombinator.Models.Interfaces
{
    public interface IParseMotor
    {
        Task<bool> Parse(string originPath, string destinyPath, IEnumerable<IRelation> columns, IEnumerable<IRelation> keys);
    }
}