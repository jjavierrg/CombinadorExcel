using Caliburn.Micro;

namespace ExcelCombinator.Models.Interfaces
{
    public interface IExcelViewer
    {
        string Path { get; }
        IObservableCollection<string> ColumnsNames { get; }
    }
}