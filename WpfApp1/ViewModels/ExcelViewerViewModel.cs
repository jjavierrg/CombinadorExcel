using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using EPPlus.Extensions;
using ExcelCombinator.CoreHelpers;
using ExcelCombinator.Models.Interfaces;
using Microsoft.Win32;
using OfficeOpenXml;

namespace ExcelCombinator.ViewModels
{
    public class ExcelViewerViewModel : PropertyChangedBase, IExcelViewer
    {
        private string _fileLocation;
        private ObservableCollection<string> _sheets;
        private readonly IEventAggregator _eventAggregator;
        private DataSet _excelData;
        private string _selectedSheet;
        private bool _isParsed;

        public ExcelViewerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public ObservableCollection<string> Sheets
        {
            get => _sheets;
            private set
            {
                _sheets = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedSheet
        {
            get => _selectedSheet;
            set
            {
                _selectedSheet = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => PreviewData);
            }
        }

        public DataTable PreviewData => _excelData?.Tables[SelectedSheet];

        public string FileLocation
        {
            get => _fileLocation;
            set
            {
                _fileLocation = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsParsed
        {
            get => _isParsed;
            set
            {
                _isParsed = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CanOpenFile => true;

        public async Task OpenFile()
        {
            var ofd = new OpenFileDialog { Filter = "Ficheros excel (*.xlsx)|*.xlsx" };
            var result = ofd.ShowDialog();

            if (!result.HasValue || !result.Value) return;
            FileLocation = ofd.FileName;

            _eventAggregator.PublishOnUIThread(new BusyMessage {IsBusy = true, Message = "Leyendo fichero"});

            await Task.Run(() =>
            {
                using (var xlPackage = new ExcelPackage(new FileInfo(FileLocation)))
                {
                    Sheets = new ObservableCollection<string>(xlPackage.Workbook.Worksheets.Select(x => x.Name));
                    _excelData = xlPackage.ToDataSet(false);
                    SelectedSheet = Sheets.FirstOrDefault();
                }

                _eventAggregator.PublishOnUIThread(new BusyMessage { IsBusy = false });
                IsParsed = true;
            });
        }
    }
}

