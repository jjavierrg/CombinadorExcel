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
        }

        public string Path => FileLocation;

        public IObservableCollection<string> ColumnsNames
        {
            get
            {
                var result = new BindableCollection<string>();
                if (_excelData?.Tables[SelectedSheet]?.Columns == null) return result;

                foreach (DataColumn column in _excelData?.Tables[SelectedSheet]?.Columns)
                {
                    result.Add(column.ColumnName);
                }

                return result;
            }
        }

        public ObservableCollection<string> Sheets
        {
            get { return _sheets; }
            private set
            {
                _sheets = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedSheet
        {
            get { return _selectedSheet; }
            set
            {
                _selectedSheet = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => PreviewData);
                NotifyOfPropertyChange(() => ColumnsNames);
            }
        }

        public DataTable PreviewData => _excelData?.Tables[SelectedSheet];

        public string FileLocation
        {
            get { return _fileLocation; }
            set
            {
                _fileLocation = value;
                NotifyOfPropertyChange();
                _eventAggregator.PublishOnUIThread(Constants.FILE_LOAD);
            }
        }

        public bool IsParsed
        {
            get { return _isParsed; }
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
                    if ((sheet?.Dimension?.End?.Row ?? 0) <= 0) continue;
                    if ((sheet?.Dimension?.End?.Column ?? 0) <= 0) continue;

                    var table = new DataTable { TableName = sheet.Name };
                    var totalRows = Math.Min(sheet.Dimension.End.Row, MAX_PREVIEW_ROWS);
                    var totalColumns = Math.Min(sheet.Dimension.End.Column, Constants.MAX_COLUMNS);
                    ColumnsNames.Clear();

                    for (var columnIndex=sheet.Dimension.Start.Column; columnIndex <= totalColumns; columnIndex++)
                    {
                        var columnName = ExcelCellAddress.GetColumnLetter(columnIndex);
                        table.Columns.Add(columnName);
                        ColumnsNames.Add(columnName);
                    }

                    for (var rowIndex = sheet.Dimension.Start.Row; rowIndex <= totalRows; rowIndex++)
                    {
                        var row = table.Rows.Add();
                        for (var columnIndex = sheet.Dimension.Start.Column; columnIndex <= totalColumns; columnIndex++)
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

