using System;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Services;
using Torshify.Client.Mocks;
using Torshify.Client.Modules.Core;
using Torshify.Client.Spotify;
using Torshify.Client.Unity;

namespace Torshify.Client
{
    public class Bootstrapper : UnityBootstrapper
    {
        #region Public Methods

        public void Dispose()
        {
            Container.Dispose();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void ConfigureContainer()
        {
            Container.InstallCoreExtensions();
            Container.RegisterStartable<InactivityNotificator, InactivityNotificator>();
            Container.RegisterInstance(typeof(Dispatcher), null, Application.Current.Dispatcher, new ContainerControlledLifetimeManager());
            Container.RegisterType<IBackdropService, BackdropService>(new ContainerControlledLifetimeManager(), new InjectionProperty("CacheLocation", AppConstants.BackdropCacheFolder));

            base.ConfigureContainer();
        }

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void InitializeModules()
        {
            InitializeMef();
            base.InitializeModules();
            InitializeStartables();
        }

        protected override void ConfigureModuleCatalog()
        {

#if !MockEnabled
            Type spotifyModule = typeof(SpotifyModule);
            ModuleCatalog.AddModule(new ModuleInfo(spotifyModule.Name,
                                                   spotifyModule.AssemblyQualifiedName));
            Type coreModule = typeof(CoreModule);
            ModuleCatalog.AddModule(new ModuleInfo(coreModule.Name,
                                                   coreModule.AssemblyQualifiedName,
                                                   spotifyModule.Name));
#else
            Type mockModule = typeof(MockModule);
            ModuleCatalog.AddModule(new ModuleInfo(mockModule.Name,
                                                   mockModule.AssemblyQualifiedName));

            Type coreModule = typeof (CoreModule);
            ModuleCatalog.AddModule(new ModuleInfo(coreModule.Name,
                                                   coreModule.AssemblyQualifiedName,
                                                   mockModule.Name));
#endif
        }

        #endregion Protected Methods

        #region Private Methods

        private void InitializeMef()
        {
            AggregateCatalog aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(Environment.CurrentDirectory, "Torshify.*.dll"));
            CompositionContainer container = new CompositionContainer(aggregateCatalog);
            Container.RegisterInstance(container);
        }

        private void InitializeStartables()
        {
            var startables = Container.ResolveAll<IStartable>();

            foreach (var startable in startables)
            {
                startable.Start();
            }
        }

        #endregion Private Methods
    }
}