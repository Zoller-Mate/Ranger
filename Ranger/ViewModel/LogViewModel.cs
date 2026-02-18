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

        // ObservableProperty-k
        [ObservableProperty]
        private ObservableCollection<string> _logDates = new();

        [ObservableProperty]
        private List<Log> _logs = new(); // Nem kell ObservableCollection mert a lista elemei sosem változnak egyesével. Mindig az egész Objectet változtatjuk, akkor elég az ObservableProperty

        [ObservableProperty]
        private string _selectedLogDate = string.Empty;

        // Esemény kezelők
        partial void OnSelectedLogDateChanged(string value)
        {
            if (value is null)
                return;

            LoadLogsByDateCommand.Execute(value);
        }

        // RelayCommand-ok
        [RelayCommand]
        private async Task LoadLogDatesAsync()
        {
            var response = await _apiService.GetAviableLogDatesAsync();

            if (response.Status != "OK")
                return;

            LogDates = new ObservableCollection<string>(response.Data.Dates);
        }

        [RelayCommand]
        private async Task LoadLogsByDateAsync(string selectedLogDate)
        {
            var response = await _apiService.GetLogsByDateAsync(selectedLogDate);


            if (response.Status != "OK")
                return;

            Logs = new List<Log>(response.Data.Logs);
        }
    }
}
