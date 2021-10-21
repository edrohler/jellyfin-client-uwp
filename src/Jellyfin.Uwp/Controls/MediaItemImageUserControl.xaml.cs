using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.Controls
{
    public sealed partial class MediaItemImageUserControl : UserControl
    {
        public MediaItemImageUserControl()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public static readonly DependencyProperty MediaIdProperty = DependencyProperty.Register(
            "MediaId", typeof(Guid), typeof(MediaItemImageUserControl), new PropertyMetadata(default(Guid), OnMediaIdChanged));

        public static readonly DependencyProperty DesiredImageTypeProperty = DependencyProperty.Register(
            "DesiredImageType", typeof(ImageType), typeof(MediaItemImageUserControl), new PropertyMetadata(ImageType.Primary));
        
        /// <summary>
        /// Sets the value of the Id of the media item's images to fetch.
        /// the control will first check the local cache to see if the image has already been downloaded.
        /// If the image is in the cache, it will load that one
        /// If the image is not in the cache, it will download it, save to the cache and then create the 
        /// </summary>
        public Guid MediaId
        {
            get => (Guid)GetValue(MediaIdProperty);
            set => SetValue(MediaIdProperty, value);
        }
        
        /// <summary>
        /// Selected the desired ImageType, Default is ImageType.Primary
        /// </summary>
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
            BitmapImage img = new BitmapImage
            {
                DecodePixelHeight = 300,
                DecodePixelWidth = 300
            };

            string cacheFileName = $"{MediaId}.png";

            // Using TryGetItem will let you get a null value (instead of a FileNoteFoundException)
            IStorageItem item = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(cacheFileName);
            
            if (item is StorageFile imageFile) // If the item exists, there is no need to download it again
            {
                // Open up the file for reading
                using(Stream stream = await imageFile.OpenStreamForReadAsync())
                {
                    // Load the image
                    await img.SetSourceAsync(stream.AsRandomAccessStream());
                }
            }
            else // If the file does not exist, download it, save it 
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Get the API response
                        FileResponse fr = await JellyfinClientServices.Current.ImageClient.GetItemImageAsync(MediaId, DesiredImageType);

                        // Copy the API stream into a MemoryStream
                        await fr.Stream.CopyToAsync(ms);

                        // Dispose the FileResponse's Stream now that we do not need it anymore 
                        fr.Stream.Dispose();

                        // Create the file in the cache 
                        StorageFile file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(cacheFileName, CreationCollisionOption.ReplaceExisting);

                        // Rewind the MemoryStream to the beginning
                        ms.Position = 0;

                        // Write the entire Stream as a byte array
                        byte[] imageBytes = ms.ToArray();

                        // Write that byte[] to the file
                        await FileIO.WriteBytesAsync(file, imageBytes);

                        // Load up the image using the same MemoryStream for best performance
                        await img.SetSourceAsync(ms.AsRandomAccessStream());
                    }
                }
                catch (Exception ex)
                {
                    //TODO: Set Default Image
                    ExceptionLogger.LogException(ex);
                }
            }
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
