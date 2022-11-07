using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AutoUpdaterDotNET;
using Caliburn.Micro;
using ExcelCombinator.Core;
using ExcelCombinator.CoreHelpers;
using ExcelCombinator.Interfaces;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Brush = System.Drawing.Brush;

namespace ExcelCombinator.ViewModels
{
    public class AccentColorMenuData
    {
        public string Name => CurrentAccent?.Name;
        public SolidColorBrush ColorBrush => (SolidColorBrush)CurrentAccent?.Resources["AccentColorBrush"];
        public Accent CurrentAccent { get; set; }

        public void ChangeAccent()
        {
            ThemeManager.ChangeAppStyle(Application.Current, CurrentAccent, ThemeManager.GetAppTheme("BaseLight"));
            Properties.Settings.Default.Theme = CurrentAccent?.Name ?? "";
            Properties.Settings.Default.Save();
        }
    }

    public class ShellViewModel : Screen, IShell, IHandle<BusyMessage>, IHandle<Exception>, IHandle<string>
    {
        private bool _isBusy;
        private string _busyText;
        private string _originColumn;
        private string _destinyColumn;
        private readonly IEventAggregator _eventAggregator;
        private readonly IParseMotor _motor;
        private bool _normalizeKeys = true;


        public ShellViewModel(IExcelViewer originExcelViewerVm, IExcelViewer destinyExcelViewerVm, IEventAggregator eventAggregator, IParseMotor motor)
        {
            OriginExcelViewerVm = originExcelViewerVm;
            DestinyExcelViewerVm = destinyExcelViewerVm;
            _eventAggregator = eventAggregator;
            _motor = motor;
            _eventAggregator.Subscribe(this);

            KeyRelations = new BindableCollection<IRelation>();
            ColumnsRelations = new BindableCollection<IRelation>();

            AccentColors = ThemeManager.Accents
                .Select(a => new AccentColorMenuData { CurrentAccent = a })
                .ToList();

            WindowCaption = GetWindowTitle();

            CheckForUpdate();
        }

        public IList<AccentColorMenuData> AccentColors { get; }

        public string WindowCaption { get; }
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange();
            }
        }
        public string BusyText
        {
            get { return _busyText; }
            set
            {
                _busyText = value;
                NotifyOfPropertyChange();
            }
        }
        public string OriginColumn
        {
            get { return _originColumn; }
            set
            {
                _originColumn = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanAddRelation);
            }
        }
        public string DestinyColumn
        {
            get { return _destinyColumn; }
            set
            {
                _destinyColumn = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanAddRelation);
            }
        }
        public bool NormalizeKeys
        {
            get { return _normalizeKeys; }
            set
            {
                _normalizeKeys = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CanAddRelation => !string.IsNullOrEmpty(OriginColumn) && !string.IsNullOrEmpty(DestinyColumn);
        public bool CanParse
        {
            get
            {
                if (KeyRelations == null || !KeyRelations.Any()) return false;
                if (ColumnsRelations == null || !ColumnsRelations.Any()) return false;
                if (string.IsNullOrEmpty(OriginExcelViewerVm?.Path)) return false;
                if (string.IsNullOrEmpty(DestinyExcelViewerVm?.Path)) return false;

                return true;
            }
        }

        public IParserOptions OriginParserOptions { get; set; } = new ParserOptions { NormalizeFields = true };

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

        public void Handle(string value)
        {
            if (value == Constants.FILE_LOAD)
                NotifyOfPropertyChange(() => CanParse);
        }

        public void AddRelation()
        {
            if (string.IsNullOrEmpty(OriginColumn)) return;
            if (string.IsNullOrEmpty(DestinyColumn)) return;

            var relation = IoC.Get<IRelation>(Constants.SUBSTITUTION_COLUMN_RELATION_KEY);
            relation.Origin = OriginColumn;
            relation.Destiny = DestinyColumn;

            if (!ColumnsRelations.Contains(relation))
                ColumnsRelations.Add(relation);

            NotifyOfPropertyChange(() => CanParse);
        }
        public void AddKey()
        {
            if (string.IsNullOrEmpty(OriginColumn)) return;
            if (string.IsNullOrEmpty(DestinyColumn)) return;

            var relation = IoC.Get<IRelation>(Constants.KEY_COLUMN_RELATION_KEY);
            relation.Origin = OriginColumn;
            relation.Destiny = DestinyColumn;

            if (!KeyRelations.Contains(relation))
                KeyRelations.Add(relation);

            NotifyOfPropertyChange(() => CanParse);
        }

        public void FixCsv()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Csv Files (*.csv) | *.csv";
            if (openFileDialog.ShowDialog() != true)
                return;

            var utf8WithBOM = new System.Text.UTF8Encoding(true);

            try
            {
                foreach (var file in openFileDialog.FileNames)
                {
                    var content = File.ReadAllLines(file);
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.WriteAllLines(file, content, utf8WithBOM);
                }

                MessageBox.Show("Ficheros procesados correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar los ficheros: " + ex.Message);
            }
        }

        public async Task Parse()
        {
            var result = await _motor.Parse(OriginExcelViewerVm.Path, OriginExcelViewerVm.SelectedSheet, DestinyExcelViewerVm.Path, DestinyExcelViewerVm.SelectedSheet, ColumnsRelations, KeyRelations, OriginParserOptions);
            if (!result)
                return;

            var dialogOptions = new MetroDialogSettings
            {
                NegativeButtonText = "Aceptar",
                AffirmativeButtonText = "Abrir excel destino",
                DefaultButtonFocus = MessageDialogResult.Affirmative
            };

            var openFile = DialogCoordinator.Instance.ShowModalMessageExternal(this, "Proceso Completado", "Proceso completado con éxito.", MessageDialogStyle.AffirmativeAndNegative, dialogOptions);
            if (openFile == MessageDialogResult.Affirmative)
                DestinyExcelViewerVm.OpenFile();
        }

        public void DeleteRelation(IRelation item)
        {
            if (item == null) return;

            if (item is KeyColumn && KeyRelations.Contains(item))
                KeyRelations.Remove(item);
            else if (item is SubstitutionColumn && ColumnsRelations.Contains(item))
                ColumnsRelations.Remove(item);

            NotifyOfPropertyChange(() => CanParse);
        }

        public void clearAllKeys()
        {
            KeyRelations.Clear();
            NotifyOfPropertyChange(() => CanParse);
        }

        public void ClearAllSubstitutions()
        {
            ColumnsRelations.Clear();
            NotifyOfPropertyChange(() => CanParse);
        }

        private string GetWindowTitle()
        {
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return $"{name} - v:{version}";
        }

        private void CheckForUpdate()
        {
            AutoUpdater.ClearAppDirectory = true;
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.Start("https://raw.githubusercontent.com/Jjavierrg/CombinadorExcel/master/update.xml");
        }
    }
}
