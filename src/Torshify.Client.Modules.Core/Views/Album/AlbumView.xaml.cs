﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

using Microsoft.Practices.Prism.Events;

using Torshify.Client.Infrastructure.Controls;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.Album
{
    public partial class AlbumView : UserControl
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion Fields

        #region Constructors

        public AlbumView(AlbumViewModel viewModel, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public AlbumViewModel Model
        {
            get { return DataContext as AlbumViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void DataGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            var dg = (FrameworkElement)sender;

            if (element != null && element.DataContext is ITrack)
            {
                var track = (ITrack)element.DataContext;
                var commandbar = new CommandBar();

                _eventAggregator.GetEvent<TrackCommandBarEvent>().Publish(new TrackCommandBarModel(track, commandbar));

                dg.ContextMenu = new CommandBarContextMenu
                {
                    ItemsSource = commandbar.ChildMenuItems
                };
            }
        }

        #endregion Methods
    }

    public class NavigateToStringBehavior : Behavior<WebBrowser>
    {
        #region Fields

        public static readonly DependencyProperty NavigateToHtmlStringProperty = 
            DependencyProperty.Register("NavigateToHtmlString", typeof(string), typeof(NavigateToStringBehavior),
                new FrameworkPropertyMetadata((string)string.Empty,
                    new PropertyChangedCallback(OnNavigateToHtmlStringChanged)));

        #endregion Fields

        #region Properties

        public string NavigateToHtmlString
        {
            get { return (string)GetValue(NavigateToHtmlStringProperty); }
            set { SetValue(NavigateToHtmlStringProperty, value); }
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null && !string.IsNullOrEmpty(NavigateToHtmlString))
            {
                AssociatedObject.NavigateToString(NavigateToHtmlString);
            }
        }

        protected virtual void OnNavigateToHtmlStringChanged(string oldNavigateToHtmlString, string newNavigateToHtmlString)
        {
            if (AssociatedObject != null && !string.IsNullOrEmpty(newNavigateToHtmlString))
            {
                AssociatedObject.NavigateToString(newNavigateToHtmlString);
            }
        }

        private static void OnNavigateToHtmlStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigateToStringBehavior target = (NavigateToStringBehavior)d;
            string oldNavigateToHtmlString = (string)e.OldValue;
            string newNavigateToHtmlString = target.NavigateToHtmlString;
            target.OnNavigateToHtmlStringChanged(oldNavigateToHtmlString, newNavigateToHtmlString);
        }

        #endregion Methods
    }
}