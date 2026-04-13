using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Ranger.Dtos;
using Ranger.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Ranger.ViewModel
{
    internal partial class LogViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new();
        private readonly BannerService _banner = BannerService.Instance;

        public LogViewModel()
        {
            _ = InitializeAsync();
        }

        [ObservableProperty]
        private ObservableCollection<string> _logDates = new();

        [ObservableProperty]
        private List<Log> _logs = new();

        [ObservableProperty]
        private string _selectedLogDate = string.Empty;

        partial void OnSelectedLogDateChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
                LoadLogsByDateCommand.Execute(value);
        }

        private async Task InitializeAsync()
        {
            await LoadLogDatesAsync();

            if (LogDates.Any())
                SelectedLogDate = LogDates.First();
        }

        // ================= LOAD =================
        [RelayCommand]
        private async Task LoadLogDatesAsync()
        {
            var result = await _apiService.GetAviableLogDatesAsync();

            if (!result.IsSuccess)
            {
                await _banner.ShowErrorAsync(result.ErrorMessage ?? "Hiba");
                return;
            }

            LogDates = new ObservableCollection<string>(result.Data!.Dates);
        }

        [RelayCommand]
        private async Task LoadLogsByDateAsync(string date)
        {
            var result = await _apiService.GetLogsByDateAsync(date);

            if (!result.IsSuccess)
            {
                await _banner.ShowErrorAsync(result.ErrorMessage ?? "Hiba");
                return;
            }

            Logs = new List<Log>(result.Data!.Logs);
        }

        // ================= SAVE =================
        [RelayCommand]
        private async Task LogsSaveAsAsync()
        {
            if (!Logs.Any())
            {
                await _banner.ShowErrorAsync("Nincs menthető log.");
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Log (*.log)|*.log|Text (*.txt)|*.txt",
                FileName = $"{SelectedLogDate}.log"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var content = string.Join(
                    Environment.NewLine,
                    Logs.Select(l => $"[{l.Timestamp}] [{l.Method}] {l.Path} {l.StatusCode} - {l.ResponseTime}")
                );

                await File.WriteAllTextAsync(dialog.FileName, content, Encoding.UTF8);

                await _banner.ShowErrorAsync("Log mentve ✔");
            }
            catch (Exception ex)
            {
                await _banner.ShowErrorAsync($"Mentési hiba: {ex.Message}");
            }
        }
    }
}