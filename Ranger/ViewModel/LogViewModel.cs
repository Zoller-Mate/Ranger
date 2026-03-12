using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ranger.Dtos;
using Ranger.Services;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;

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

        [RelayCommand]
        private async Task LogsSaveAsAsync()
        {
            // Ellenőrizzük, hogy vannak-e logok
            if (!Logs.Any())
            {
                MessageBox.Show("No logs to save!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // SaveFileDialog konfigurálása
            SaveFileDialog saveLogDialog = new SaveFileDialog
            {
                Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "log",
                FileName = $"{SelectedLogDate}.log" // Alapértelmezett fájlnév a kiválasztott dátum alapján
            };

            // Dialog megjelenítése
            if (saveLogDialog.ShowDialog() == true)
            {
                try
                {
                    // Log objektumok formázása szöveggé
                    var logLines = Logs.Select(log => FormatLogEntry(log)).ToList();

                    // Szöveg összeállítása
                    var logContent = string.Join(Environment.NewLine, logLines);

                    // Fájl írása aszinkron módon
                    await File.WriteAllTextAsync(saveLogDialog.FileName, logContent, Encoding.UTF8);

                    // Sikeres mentés üzenete
                    MessageBox.Show($"Logs saved successfully to:\n{saveLogDialog.FileName}","Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Hiba esetén hibaüzenet
                    MessageBox.Show($"Error saving file:\n{ex.Message}","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string FormatLogEntry(Log log)
        {
            return $"[{log.Timestamp}] [{log.Method}] {log.Path} {log.StatusCode} - {log.ResponseTime}";
        }

        public LogViewModel()
        {
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadLogDatesCommand.ExecuteAsync(null);
            if (LogDates.Any()) SelectedLogDate = LogDates.First();
        }
    }
}
