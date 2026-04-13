using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Ranger.ViewModel
{
    internal partial class MainViewModel : ObservableObject
    {

        public LogViewModel LogVm { get; set; } = new LogViewModel();
        public DatabaseViewModel databaseVm { get; set; } = new DatabaseViewModel();
        public SettingsViewModel settingsVm { get; set; } = new SettingsViewModel();


        [ObservableProperty]
        private object _currentView;


        [RelayCommand]
        private void ShowLogView() => CurrentView = LogVm;
        [RelayCommand]
        private void ShowDatabaseView() => CurrentView = databaseVm;
        [RelayCommand]
        private void ShowSettingsView() => CurrentView = settingsVm;


        public MainViewModel()
        {
            CurrentView = LogVm;
        }
    }
}
