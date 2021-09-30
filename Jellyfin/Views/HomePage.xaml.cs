using Jellyfin.Models;
using System;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class HomePage : Page
    {
        //private DispatcherTimer timer = null;

        public HomePage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
           base.OnNavigatedTo(e);

            //timer = new DispatcherTimer();

           await ViewModel.PageReadyAsync();
        }

        private void MyMediaHubtile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LibraryDataItems library = (LibraryDataItems)((RadSlideHubTile)sender).DataContext;

            App.Current.Shell.ChangeMenuSelection(library.Id);
        }

        private void LatestMediaHubTile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LatestMediaDataItem LatestItem = (LatestMediaDataItem)((RadHubTile)sender).DataContext;

            App.Current.Shell.ChangeMenuSelection(LatestItem.Id);

            Frame.Navigate(typeof(ItemPage), LatestItem.Id);
        }

        //
        // trying to animate the title overflow text left to right if it is a long title
        //private void LatestMediaTitleScrollViewer_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //ScrollViewer scrollviewer = (ScrollViewer)sender;

        //    //Canvas canvas = (Canvas)scrollviewer.ContentTemplateRoot;

        //    //TextBlock textblock = (TextBlock)canvas.Children[0];

        //    ////timer.Tick += (ss, ee) =>
        //    ////{
        //    ////    if (timer.Interval.Ticks == 300)
        //    ////    {
        //    ////        //each time set the offset to scrollviewer.HorizontalOffset + 5
        //    ////        scrollviewer.ChangeView(scrollviewer.HorizontalOffset + 5, 0, 1);
        //    ////        //if the scrollviewer scrolls to the end, scroll it back to the start.
        //    ////        if (scrollviewer.HorizontalOffset == scrollviewer.ScrollableWidth)
        //    ////            scrollviewer.ChangeView(0, 0, 1);
        //    ////    }
        //    ////};
        //    ////timer.Interval = new TimeSpan(300);
        //    ////timer.Start();

        //    //double tbActualWidth = textblock.ActualWidth;
        //    //double tbwidth = textblock.Width;
        //    //double cActualWdith = canvas.ActualWidth;
        //    //double cWidth = canvas.Width;


        //    //double height = canvas.ActualHeight - textblock.ActualHeight;
        //    //textblock.Margin = new Thickness(0, height / 2, 0, 0);

        //    //Rect rect = new Rect
        //    //{
        //    //    X = 0,
        //    //    Y = 0,
        //    //    Width = tbActualWidth,
        //    //    Height = textblock.ActualHeight
        //    //};

        //    //RectangleGeometry rectangle = new RectangleGeometry
        //    //{
        //    //    Rect = rect
        //    //};

        //    //DoubleAnimation doubleAnimation = new DoubleAnimation
        //    //{
        //    //    From = -textblock.ActualWidth,
        //    //    To = canvas.ActualWidth,
        //    //    RepeatBehavior = RepeatBehavior.Forever,
        //    //    Duration = new Duration(TimeSpan.FromSeconds(5))
        //    //};
        //    //textblock.StartAnimation(doubleAnimation);
        //}

        //private void LatestMediaTitleScrollViewer_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    timer.Stop();
        //}

        private void TitleScrollerCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            //Canvas canvas = (Canvas)sender;

            

        }

        private void TitlePlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected media item
            LatestMediaDataItem MediatDataItem = (LatestMediaDataItem)((Button)sender).DataContext;

            // Set root frame to media player
            App.Current.RootFrame.Navigate(typeof(MediaPlayerPage), MediatDataItem.Id);
        }
    }
}
