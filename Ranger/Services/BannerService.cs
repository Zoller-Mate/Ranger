using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Ranger.Services
{
    public partial class BannerService : ObservableObject
    {
        private static readonly BannerService _instance = new();
        public static BannerService Instance => _instance;

        [ObservableProperty]
        private string message = "";

        [ObservableProperty]
        private bool isVisible;

        private BannerService() { }

        public async Task ShowErrorAsync(string message)
        {
            Message = message;
            IsVisible = true;

            // auto hide 4 sec után
            await Task.Delay(4000);

            IsVisible = false;
        }
    }
}