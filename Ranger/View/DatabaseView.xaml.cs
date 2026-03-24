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
        public static bool GetEnable(DependencyObject obj)
            => (bool)obj.GetValue(EnableProperty);

        public static void SetEnable(DependencyObject obj, bool value)
            => obj.SetValue(EnableProperty, value);

        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(DataGridAutoResizeBehavior),
                new PropertyMetadata(false, OnEnableChanged));

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid || !(bool)e.NewValue)
                return;

            grid.Loaded += (s, _) => Resize(grid);

            // 🔥 KERESSÜK MEG A TABCONTROLT
            var tab = FindParent<TabControl>(grid);
            if (tab != null)
            {
                tab.SelectionChanged += (s, _) =>
                {
                    if (grid.IsVisible)
                        Resize(grid);
                };
            }
        }

        private static void Resize(DataGrid grid)
        {
            if (grid.Columns.Count == 0) return;

            foreach (var col in grid.Columns)
                col.Width = DataGridLength.Auto;

            grid.Columns.Last().Width =
                new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;

                child = System.Windows.Media.VisualTreeHelper.GetParent(child);
            }
            return null;
        }
    }
}
