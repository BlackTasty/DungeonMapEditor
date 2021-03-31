using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DungeonMapEditor.ViewModel.AttachedProperties
{
    class SelectingItemAttachedProperty<T>
    {
        public static readonly DependencyProperty SelectingItemProperty = DependencyProperty.RegisterAttached(
            "SelectingItem",
            typeof(T),
            typeof(SelectingItemAttachedProperty<T>),
            new PropertyMetadata(default(T), OnSelectingItemChanged));

        public static object GetSelectingItem(DependencyObject target)
        {
            return (T)target.GetValue(SelectingItemProperty);
        }

        public static void SetSelectingItem(DependencyObject target, T value)
        {
            target.SetValue(SelectingItemProperty, value);
        }

        static void OnSelectingItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid == null || grid.SelectedItem == null)
                return;

            // Works with .Net 4.5
            grid.Dispatcher.InvokeAsync(() =>
            {
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.SelectedItem, null);
            });

            // Works with .Net 4.0
            grid.Dispatcher.BeginInvoke((Action)(() =>
            {
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.SelectedItem, null);
            }));
        }
    }
}
