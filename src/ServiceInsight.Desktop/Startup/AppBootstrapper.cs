﻿using System;
using System.Reflection;
using System.Windows.Threading;
using Autofac;
using Caliburn.Core.InversionOfControl;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Conventions;
using DevExpress.Xpf.Bars;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Shell;
using IContainer = Autofac.IContainer;

namespace NServiceBus.Profiler.Desktop.Startup
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Markup;

    public class AppBootstrapper : Bootstrapper<IShellViewModel>
    {
        private IContainer _container;
        
        protected override void PrepareApplication()
        {
            base.PrepareApplication();
            ExtendConventions();
            ApplyBindingCulture();
        }

        private void ApplyBindingCulture()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        private void ExtendConventions()
        {
            var convention = Container.GetInstance<IConventionManager>();
            convention.AddElementConvention(new DefaultElementConvention<BarButtonItem>("ItemClick", BarButtonItem.IsVisibleProperty, (item, o) => item.DataContext = o, item => item.DataContext));
        }

        public IContainer GetContainer()
        {
            return _container;
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = TryHandleException(e.Exception);
        }

        protected override IServiceLocator CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            _container = builder.Build();

            return new AutofacAdapter(_container);
        }

        protected virtual bool TryHandleException(Exception exception)
        {
            try
            {
                var handler = _container.Resolve<IExceptionHandler>();
                handler.Handle(exception);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
