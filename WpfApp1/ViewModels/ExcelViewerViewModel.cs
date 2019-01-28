using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExcelCombinator.CoreHelpers;
using ExcelCombinator.Interfaces;
using MahApps.Metro.Controls.Dialogs;
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
        private const int MAX_PREVIEW_ROWS = 20;

        public ExcelViewerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            ColumnsNames = new BindableCollection<string>();
        }

        public string Path => FileLocation;

        public IObservableCollection<string> ColumnsNames { get; }

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
                    _excelData = ParseExcel(xlPackage);
                    SelectedSheet = Sheets.FirstOrDefault();
                }

                _eventAggregator.PublishOnUIThread(new BusyMessage { IsBusy = false });
                IsParsed = _excelData != null;
            });
        }

        private DataSet ParseExcel(ExcelPackage package)
        {
            if (package == null) return null;

            try
            {
                var result = new DataSet();

                foreach (var sheet in package.Workbook.Worksheets)
                {
                    var table = new DataTable { TableName = sheet.Name };
                    var totalRows = Math.Min(sheet.Dimension.End.Row, MAX_PREVIEW_ROWS);
                    ColumnsNames.Clear();

                    for (var columnIndex=sheet.Dimension.Start.Column; columnIndex <= sheet.Dimension.End.Column; columnIndex++)
                    {
                        var columnName = ExcelCellAddress.GetColumnLetter(columnIndex);
                        table.Columns.Add(columnName);
                        ColumnsNames.Add(columnName);
                    }

                    for (var rowIndex = sheet.Dimension.Start.Row; rowIndex <= totalRows; rowIndex++)
                    {
                        var row = table.Rows.Add();
                        for (var columnIndex = sheet.Dimension.Start.Column; columnIndex <= sheet.Dimension.End.Column; columnIndex++)
                        {
                            var columnName = ExcelCellAddress.GetColumnLetter(columnIndex);
                            row[columnName] = sheet.Cells[columnName + rowIndex].Value;
                        }
                    }

                    result.Tables.Add(table);
                }

                return result;
            }
            catch (Exception ex)
            {
                ColumnsNames.Clear();
                _eventAggregator.PublishOnUIThread(ex);
                return null;
            }
        }
    }
}

