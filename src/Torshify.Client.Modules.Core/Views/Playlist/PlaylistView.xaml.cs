﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

using Microsoft.Practices.Prism.Events;

using Torshify.Client.Infrastructure.Controls;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.Playlist
{
    public class DataGridRowActionBehavior : Behavior<DataGrid>
    {
        #region Fields

        public static readonly DependencyProperty CommandParameterProperty = 
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(DataGridRowActionBehavior),
                new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DataGridRowActionBehavior),
                new FrameworkPropertyMetadata((ICommand)null));

        #endregion Fields

        #region Properties

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.MouseDoubleClick += OnMouseDoubleClick;
            AssociatedObject.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDoubleClick -= OnMouseDoubleClick;
            AssociatedObject.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnKeyDown));
            base.OnDetaching();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var element = e.OriginalSource as DependencyObject;

                if (element != null)
                {
                    var row = UIHelpers.FindVisualAncestorByType<DataGridRow>(element);

                    if (row != null && Command != null && Command.CanExecute(CommandParameter))
                    {
                        Command.Execute(CommandParameter);
                        e.Handled = true;
                    }
                }
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as DependencyObject;

            if (element != null)
            {
                var row = UIHelpers.FindVisualAncestorByType<DataGridRow>(element);

                if (row != null && Command != null && Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

        #endregion Methods
    }

    public partial class PlaylistView : UserControl
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion Fields

        #region Constructors

        public PlaylistView(PlaylistViewModel viewModel, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public PlaylistViewModel Model
        {
            get { return DataContext as PlaylistViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void DataGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            var dg = (MultiSelector)sender;

            if (element != null && element.DataContext is ITrack)
            {
                var commandbar = new CommandBar();

                if (dg.SelectedItems.Count == 1)
                {
                    var track = (ITrack)dg.SelectedItems[0];
                    _eventAggregator.GetEvent<TrackCommandBarEvent>().Publish(new TrackCommandBarModel(track, commandbar));
                }
                else if (dg.SelectedItems.Count > 1)
                {
                    var tracks = dg.SelectedItems.Cast<ITrack>();
                    _eventAggregator.GetEvent<TracksCommandBarEvent>().Publish(new TracksCommandBarModel(tracks, commandbar));
                }

                dg.ContextMenu = new CommandBarContextMenu
                {
                    ItemsSource = commandbar.ChildMenuItems
                };
            }
        }

        #endregion Methods
    }
}