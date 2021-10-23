using CommonHelpers.Common;
using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.ViewModels
{
    public class SuggestionsViewModel : ViewModelBase
    {
        private BaseItemDto userView;

        public BaseItemDto UserView { get => userView; set => SetProperty(ref userView, value); }

        public SuggestionsViewModel()
        {

        }
    }
}
