using Jellyfin.Helpers;
using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Jellyfin.Controls
{
    public sealed partial class MediaItemImageUserControl : UserControl
    {
        public MediaItemImageUserControl()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public static readonly DependencyProperty DesiredImageTypeProperty = DependencyProperty.Register(
            "DesiredImageType", typeof(ImageType), typeof(MediaItemImageUserControl), new PropertyMetadata(ImageType.Primary));

        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
            "Media", typeof(BaseItemDto), typeof(MediaItemImageUserControl), new PropertyMetadata(null, OnMediaChanged));

        public BaseItemDto Media
        {
            get => (BaseItemDto)GetValue(MediaProperty);
            set => SetValue(MediaProperty, value);
        }

        public ImageType DesiredImageType
        {
            get => (ImageType)GetValue(DesiredImageTypeProperty);
            set => SetValue(DesiredImageTypeProperty, value);
        }

        private static async void OnMediaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
            // Set FileName without Extension
            // Accounts for various image types.
            // Single Episodes and Single Audio Songs
            // need to get parent Id.
            string cacheFileName;
            Guid mediaId;

            switch (Media.Type)
            {
                case "Episode":
                    mediaId = (Guid)Media.SeriesId;
                    cacheFileName = $"{Media.SeriesId}-{DesiredImageType}";
                    break;
                case "Audio":
                    mediaId = (Guid)Media.AlbumId;
                    cacheFileName = $"{Media.AlbumId}-{DesiredImageType}";
                    break;
                default:
                    mediaId = Media.Id;
                    cacheFileName = $"{Media.Id}-{DesiredImageType}";
                    break;
            }

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
                        FileResponse fr = await JellyfinClientServices.Current.ImageClient.GetItemImageAsync(mediaId, DesiredImageType, addPlayedIndicator: true);

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
