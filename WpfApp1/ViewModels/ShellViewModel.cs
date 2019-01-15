using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using WpfApp1.Interfaces;

namespace WpfApp1.ViewModels
{
    public class ShellViewModel : Screen, IShell
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange();
            }
        }

        public ShellViewModel(IExcelViewer originExcelViewerVm, IExcelViewer destinyExcelViewerVm)
        {
            OriginExcelViewerVm = originExcelViewerVm;
            DestinyExcelViewerVm = destinyExcelViewerVm;
        }

        public string WindowCaption => "Combinador Excel";

        public IExcelViewer OriginExcelViewerVm { get; private set; }
        public IExcelViewer DestinyExcelViewerVm { get; private set; }
    }
}
