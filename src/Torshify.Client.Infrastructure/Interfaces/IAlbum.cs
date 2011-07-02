﻿using System.Windows.Media.Imaging;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public enum AlbumType
    {
        Album,
        Compilation,
        Single,
        Unknown
    }

    public interface IAlbum
    {
        #region Properties

        IArtist Artist
        {
            get;
        }

        bool IsAvailable
        {
            get;
        }

        string Name
        {
            get;
        }

        int Year
        {
            get;
        }

        BitmapSource Cover
        {
            get;
        }

        IImage CoverArt
        {
            get;
        }

        IAlbumInformation Info
        {
            get;
        }

        AlbumType Type
        {
            get;
        }

        #endregion Properties
    }
}