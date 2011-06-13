using System;
using System.Windows.Media.Imaging;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class Album : IAlbum
    {
        #region Properties

        public IArtist Artist
        {
            get; set;
        }

        public bool IsAvailable
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int Year
        {
            get; set;
        }

        public BitmapSource Cover
        {
            get; 
            set;
        }

        #endregion Properties
    }
}