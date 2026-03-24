using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ranger.View
{
    /// <summary>
    /// Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : UserControl
    {
        public DatabaseView()
        {
            InitializeComponent();
        }
    }

    public static class DataGridAutoResizeBehavior
    {
        public static TabControl GetHostTab(DependencyObject obj)
            => (TabControl)obj.GetValue(HostTabProperty);

        public static void SetHostTab(DependencyObject obj, TabControl value)
            => obj.SetValue(HostTabProperty, value);

        public static readonly DependencyProperty HostTabProperty =
            DependencyProperty.RegisterAttached(
                "HostTab",
                typeof(TabControl),
                typeof(DataGridAutoResizeBehavior),
                new PropertyMetadata(null, OnChanged));

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid)
                return;

            // első betöltés
            grid.Loaded += (_, __) => TryResize(grid);

            // tab váltás
            if (e.NewValue is TabControl tab)
            {
                tab.SelectionChanged += (_, __) => TryResize(grid);
            }
        }

        private static void TryResize(DataGrid grid)
        {
            // 🔥 EZ A LÉNYEG: várunk amíg van szélesség
            grid.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (grid.ActualWidth == 0 || grid.Columns.Count == 0)
                    return;

                foreach (var col in grid.Columns)
                    col.Width = DataGridLength.Auto;

                grid.Columns[^1].Width =
                    new DataGridLength(1, DataGridLengthUnitType.Star);

            }), DispatcherPriority.Render); // <-- FONTOS!
        }
    }
}
