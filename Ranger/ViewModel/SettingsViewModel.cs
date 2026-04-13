using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ranger.Properties;
using System.Windows;
using Ranger.Services;

namespace Ranger.ViewModel
{
    internal partial class SettingsViewModel : ObservableObject
    {
        private readonly BannerService _banner = BannerService.Instance;
        public SettingsViewModel() { }

        // [ObservableProperty]
        // private string _serverAddress = Settings.Default.ServerAddress;

        [ObservableProperty]
        private string _devApiKey = Settings.Default.DevApiKey;

        [RelayCommand]
        private async void SaveSettingsAsync()
        {
            Settings.Default.DevApiKey = DevApiKey;
            Settings.Default.Save();

            await _banner.ShowSuccessAsync("Sikeres mentés ✔");
        }
    }
}
