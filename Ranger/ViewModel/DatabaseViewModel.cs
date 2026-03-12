using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;
using Ranger.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Ranger.Dtos;
using CommunityToolkit.Mvvm.Input;
using System.Dynamic;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using ClosedXML.Excel;

namespace Ranger.ViewModel
{
    internal partial class DatabaseViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new();

        [ObservableProperty]
        private ObservableCollection<object> _tables = new();

        [ObservableProperty]
        private object? _selectedTable;





        [RelayCommand]
        private async Task LoadDatabaseDumpAsync()
        {
            var response = await _apiService.GetDatabaseDumpAsync();
            
            if (response.Status != "OK")
                return;

            Tables.Clear();

            // Végigmegyünk a DatabaseDto összes property-jén
            var dataProperties = response.Data.GetType().GetProperties();

            foreach (var prop in dataProperties)
            {
                var value = prop.GetValue(response.Data);
                if (value == null) continue;

                var listType = value.GetType();
                /*/ Ellenőrizzük, hogy List<T> típusú-e
                if (!listType.IsGenericType || listType.GetGenericTypeDefinition() != typeof(List<>))
                    continue;*/

                // Megszerezzük a lista item típusát (pl. UserDto, CampDto stb.)
                var itemType = listType.GetGenericArguments()[0];

                // Létrehozzuk a TableViewModel<T> objektumot a megfelelő típussal
                var tableViewModelType = typeof(TableViewModel<>).MakeGenericType(itemType);
                var tableViewModel = Activator.CreateInstance(tableViewModelType);

                // Beállítjuk a property-ket
                tableViewModelType.GetProperty("TableName")?.SetValue(tableViewModel, prop.Name);
                tableViewModelType.GetProperty("Rows")?.SetValue(tableViewModel, value);

                Tables.Add(tableViewModel);
            }

            SelectedTable = Tables.FirstOrDefault();
        }

        [RelayCommand]
        private async Task DatabaseSaveAsAsync()
        {
            if (!Tables.Any())
            {
                MessageBox.Show("No database tables to save!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                FileName = $"database_dump_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    await SaveAsExcelAsync(saveDialog.FileName);
                    MessageBox.Show($"Database saved successfully to:\n{saveDialog.FileName}","Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file:\n{ex.Message}","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

                    if (!string.IsNullOrEmpty(tableName) && rows != null)
                    {
                        // Érvényes munkalap név biztosítása (Excel korlátok miatt)
                        var safeSheetName = GetSafeSheetName(tableName);
                        var worksheet = workbook.Worksheets.Add(safeSheetName);

                        if (rows is IEnumerable<object> enumerable)
                        {
                            var rowList = enumerable.ToList();
                            if (rowList.Any())
                            {
                                // Header sor
                                var firstRow = rowList.First();
                                var properties = firstRow.GetType().GetProperties();

                                for (int i = 0; i < properties.Length; i++)
                                {
                                    worksheet.Cell(1, i + 1).Value = properties[i].Name;
                                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                                    worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                                }

                                // Adat sorok
                                int currentRow = 2;
                                foreach (var row in rowList)
                                {
                                    for (int i = 0; i < properties.Length; i++)
                                    {
                                        var value = properties[i].GetValue(row);
                                        worksheet.Cell(currentRow, i + 1).Value = value?.ToString() ?? "";
                                    }
                                    currentRow++;
                                }

                                // Auto-fit oszlopok
                                worksheet.ColumnsUsed().AdjustToContents();
                            }
                            else
                            {
                                worksheet.Cell(1, 1).Value = "";
                            }
                        }
                    }
                }

                // Ha nincs adat, adj hozzá egy üres lapot
                if (!workbook.Worksheets.Any())
                {
                    workbook.Worksheets.Add("Empty");
                }

                workbook.SaveAs(fileName);
            });
        }

        private string GetSafeSheetName(string name)
        {
            // Excel munkalap név korlátok: max 31 karakter, nem tartalmazhat: \ / ? * [ ]
            var safeName = name;
            var invalidChars = new char[] { '\\', '/', '?', '*', '[', ']', ':' };

            foreach (var c in invalidChars)
            {
                safeName = safeName.Replace(c, '_');
            }

            if (safeName.Length > 31)
                safeName = safeName.Substring(0, 31);

            return safeName;
        }


        public DatabaseViewModel()
        {
            LoadDatabaseDumpCommand.ExecuteAsync(null);
        }

    }

    // Egy tábla
    internal partial class TableViewModel<T> : ObservableObject
    {
        [ObservableProperty]
        private string _tableName = string.Empty;

        // List elég, mert mindig az egész objektet cseréljük, nem módosítjuk egyesével az elemeket
        [ObservableProperty]
        private List<T> _rows = new List<T>();
    }
}
