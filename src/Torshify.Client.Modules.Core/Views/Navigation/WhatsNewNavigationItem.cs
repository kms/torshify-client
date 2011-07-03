using System;
using System.Windows;

using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class WhatsNewNavigationItem : INavigationItem
    {
        #region Fields

        private IRegionManager _regionManager;
        private Uri _uri;

        #endregion Fields

        #region Constructors

        public WhatsNewNavigationItem(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _uri = new Uri(MusicRegionViewNames.WhatsNew, UriKind.Relative);
        }

        #endregion Constructors

        #region Properties

        public DataTemplate DataTemplate
        {
            get
            {
                return NavigationItemTemplates.Instance[GetType()] as DataTemplate;
            }
        }

        #endregion Properties

        #region Methods

        public bool IsMe(IRegionNavigationJournalEntry entry)
        {
            return entry.Uri == _uri;
        }

        public void NavigateTo()
        {
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, _uri);
        }

        #endregion Methods
    }
}