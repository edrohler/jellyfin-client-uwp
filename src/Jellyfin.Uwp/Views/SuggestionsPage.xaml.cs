﻿using Jellyfin.Sdk;
using Jellyfin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Jellyfin.Views
{
    public sealed partial class SuggestionsPage : Page
    {
        public SuggestionsPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.UserView = App.Current.UserViews.Items.First(i => i.Id == (Guid)e.Parameter);

            await ViewModel.PageReadyAsync();
        }

        private void SuggestionsMediaItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
