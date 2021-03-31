using DungeonMapEditor.Controls;
using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.Core.Observer;
using DungeonMapEditor.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace DungeonMapEditor.UI
{
    /// <summary>
    /// Interaction logic for TileManager.xaml
    /// </summary>
    public partial class TileManager : Grid
    {
        public event EventHandler<OpenDialogEventArgs> OpenDialog;
        public event EventHandler<ClosingUnsavedDialogButtonClickedEventArgs> UnsavedDialogsCompleted;

        private List<CollectionSet> unsavedCollections;
        private int currentDialogIndex;
        private bool abortShowDialogs;

        public bool AnyUnsavedChanges => (DataContext as TileManagerViewModel).LoadedCollections.Any(x => x.AnyUnsavedChanges);

        public TileManager()
        {
            InitializeComponent();
        }

        public void BeginShowDialogForUnsavedCollections(TabItem targetTab)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            unsavedCollections = vm.LoadedCollections.Where(x => x.AnyUnsavedChanges).ToList();
            abortShowDialogs = false;
            currentDialogIndex = 0;
            ShowDialogForUnsavedCollection(targetTab);
        }

        private void ShowDialogForUnsavedCollection(TabItem targetTab)
        {
            if (abortShowDialogs)
            {
                return;
            }

            DialogCollectionSetClosingUnsaved dialog = new DialogCollectionSetClosingUnsaved(targetTab, unsavedCollections[currentDialogIndex], this);
            dialog.DialogCompleted += DialogUnsavedCollection_DialogCompleted;
        }

        private void DialogUnsavedCollection_DialogCompleted(object sender, ClosingUnsavedDialogButtonClickedEventArgs e)
        {
            if (e.DialogResult == DialogResult.Abort)
            {
                abortShowDialogs = true;
                OnUnsavedDialogsCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(e.DialogResult));
                return;
            }

            if (e.DialogResult == DialogResult.Yes)
            {
                unsavedCollections[currentDialogIndex].Save();
            }
            else
            {
                unsavedCollections[currentDialogIndex].Load();
            }

            currentDialogIndex++;

            if (currentDialogIndex < unsavedCollections.Count)
            {
                ShowDialogForUnsavedCollection(e.TargetTab);
            }
            else
            {
                OnUnsavedDialogsCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(e.DialogResult));
            }
        }

        private void TileConfigurator_DialogButtonClicked(object sender, TileDialogButtonClickedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            if (e.DialogResult == DialogResult.OK)
            {
                FileInfo imageFileInfo = !string.IsNullOrWhiteSpace(e.Tile.ImagePath) ? new FileInfo(e.Tile.ImagePath) : null;
                if (imageFileInfo != null && imageFileInfo.Exists)
                {
                    string localResourcePath = Path.Combine(vm.SelectedCollection.FilePath, "tiles", "resources");
                    if (!Directory.Exists(localResourcePath))
                    {
                        Directory.CreateDirectory(localResourcePath);
                    }

                    string localImagePath = Path.Combine(localResourcePath, imageFileInfo.Name);
                    if (!new FileInfo(localImagePath).Exists)
                    {
                        File.Copy(imageFileInfo.FullName, localImagePath);
                    }
                    e.Tile.ImagePath = localImagePath;
                }

                if (!vm.IsEditTile)
                {
                    vm.SelectedCollection.TileFile.Data.Add(e.Tile);
                }
            }

            vm.IsTileConfiguratorOpen = false;
        }

        private void AddNewTile_Click(object sender, RoutedEventArgs e)
        {
            ShowTileConfigurator(false);
        }

        private void EditTile_Click(object sender, RoutedEventArgs e)
        {
            ShowTileConfigurator(true);
        }

        private void RemoveTile_Click(object sender, RoutedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            DialogRemoveObject dialog = new DialogRemoveObject(vm.SelectedCollection.TileFile.Data[vm.SelectedTileIndex].Name);
            dialog.DialogCompleted += DialogRemoveTile_DialogCompleted;

            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void DialogRemoveTile_DialogCompleted(object sender, DialogButtonClickedEventArgs e)
        {
            if (e.DialogResult == DialogResult.OK)
            {
                TileManagerViewModel vm = DataContext as TileManagerViewModel;
                Tile selectedTile = vm.SelectedCollection.TileFile.Data[vm.SelectedTileIndex];
                int index = vm.SelectedCollection.TileFile.Data.ToList().FindIndex(x => x.Guid == selectedTile.Guid);
                if (index > -1)
                {
                    vm.SelectedCollection.TileFile.Data.RemoveAt(index);
                    vm.SelectedTileIndex = -1;
                }
            }
        }

        private void ShowTileConfigurator(bool isEdit)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;

            vm.IsTileConfiguratorOpen = true;
            vm.IsEditTile = isEdit;
            if (isEdit)
            {
                tileConfigurator.SetTileAssignment(new TileAssignment(vm.SelectedTile));
            }
            else
            {
                tileConfigurator.SetTileAssignment(new TileAssignment(new Tile(true)));
            }
        }

        private void ShowPlaceableConfigurator(bool isEdit)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;

            vm.IsPlaceableConfiguratorOpen = true;
            vm.IsEditPlaceable = isEdit;
            if (isEdit)
            {
                placeableConfigurator.SetPlaceable(vm.SelectedPlaceable);
            }
            else
            {
                placeableConfigurator.SetPlaceable(new Placeable(true));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            string collectionDir = Path.Combine(App.BasePath, "Collections", vm.SelectedCollection.Name);
            Directory.CreateDirectory(collectionDir);

            var selectedCollectionIndex = App.LoadedCollections.IndexOf(vm.SelectedCollection);
            vm.SelectedCollection.Save(collectionDir);
            App.LoadCollections();
            vm.SelectedCollection = App.LoadedCollections[selectedCollectionIndex];
        }

        private void AddPlaceable_Click(object sender, RoutedEventArgs e)
        {
            ShowPlaceableConfigurator(false);
        }

        private void EditPlaceable_Click(object sender, RoutedEventArgs e)
        {
            ShowPlaceableConfigurator(true);
        }

        private void RemovePlaceable_Click(object sender, RoutedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            DialogRemoveObject dialog = new DialogRemoveObject(vm.SelectedCollection.PlaceableFile.Data[vm.SelectedPlaceableIndex].Name);
            dialog.DialogCompleted += DialogRemovePlaceable_DialogCompleted;

            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void DialogRemovePlaceable_DialogCompleted(object sender, DialogButtonClickedEventArgs e)
        {
            if (e.DialogResult == DialogResult.OK)
            {
                TileManagerViewModel vm = DataContext as TileManagerViewModel;
                Placeable selectedPlaceable = vm.SelectedCollection.PlaceableFile.Data[vm.SelectedPlaceableIndex];
                int index = vm.SelectedCollection.PlaceableFile.Data.ToList().FindIndex(x => x.Guid == selectedPlaceable.Guid);
                if (index > -1)
                {
                    vm.SelectedCollection.PlaceableFile.Data.RemoveAt(index);
                    vm.SelectedPlaceableIndex = -1;
                }
            }
        }

        private void PlaceableConfigurator_DialogButtonClicked(object sender, PlaceableDialogButtonClickedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            if (e.DialogResult == DialogResult.OK)
            {
                FileInfo imageFileInfo = !string.IsNullOrWhiteSpace(e.Placeable.ImagePath) ? new FileInfo(e.Placeable.ImagePath) : null;
                if (imageFileInfo != null && imageFileInfo.Exists)
                {
                    string localResourcePath = Path.Combine(vm.SelectedCollection.FilePath, "placeables", "resources");
                    if (!Directory.Exists(localResourcePath))
                    {
                        Directory.CreateDirectory(localResourcePath);
                    }

                    string localImagePath = Path.Combine(localResourcePath, imageFileInfo.Name);
                    if (!new FileInfo(localImagePath).Exists)
                    {
                        File.Copy(imageFileInfo.FullName, localImagePath);
                    }
                    e.Placeable.ImagePath = localImagePath;
                }

                if (!vm.IsEditPlaceable)
                {
                    vm.SelectedCollection.PlaceableFile.Data.Add(e.Placeable);
                }
            }

            vm.IsPlaceableConfiguratorOpen = false;
        }

        private void CreateCollection_Click(object sender, RoutedEventArgs e)
        {
            DialogCreateCollection dialog = new DialogCreateCollection();
            dialog.DialogCompleted += CollectionDialog_DialogCompleted;
            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void CollectionDialog_DialogCompleted(object sender, CreateDialogCompletedEventArgs e)
        {
            if (e.DialogResult == DialogResult.OK && e.ResultObject is CollectionSet result)
            {
                TileManagerViewModel vm = DataContext as TileManagerViewModel;
                vm.LoadedCollections.Add(result);
                vm.SelectedCollection = result;
            }
        }

        protected virtual void OnOpenDialog(OpenDialogEventArgs e)
        {
            OpenDialog?.Invoke(this, e);
        }

        protected virtual void OnUnsavedDialogsCompleted(ClosingUnsavedDialogButtonClickedEventArgs e)
        {
            UnsavedDialogsCompleted?.Invoke(this, e);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Collection package|*.col",
                FileName = vm.SelectedCollection.Name + ".col",
                AddExtension = true
            };

            if (dialog.ShowDialog() == true)
            {
                vm.SelectedCollection.ExportCollection(dialog.FileName);
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Collection package|*.col",
                AddExtension = true
            };


            if (dialog.ShowDialog() == true)
            {
                CollectionSet imported = CollectionSet.ImportFromFile(dialog.FileName);
                vm.LoadedCollections.Add(imported);
                vm.SelectedCollection = imported;
            }
        }

        private void DeleteCollection_Click(object sender, RoutedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            DialogRemoveObject dialog = new DialogRemoveObject(vm.SelectedCollection.Name);
            dialog.DialogCompleted += DialogRemoveCollection_DialogCompleted;

            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void DialogRemoveCollection_DialogCompleted(object sender, DialogButtonClickedEventArgs e)
        {
            if (e.DialogResult == DialogResult.OK)
            {
                TileManagerViewModel vm = DataContext as TileManagerViewModel;
                CollectionSet selectedCollection = vm.SelectedCollection;
                selectedCollection.Delete();
                vm.LoadedCollections.Remove(selectedCollection);
                vm.SelectedCollection = null;
            }
        }

        private void DuplicatePlaceable_Click(object sender, RoutedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            Placeable duplicate = vm.SelectedPlaceable.Duplicate(true);
            duplicate.Name += string.Format(" ({0})", vm.SelectedCollection.PlaceableFile.Data.Count(x => x.Name == duplicate.Name));
            vm.SelectedCollection.PlaceableFile.Data.Add(duplicate);
            vm.SelectedPlaceableIndex = vm.SelectedCollection.PlaceableFile.Data.Count - 1;
        }

        private void DuplicateTile_Click(object sender, RoutedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            Tile duplicate = vm.SelectedTile.Duplicate(true);
            duplicate.Name += string.Format(" ({0})", vm.SelectedCollection.TileFile.Data.Count(x => x.Name == duplicate.Name));
            vm.SelectedCollection.TileFile.Data.Add(duplicate);
            vm.SelectedTileIndex = vm.SelectedCollection.TileFile.Data.Count - 1;

        }
    }
}
