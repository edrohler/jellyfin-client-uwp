﻿using System;
using Windows.UI.Xaml.Media;

namespace Jellyfin.Models
{
    public class LibraryDataItems
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public ImageSource ImageSrc { get; set; }
        public string Type { get; set; }
        public TimeSpan UpdateInterval { get; set; }
    }
}