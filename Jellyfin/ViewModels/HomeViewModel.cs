using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using Jellyfin.Sdk;
using Jellyfin.Services;
using Jellyfin.Views;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Jellyfin.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ObservableCollection<LatestMediaModel> LatestMedia { get; set; } = new ObservableCollection<LatestMediaModel>();
        public ObservableCollection<LibrariesModel> Libraries { get; set; } = new ObservableCollection<LibrariesModel>();

        public HomeViewModel()
        {
        }

        //Load Home Page Content
        public async Task PageReadyAsync()
        {
            foreach (var item in App.Current.UserViews.Items)
            {
                Libraries.Add(new LibrariesModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    ImgSource = new BitmapImage(new Uri($"{App.Current.SdkClientSettings.BaseUrl}/Items/{item.DisplayPreferencesId}/Images/Primary")),
                    Type = item.CollectionType,
                    UpdateInterval = new TimeSpan(0,0,new Random().Next(2,9))
                }); 
            }

            foreach (var item in this.Libraries)
            {
                IReadOnlyList<BaseItemDto> LibraryLatestMedia = await UserLibraryClientService.Current.UserLibraryClient.GetLatestMediaAsync(App.Current.AppUser.Id, item.Id);
                if(item.Type != "boxsets")
                {
                    LatestMedia.Add(new LatestMediaModel
                    {
                        Name = item.Name,
                        Id = item.Id,
                        LatestItems = LibraryLatestMedia
                    });
                }
            }
        }
    }

    public class LibrariesModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public ImageSource ImgSource { get; set; }
        public string Type { get; set; }
        public TimeSpan UpdateInterval { get; set; }
    }

    public class LatestMediaModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public IReadOnlyList<BaseItemDto> LatestItems { get; set; }
    }
}
