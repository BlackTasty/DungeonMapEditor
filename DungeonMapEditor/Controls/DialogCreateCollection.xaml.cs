using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.ViewModel;
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

namespace DungeonMapEditor.Controls
{
    /// <summary>
    /// Interaction logic for DialogCreateCollection.xaml
    /// </summary>
    public partial class DialogCreateCollection : Border
    {
        public event EventHandler<CreateDialogCompletedEventArgs<CollectionSet>> DialogCompleted;

        public DialogCreateCollection()
        {
            InitializeComponent();
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            CreateCollectionViewModel vm = DataContext as CreateCollectionViewModel;
            OnDialogCompleted(new CreateDialogCompletedEventArgs<CollectionSet>(DialogResult.OK, new CollectionSet(vm.CollectionName)));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new CreateDialogCompletedEventArgs<CollectionSet>(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(CreateDialogCompletedEventArgs<CollectionSet> e)
        {
            DialogCompleted?.Invoke(this, e);
        }
    }
}
