using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Ranger.ViewModel
{
    internal partial class MainViewModel : ObservableObject
    {

        public LogViewModel LogVm { get; set; } = new LogViewModel();
        public DatabaseViewModel databaseVm { get; set; } = new DatabaseViewModel();


        [ObservableProperty]
        private object _currentView;


        [RelayCommand]
        private void ShowLogView() => CurrentView = LogVm;
        [RelayCommand]
        private void ShowDatabaseView() => CurrentView = databaseVm;


        public MainViewModel()
        {
            CurrentView = LogVm;
        }
    }
}
