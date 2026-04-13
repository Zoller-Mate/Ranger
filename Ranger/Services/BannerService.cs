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

        [ObservableProperty]
        private bool isError;

        private BannerService() { }

        public async Task ShowErrorAsync(string message)
        {
            Message = message;
            IsError = true;
            IsVisible = true;

            await Task.Delay(4000);

            IsVisible = false;
        }

        public async Task ShowSuccessAsync(string message)
        {
            Message = message;
            IsError = false;
            IsVisible = true;

            await Task.Delay(4000);

            IsVisible = false;
        }
    }
}