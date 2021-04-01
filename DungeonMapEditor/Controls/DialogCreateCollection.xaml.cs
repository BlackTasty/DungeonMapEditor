using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.Core.Events;
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
using System.Windows.Shapes;

namespace DungeonMapEditor.Controls
{
    /// <summary>
    /// Interaction logic for DialogCreateCollection.xaml
    /// </summary>
    public partial class DialogCreateCollection : Border, ICreateDialog
    {
        public event EventHandler<CreateDialogCompletedEventArgs> DialogCompleted;

        private List<DirectoryInfo> existingCollections;

        public DialogCreateCollection()
        {
            InitializeComponent();
            existingCollections = new DirectoryInfo(App.Settings.CollectionDirectory).EnumerateDirectories().ToList();
            CheckCollectionNameExists();
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            CreateCollectionViewModel vm = DataContext as CreateCollectionViewModel;
            OnDialogCompleted(new CreateDialogCompletedEventArgs(DialogResult.OK, (IBaseData)new CollectionSet(vm.CollectionName)));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new CreateDialogCompletedEventArgs(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(CreateDialogCompletedEventArgs e)
        {
            DialogCompleted?.Invoke(this, e);
        }

        private void CollectionName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckCollectionNameExists();
        }

        private void CheckCollectionNameExists()
        {
            if (existingCollections != null)
            {
                CreateCollectionViewModel vm = DataContext as CreateCollectionViewModel;
                bool projectNameExists = existingCollections.Any(x => x.Name == vm.CollectionName);
                vm.CollectionNameExists = projectNameExists;
            }
        }
    }
}
