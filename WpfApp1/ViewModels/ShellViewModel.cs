using System.Collections.ObjectModel;
using Caliburn.Micro;
using ExcelCombinator.CoreHelpers;
using ExcelCombinator.Models.Interfaces;

namespace ExcelCombinator.ViewModels
{
    public class ShellViewModel : Screen, IShell, IHandle<BusyMessage>
    {
        private bool _isBusy;
        private string _busyText;
        private string _originColumn;
        private string _destinyColumn;
        private readonly IEventAggregator _eventAggregator;
        private IRelation _selectedColumnRelation;
        private IRelation _selectedKeyRelation;

        public ShellViewModel(IExcelViewer originExcelViewerVm, IExcelViewer destinyExcelViewerVm, IEventAggregator eventAggregator)
        {
            OriginExcelViewerVm = originExcelViewerVm;
            DestinyExcelViewerVm = destinyExcelViewerVm;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            KeyRelations = new BindableCollection<IRelation>();
            ColumnsRelations = new BindableCollection<IRelation>();
            OriginColumns = new BindableCollection<string>();
            DestinyColumns = new BindableCollection<string>();
        }

        public string WindowCaption => "Combinador de excels";
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange();
            }
        }
        public string BusyText
        {
            get => _busyText;
            set
            {
                _busyText = value;
                NotifyOfPropertyChange();
            }
        }
        public string OriginColumn
        {
            get => _originColumn;
            set
            {
                _originColumn = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanAddFilter);
            }
        }
        public string DestinyColumn
        {
            get => _destinyColumn;
            set
            {
                _destinyColumn = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanAddFilter);
            }
        }

        public IRelation SelectedColumnRelation
        {
            get => _selectedColumnRelation;
            set
            {
                _selectedColumnRelation = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanRemoveColumnRelation);
            }
        }
        public IRelation SelectedKeyRelation
        {
            get => _selectedKeyRelation;
            set
            {
                _selectedKeyRelation = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanRemoveKeyRelation);
            }
        }

        public bool CanAddFilter => !string.IsNullOrEmpty(OriginColumn) && !string.IsNullOrEmpty(DestinyColumn);
        public bool CanRemoveColumnRelation => !string.IsNullOrEmpty(OriginColumn) && !string.IsNullOrEmpty(DestinyColumn);
        public bool CanRemoveKeyRelation => !string.IsNullOrEmpty(OriginColumn) && !string.IsNullOrEmpty(DestinyColumn);

        public IExcelViewer OriginExcelViewerVm { get; }
        public IExcelViewer DestinyExcelViewerVm { get; }
        public IObservableCollection<IRelation> KeyRelations { get; }
        public IObservableCollection<IRelation> ColumnsRelations { get; }
        public IObservableCollection<string> OriginColumns { get; }
        public IObservableCollection<string> DestinyColumns { get; }

        public void Handle(BusyMessage message)
        {
            IsBusy = message.IsBusy;
            BusyText = message.Message;
        }

        void AddRelation()
        {
            if (string.IsNullOrEmpty(OriginColumn)) return;
            if (string.IsNullOrEmpty(DestinyColumn)) return;

            var relation = IoC.Get<IRelation>();
            relation.Origin = OriginColumn;
            relation.Destiny = DestinyColumn;
            ColumnsRelations.Add(relation);
        }
        void AddKey()
        {
            if (string.IsNullOrEmpty(OriginColumn)) return;
            if (string.IsNullOrEmpty(DestinyColumn)) return;

            var relation = IoC.Get<IRelation>();
            relation.Origin = OriginColumn;
            relation.Destiny = DestinyColumn;
            KeyRelations.Add(relation);
        }
        void RemoveColumnRelation()
        {
            if (SelectedColumnRelation == null) return;
            if (!ColumnsRelations.Contains(SelectedColumnRelation)) return;

            ColumnsRelations.Remove(SelectedColumnRelation);
        }
        void RemoveKeyRelation()
        {
            if (SelectedKeyRelation ==  null) return;
            if (!KeyRelations.Contains(SelectedKeyRelation)) return;

            KeyRelations.Remove(SelectedKeyRelation);
        }
    }
}
