using System.Threading.Tasks;
using CommonHelpers.Common;

namespace Jellyfin.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel()
        {
            
        }

        // IMPORTANT: You should never run async code in a constructor. so we signal the view model when the page is ready in this Task
        public async Task PageReadyAsync()
        {

        }
    }
}
