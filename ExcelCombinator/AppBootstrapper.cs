using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using ExcelCombinator.Core;
using ExcelCombinator.CoreHelpers;
using ExcelCombinator.Interfaces;
using ExcelCombinator.ViewModels;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;

namespace ExcelCombinator
{
    public class AppBootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();

            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            _container.PerRequest<IShell, ShellViewModel>();
            _container.PerRequest<IExcelViewer, ExcelViewerViewModel>();
            _container.PerRequest<IRelation, ColumnRelations>();
            _container.PerRequest<IRelation, KeyColumn>(Constants.KEY_COLUMN_RELATION_KEY);
            _container.PerRequest<IRelation, SubstitutionColumn>(Constants.SUBSTITUTION_COLUMN_RELATION_KEY);

            _container.Singleton<IParseMotor, ParserMotor>();
            _container.PerRequest<IOriginParser, OriginParser>();
            _container.PerRequest<IDestinyParser, DestinyParser>();
            _container.PerRequest<INormalizer, Normalizer>();
            _container.PerRequest<IRelationEntry, RelationEntry>();

            _container.PerRequest<IValueEntryFinder, AndValueEntryFinder>(Constants.AND_COMPARER);
            _container.PerRequest<IValueEntryFinder, OrValueEntryFinder>(Constants.OR_COMPARER);
            _container.PerRequest<IKey, Key>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var accent = Properties.Settings.Default.Theme;

            if (string.IsNullOrEmpty(accent))
                accent = "Cobalt";

            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(accent), ThemeManager.GetAppTheme("BaseLight"));
            DisplayRootViewFor<IShell>();
        }
    }
}
