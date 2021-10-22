using Jellyfin.Models;
using Jellyfin.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Jellyfin.Controls
{
    public sealed partial class MediaItemUserControl : UserControl
    {
        public MediaItemUserControl()
        {
            this.InitializeComponent();
        }

        // Play Media Item
        private void TitlePlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected media item
            MediaDataItem MediaDataItem = (MediaDataItem)((Button)sender).DataContext;

            // Set root frame to media player
            App.Current.RootFrame.Navigate(typeof(MediaPlayerPage), MediaDataItem.BaseItem.Id);
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
    }
}
