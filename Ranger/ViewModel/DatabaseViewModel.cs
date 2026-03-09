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
