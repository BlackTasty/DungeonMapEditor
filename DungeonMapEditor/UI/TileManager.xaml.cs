using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.Core.FileSystem;
using DungeonMapEditor.ViewModel;
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
        public event EventHandler<TileManagerDialogButtonClickedEventArgs> DialogButtonClicked;
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
                vm.SelectedCollection.TileFile.Data.Remove(selectedTile);
                vm.SelectedTileIndex = -1;
            }
        }

        private void ShowTileConfigurator(bool isEdit)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;

            vm.IsTileConfiguratorOpen = true;
            vm.IsEditTile = isEdit;
            if (isEdit)
            {
                tileConfigurator.SetTileAssignment(new TileAssignment(vm.GetSelectedTile()));
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
                placeableConfigurator.SetPlaceable(vm.GetSelectedPlaceable());
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

            OnDialogButtonClicked(new TileManagerDialogButtonClickedEventArgs(vm.SelectedCollection.TileFile, DialogResult.OK));
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            OnDialogButtonClicked(new TileManagerDialogButtonClickedEventArgs(DialogResult.Abort));
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
                vm.SelectedCollection.PlaceableFile.Data.Remove(selectedPlaceable);
                vm.SelectedPlaceableIndex = -1;
            }
        }

        private void PlaceableConfigurator_DialogButtonClicked(object sender, PlaceableDialogButtonClickedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            if (e.DialogResult == DialogResult.OK)
            {
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

        private void CollectionDialog_DialogCompleted(object sender, CreateDialogCompletedEventArgs<CollectionSet> e)
        {
            if (e.DialogResult == DialogResult.OK)
            {
                TileManagerViewModel vm = DataContext as TileManagerViewModel;
                vm.LoadedCollections.Add(e.ResultObject);
                vm.SelectedCollection = e.ResultObject;
            }
        }

        protected virtual void OnDialogButtonClicked(TileManagerDialogButtonClickedEventArgs e)
        {
            DialogButtonClicked?.Invoke(this, e);
        }

        protected virtual void OnOpenDialog(OpenDialogEventArgs e)
        {
            OpenDialog?.Invoke(this, e);
        }

        protected virtual void OnUnsavedDialogsCompleted(ClosingUnsavedDialogButtonClickedEventArgs e)
        {
            UnsavedDialogsCompleted?.Invoke(this, e);
        }
    }
}
