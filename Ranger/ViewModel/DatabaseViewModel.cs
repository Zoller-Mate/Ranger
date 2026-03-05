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

namespace Ranger.ViewModel
{
    internal partial class DatabaseViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new();

        [ObservableProperty]
        private ObservableCollection<TableViewModel> _tables = new();

        [ObservableProperty]
        private TableViewModel? _selectedTable;

        [RelayCommand]
        private async Task LoadDatabaseDumpAsync()
        {
            var response = await _apiService.GetDatabaseDumpAsync();

            if (response.Status != "OK")
                return;

            Tables.Clear();

            foreach (var (tableName, tableData) in response.Data.Tables)
            {

                var rawJson = tableData.GetRawText();
                MessageBox.Show($"Tábla: {tableName}\nJSON: {rawJson}");

                var dictRows = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(
                    tableData.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                // ExpandoObject-re alakítás, hogy működjön a dataGrid
                var expandoRows = dictRows?.Select(dict =>
                {
                    var expando = new ExpandoObject() as IDictionary<string, object>;
                    foreach (var kvp in dict)
                    {
                        expando[kvp.Key] = ConvertJsonElement(kvp.Value);
                    }
                    if (dictRows == null)
                    {
                        // Itt logolhatsz vagy breakpointot tehetsz
                        MessageBox.Show($"A {tableName} tábla deszerializálása sikertelen vagy üres.");
                    }
                    return (dynamic)expando;
                }).ToList() ?? new List<dynamic>();


                var tableVm = new TableViewModel
                {
                    TableName = tableName,
                    Rows = expandoRows
                };


                MessageBox.Show($"Tábla: {tableName}\nSorok száma: {expandoRows.Count}\nElső sor típusa: {expandoRows.FirstOrDefault()?.GetType().Name}");


                Tables.Add(tableVm);
            }

            SelectedTable = Tables.FirstOrDefault();
        }

        // JsonElement konverzió megfelelő típusra
        private static object ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => "NULL",
                JsonValueKind.Undefined => string.Empty,
                _ => element.ToString()
            };
        }
    }

    // Egy tábla
    internal partial class TableViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _tableName = string.Empty;

        // List elég, mert mindig az egész objektet cseréljük, nem módosítjuk egyesével az elemeket
        [ObservableProperty]
        private List<dynamic> _rows = new();
    }
}
