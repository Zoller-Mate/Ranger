using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ranger.Dtos;
using Ranger.Services;
using System.Collections.ObjectModel;

namespace Ranger.ViewModel
{
    internal partial class LogViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new();

        [ObservableProperty]
        private ObservableCollection<LogDateDto> _logDates = new();

        [RelayCommand]
        private async Task LoadLogDatesAsync()
        {
            var response = await _apiService.GetAviableLogDatesAsync();

            if (response.Status != "Success")
                return;

            LogDates = new ObservableCollection<LogDateDto>(response.Data);
        }
    }
}
