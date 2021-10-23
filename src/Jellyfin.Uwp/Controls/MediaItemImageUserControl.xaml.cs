﻿using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Jellyfin.Controls
{
    public sealed partial class MediaItemImageUserControl : UserControl
    {
        public MediaItemImageUserControl()
        {
            this.InitializeComponent();
        }

        #region Dependency Properties

        public static readonly DependencyProperty MediaIdProperty = DependencyProperty.Register(
            "MediaId", typeof(Guid), typeof(MediaItemImageUserControl), new PropertyMetadata(default(Guid), OnMediaIdChanged));

        public static readonly DependencyProperty DesiredImageTypeProperty = DependencyProperty.Register(
            "DesiredImageType", typeof(ImageType), typeof(MediaItemImageUserControl), new PropertyMetadata(ImageType.Primary));

        public Guid MediaId
        {
            get => (Guid)GetValue(MediaIdProperty);
            set => SetValue(MediaIdProperty, value);
        }

        public ImageType DesiredImageType
        {
            get => (ImageType)GetValue(DesiredImageTypeProperty);
            set => SetValue(DesiredImageTypeProperty, value);
        }

        private static async void OnMediaIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MediaItemImageUserControl self)
            {
                if (e.NewValue != null)
                {
                    await self.LoadImageAsync();
                }

                if (e.NewValue == null)
                {
                    self.ItemImage.Source = null;
                }
            }
        }

        #endregion

        #region Local Methods

        private async Task LoadImageAsync()
        {
            //var height = this.Height;
            //var width = this.Width;

            //BitmapImage bitmapImage = new BitmapImage
            //{
            //    DecodePixelHeight = Convert.ToInt32(this.Height),
            //    DecodePixelWidth = Convert.ToInt32(this.Width)
            //};

            // Set FileName without Extension
            // Accounts for various image types.
            string cacheFileName = $"{this.MediaId}-{this.DesiredImageType}";

            // Verify that an image exists by {filename}.*
            DirectoryInfo root = new DirectoryInfo(ApplicationData.Current.LocalCacheFolder.Path);
            FileInfo[] files = root.GetFiles($"{cacheFileName}.*");

            // If the item exists, there is no need to download it again
            if (files.Count() > 0)
            {
                Debug.WriteLine("LOCAL FILE Loading from file");

                // Get FirstFileInfo (Should be only 1)
                FileInfo file = files[0];

                // Create new Storage File object
                StorageFile imageFile = (StorageFile)await ApplicationData.Current.LocalCacheFolder.GetItemAsync(file.Name);

                // Open up the file for reading
                using (Stream stream = await imageFile.OpenStreamForReadAsync())
                {
                    // Load the image
                    await ItemBitmapImage.SetSourceAsync(stream.AsRandomAccessStream());

                }
            }
            else // If the file does not exist, download it, save it 
            {
                Debug.WriteLine("SERVER Loading from server");

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Get the API response
                        FileResponse fr = await JellyfinClientServices.Current.ImageClient.GetItemImageAsync(this.MediaId, this.DesiredImageType);

                        // Copy the API stream into a MemoryStream
                        await fr.Stream.CopyToAsync(ms);

                        // Get image type from Contet-Type header
                        string imageType = fr.Headers["Content-Type"].First().Split("/")[1];

                        // Create the file in the cache 
                        StorageFile file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"{cacheFileName}.{imageType}", CreationCollisionOption.ReplaceExisting);

                        // Dispose the FileResponse's Stream now that we do not need it anymore 
                        fr.Stream.Dispose();

                        // Rewind the MemoryStream to the beginning
                        ms.Position = 0;

                        // Write the entire Stream as a byte array
                        byte[] imageBytes = ms.ToArray();

                        // Write that byte[] to the file
                        await FileIO.WriteBytesAsync(file, imageBytes);

                        // Load up the image using the same MemoryStream for best performance
                        await ItemBitmapImage.SetSourceAsync(ms.AsRandomAccessStream());
                    }
                }
                catch (Exception ex)
                {
                    // Need to log exception when request gets error from server
                    ExceptionLogger.LogException(ex);
                }
            }

            // Finally assign the ImageSource to the Image
            //this.ItemImage.Source = bitmapImage;
        }

        #endregion

        #region Global Methods

        /// <summary>
        /// Clear the cached file.
        /// </summary>
        /// <param name="id">the Guid of the item</param>
        /// <param name="fileExtension">the file extension, without separator (default is 'jpg')</param>
        /// <returns></returns>
        public static async Task ClearCacheForId(Guid id, string fileExtension = "png")
        {
            string cacheFileName = $"{id}.{fileExtension}";

            IStorageItem item = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(cacheFileName);

            await item.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        #endregion
    }
}