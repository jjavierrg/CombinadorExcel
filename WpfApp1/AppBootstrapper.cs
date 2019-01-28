using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using ExcelCombinator.Models.Core;
using ExcelCombinator.Models.Interfaces;
using ExcelCombinator.ViewModels;
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
            DisplayRootViewFor<IShell>();
        }
    }
}
