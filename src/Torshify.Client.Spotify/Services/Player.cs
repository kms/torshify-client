using System;
using System.Timers;
using System.Windows.Threading;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Spotify.Services
{
    public class Player : NotificationObject, IPlayer
    {
        #region Fields

        private readonly ISession _session;

        private BassPlayer _bass;
        private bool _isPlaying;
        private IPlayerQueue _playlist;
        private Error? _lastLoadStatus;
        private Timer _timer;

        private TimeSpan _playLocation = TimeSpan.Zero;

        #endregion Fields

        #region Constructors

        public Player(ISession session, Dispatcher dispatcher)
        {
            _bass = new BassPlayer();

            _session = session;
            _session.MusicDeliver += OnSessionMusicDeliver;
            _session.PlayTokenLost += OnSessionPlayerTokenLost;
            _session.StopPlayback += OnSessionStopPlayback;
            _session.StartPlayback += OnSessionStartPlayback;

            _playlist = new PlayerQueue(dispatcher);
            _playlist.CurrentChanged += OnCurrentChanged;

            _timer = new Timer(100);
            _timer.Elapsed += OnTimerElapsed;
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsPlayingChanged;

        #endregion Events

        #region Properties

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;

                    if (_isPlaying)
                    {
                        _timer.Start();
                    }
                    else
                    {
                        _timer.Stop();
                    }

                    OnIsPlayingChanged();
                }
            }
        }

        public TimeSpan DurationPlayed
        {
            get
            {
                return _playLocation;
            }
            set
            {
                Seek(value);
            }
        }

        public IPlayerQueue Playlist
        {
            get { return _playlist; }
        }

        #endregion Properties

        #region Public Methods

        public void Pause()
        {
            if (_lastLoadStatus == Error.OK)
            {
                _session.PlayerPause();
                IsPlaying = false;
            }
        }

        public void Play()
        {
            if (!_lastLoadStatus.HasValue)
            {
                if (_playlist.Current == null)
                {
                    _playlist.Next();
                }

                if (_playlist.Current != null)
                {
                    var track = (Track)Playlist.Current.Track;
                    _lastLoadStatus = track.InternalTrack.Load();
                }
            }

            if (_lastLoadStatus.HasValue && _lastLoadStatus == Error.OK)
            {
                _session.PlayerPlay();
                IsPlaying = true;
            }
        }

        public void Seek(TimeSpan timeSpan)
        {
            if (_isPlaying)
            {
                _session.PlayerSeek(timeSpan);
                _playLocation = timeSpan;
                RaisePropertyChanged("DurationPlayed");
            }
        }

        public void Stop()
        {
            _session.PlayerUnload();
            IsPlaying = false;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnIsPlayingChanged()
        {
            var handler = IsPlayingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnSessionStartPlayback(object sender, SessionEventArgs e)
        {
            Console.WriteLine("Start playback");
        }

        private void OnSessionStopPlayback(object sender, SessionEventArgs e)
        {
            Console.WriteLine("Stop playback");
        }

        private void OnSessionPlayerTokenLost(object sender, SessionEventArgs e)
        {
            IsPlaying = false;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Playlist.Current != null)
            {
                if ((DurationPlayed >= Playlist.Current.Track.Duration) && IsPlaying)
                {
                    if (Playlist.CanGoNext)
                    {
                        Playlist.Next();
                    }
                }
            }

            _playLocation = _playLocation.Add(TimeSpan.FromMilliseconds(_timer.Interval));

            RaisePropertyChanged("DurationPlayed");
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            var track = Playlist.Current.Track as Track;

            if (track != null)
            {
                track.InternalTrack.Load();

                if (IsPlaying)
                {
                    track.InternalTrack.Play();
                }

                _playLocation = TimeSpan.Zero;
            }
        }

        private void OnSessionMusicDeliver(object sender, MusicDeliveryEventArgs e)
        {
            if (e.Samples.Length > 0)
            {
                e.ConsumedFrames = _bass.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
                IsPlaying = true;
            }
            else
            {
                e.ConsumedFrames = 0;
                IsPlaying = false;
            }
        }

        #endregion Private Methods
    }
}