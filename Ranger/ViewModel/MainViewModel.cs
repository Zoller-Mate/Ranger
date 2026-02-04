using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Ranger.ViewModel
{
    internal partial class MainViewModel : ObservableObject
    {

        public LogViewModel LogVm { get; set; } = new LogViewModel();

        [ObservableProperty]
        private object _currentView;

        [RelayCommand]
        private void ShowLogView() => CurrentView = LogVm;

        public MainViewModel()
        {
            CurrentView = LogVm;
        }
    }
}
