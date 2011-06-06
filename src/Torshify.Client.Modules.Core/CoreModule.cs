using System;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

using Torshify.Client.Infrastructure;
using Torshify.Client.Modules.Core.Views;
using Torshify.Client.Modules.Core.Views.Playlists;

namespace Torshify.Client.Modules.Core
{
    public class CoreModule : IModule
    {
        #region Fields

        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public CoreModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        #endregion Constructors

        #region Public Methods

        public void Initialize()
        {
            _container.RegisterType<MainView>("MainView");
            _container.RegisterType<PlaylistsView>(MusicRegionViewNames.PlaylistView);

            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MainView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.LeftMusicRegion, typeof(PlaylistsView));

            _regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("MainView", UriKind.Relative));
        }

        #endregion Public Methods
    }
}