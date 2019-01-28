using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ExcelCombinator.CoreHelpers;
using ExcelCombinator.Models.Core;
using ExcelCombinator.Models.Interfaces;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using Brush = System.Drawing.Brush;

namespace ExcelCombinator.ViewModels
{
    public class AccentColorMenuData
    {
        public string Name => CurrentAccent?.Name;
        public SolidColorBrush ColorBrush => (SolidColorBrush) CurrentAccent?.Resources["AccentColorBrush"];
        public Accent CurrentAccent { get; set; }

        public void ChangeAccent()
        {
            ThemeManager.ChangeAppStyle(Application.Current, CurrentAccent, ThemeManager.GetAppTheme("BaseLight"));
        }
    }
    public class ShellViewModel : Screen, IShell, IHandle<BusyMessage>, IHandle<Exception>
    {
        private bool _isBusy;
        private string _busyText;
        private string _originColumn;
        private string _destinyColumn;
        private readonly IEventAggregator _eventAggregator;
        private readonly IParseMotor _motor;

        public ShellViewModel(IExcelViewer originExcelViewerVm, IExcelViewer destinyExcelViewerVm, IEventAggregator eventAggregator, IParseMotor motor)
        {
            OriginExcelViewerVm = originExcelViewerVm;
            DestinyExcelViewerVm = destinyExcelViewerVm;
            _eventAggregator = eventAggregator;
            _motor = motor;
            _eventAggregator.Subscribe(this);

            KeyRelations = new BindableCollection<IRelation>();
            ColumnsRelations = new BindableCollection<IRelation>();

            this.AccentColors = ThemeManager.Accents
                .Select(a => new AccentColorMenuData {CurrentAccent = a})
                .ToList();
        }

        public IList<AccentColorMenuData> AccentColors { get; }

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
                NotifyOfPropertyChange(() => CanAddRelation);
            }
        }
        public string DestinyColumn
        {
            get => _destinyColumn;
            set
            {
                _destinyColumn = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanAddRelation);
            }
        }

        public bool CanAddRelation=> !string.IsNullOrEmpty(OriginColumn) && !string.IsNullOrEmpty(DestinyColumn);

        public IExcelViewer OriginExcelViewerVm { get; }
        public IExcelViewer DestinyExcelViewerVm { get; }
        public IObservableCollection<IRelation> KeyRelations { get; }
        public IObservableCollection<IRelation> ColumnsRelations { get; }

        public void Handle(BusyMessage message)
        {
            IsBusy = message.IsBusy;
            BusyText = message.Message;
        }

        public void Handle(Exception ex)
        {
            DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", ex.Message);
        }

        public void AddRelation()
        {
            if (string.IsNullOrEmpty(OriginColumn)) return;
            if (string.IsNullOrEmpty(DestinyColumn)) return;

            var relation = IoC.Get<IRelation>();
            relation.Origin = OriginColumn;
            relation.Destiny = DestinyColumn;

            if (!ColumnsRelations.Contains(relation))
                ColumnsRelations.Add(relation);
        }
        public void AddKey()
        {
            if (string.IsNullOrEmpty(OriginColumn)) return;
            if (string.IsNullOrEmpty(DestinyColumn)) return;

            var relation = IoC.Get<IRelation>();
            relation.Origin = OriginColumn;
            relation.Destiny = DestinyColumn;

            if (!KeyRelations.Contains(relation))
                KeyRelations.Add(relation);
        }

        public async Task Parse()
        {
            var result = await _motor.Parse(OriginExcelViewerVm.Path, DestinyExcelViewerVm.Path, ColumnsRelations, KeyRelations);
            if (result)
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Proceso Completado", "Proceso completado con éxito");
        }
    }
}
