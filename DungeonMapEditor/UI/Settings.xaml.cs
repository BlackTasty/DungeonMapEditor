using DungeonMapEditor.Core.Events;
using DungeonMapEditor.ViewModel;
using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace DungeonMapEditor.UI
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : DockPanel
    {
        public event EventHandler<SettingsClosingEventArgs> Closing;

        public Settings()
        {
            InitializeComponent();
        }

        public void Save()
        {
            SettingsViewModel vm = DataContext as SettingsViewModel;
            vm.TempSettings.UpdateSearchIntervalMin = int.Parse(updateInterval.Text);
            App.Settings.ApplySettings(vm.TempSettings);
            vm.TempSettings.ResetObserver();
            App.Settings.Save();

            OnClosing(new SettingsClosingEventArgs(vm.TempSettings.UnsavedChanges));
        }

        public void DiscardChanges()
        {
            SettingsViewModel vm = DataContext as SettingsViewModel;
            vm.TempSettings.ApplySettings(App.Settings);
            vm.ForceUpdate();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            SettingsViewModel vm = DataContext as SettingsViewModel;
            App.Settings = new Core.AppSettings();
            vm.TempSettings.ApplySettings(App.Settings);
            vm.TempSettings.ResetObserver();
            vm.ForceUpdate();
            App.Settings.Save(AppDomain.CurrentDomain.BaseDirectory);
            OnClosing(new SettingsClosingEventArgs(vm.TempSettings.UnsavedChanges));
        }

        private void SelectProjectFolder_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectFolder("Select a folder for your projects");
            if (path != null)
            {
                (DataContext as SettingsViewModel).TempSettings.ProjectDirectory = path;
            }
        }

        private void SelectCollectionFolder_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectFolder("Select a folder for your collections");
            if (path != null)
            {
                (DataContext as SettingsViewModel).TempSettings.CollectionDirectory = path;
            }
        }

        private string SelectFolder(string title)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                EnsurePathExists = true,
                EnsureValidNames = true,
                ShowPlacesList = true,
                RestoreDirectory = true,
                Multiselect = false,
                Title = title
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        protected virtual void OnClosing(SettingsClosingEventArgs e)
        {
            Closing?.Invoke(this, e);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            OnClosing(new SettingsClosingEventArgs((DataContext as SettingsViewModel).TempSettings.UnsavedChanges));
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && updateInterval != null)
            {
                updateInterval.IsEnabled = (bool)checkBox.IsChecked;
            }
        }

        private void DockPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.IsDesignMode)
            {
                return;
            }
            updateInterval.IsEnabled = (DataContext as SettingsViewModel).TempSettings.UpdatesEnabled;
        }
    }
}
