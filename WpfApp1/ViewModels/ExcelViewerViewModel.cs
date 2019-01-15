using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Win32;
using OfficeOpenXml;
using WpfApp1.Interfaces;
using Xceed.Wpf.Toolkit;

namespace WpfApp1.ViewModels
{
    [Export(typeof(IExcelViewer))]
    public class ExcelViewerViewModel : PropertyChangedBase, IExcelViewer
    {
        private bool _isBusy;
        private string _fileLocation;
        private ObservableCollection<string> _sheets;

        public ObservableCollection<string> Sheets
        {
            get => _sheets;
            private set
            {
                _sheets = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange();
            }
        }

        public string FileLocation
        {
            get => _fileLocation;
            set
            {
                _fileLocation = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CanOpenFile => true;

        public void OpenFile()
        {
            var ofd = new OpenFileDialog { Filter = "Ficheros excel (*.xlsx)|*.xlsx" };
            var result = ofd.ShowDialog();

            if (!result.HasValue || !result.Value) return;
            FileLocation = ofd.FileName;

            using (var xlPackage = new ExcelPackage(new FileInfo(FileLocation)))
                Sheets = new ObservableCollection<string>(xlPackage.Workbook.Worksheets.Select(x => x.Name));
        }
    }
}
