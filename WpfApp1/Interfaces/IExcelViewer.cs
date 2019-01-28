using Caliburn.Micro;

namespace ExcelCombinator.Interfaces
{
    public interface IExcelViewer
    {
        string Path { get; }
        string SelectedSheet { get; }
        IObservableCollection<string> ColumnsNames { get; }
    }
}