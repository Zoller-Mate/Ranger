using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Ranger.Services;
using System.Collections.ObjectModel;

namespace Ranger.ViewModel
{
    internal partial class DatabaseViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new();
        private readonly BannerService _banner = BannerService.Instance;

        public DatabaseViewModel()
        {
            _ = LoadDatabaseDumpAsync();
        }

        [ObservableProperty]
        private ObservableCollection<object> _tables = new();

        [ObservableProperty]
        private object? _selectedTable;

        // ================= LOAD =================
        [RelayCommand]
        private async Task LoadDatabaseDumpAsync()
        {
            var result = await _apiService.GetDatabaseDumpAsync();

            if (!result.IsSuccess)
            {
                await _banner.ShowErrorAsync(result.ErrorMessage ?? "Hiba történt");
                return;
            }

            Tables.Clear();

            var dataProperties = result.Data!.GetType().GetProperties();

            foreach (var prop in dataProperties)
            {
                var value = prop.GetValue(result.Data);
                if (value == null) continue;

                var listType = value.GetType();
                var itemType = listType.GetGenericArguments()[0];

                var tableViewModelType = typeof(TableViewModel<>).MakeGenericType(itemType);
                var tableViewModel = Activator.CreateInstance(tableViewModelType);

                tableViewModelType.GetProperty("TableName")?.SetValue(tableViewModel, prop.Name);
                tableViewModelType.GetProperty("Rows")?.SetValue(tableViewModel, value);

                Tables.Add(tableViewModel);
            }

            SelectedTable = Tables.FirstOrDefault();
        }

        // ================= SAVE =================
        [RelayCommand]
        private async Task DatabaseSaveAsAsync()
        {
            if (!Tables.Any())
            {
                await _banner.ShowErrorAsync("Nincs menthető adat.");
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"database_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                await SaveAsExcelAsync(dialog.FileName);
                await _banner.ShowErrorAsync("Sikeres mentés ✔"); // (igen, most error színnel – lásd lent)
            }
            catch (Exception ex)
            {
                await _banner.ShowErrorAsync($"Mentési hiba: {ex.Message}");
            }
        }

        private async Task SaveAsExcelAsync(string fileName)
        {
            await Task.Run(() =>
            {
                using var workbook = new XLWorkbook();

                foreach (var table in Tables)
                {
                    var tableName = table.GetType().GetProperty("TableName")?.GetValue(table) as string;
                    var rows = table.GetType().GetProperty("Rows")?.GetValue(table);

                    if (string.IsNullOrEmpty(tableName) || rows == null) continue;

                    var ws = workbook.Worksheets.Add(GetSafeSheetName(tableName));

                    if (rows is IEnumerable<object> data)
                    {
                        var list = data.ToList();
                        if (!list.Any()) continue;

                        var props = list.First().GetType().GetProperties();

                        for (int i = 0; i < props.Length; i++)
                        {
                            ws.Cell(1, i + 1).Value = props[i].Name;
                            ws.Cell(1, i + 1).Style.Font.Bold = true;
                        }

                        int r = 2;
                        foreach (var row in list)
                        {
                            for (int i = 0; i < props.Length; i++)
                            {
                                ws.Cell(r, i + 1).Value = props[i].GetValue(row)?.ToString();
                            }
                            r++;
                        }

                        ws.ColumnsUsed().AdjustToContents();
                    }
                }

                workbook.SaveAs(fileName);
            });
        }

        private string GetSafeSheetName(string name)
        {
            var invalid = new char[] { '\\', '/', '?', '*', '[', ']', ':' };

            foreach (var c in invalid)
                name = name.Replace(c, '_');

            return name.Length > 31 ? name[..31] : name;
        }
    }

    internal partial class TableViewModel<T> : ObservableObject
    {
        [ObservableProperty]
        private string _tableName = string.Empty;

        [ObservableProperty]
        private List<T> _rows = new();
    }
}