using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.UI;
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
    /// Interaction logic for DialogCollectionSetClosingUnsaved.xaml
    /// </summary>
    public partial class DialogCollectionSetClosingUnsaved : Border
    {
        public event EventHandler<ClosingUnsavedDialogButtonClickedEventArgs> DialogCompleted;

        private TileManager tileManager;
        private TabItem targetTab;

        public TabItem TargetTab => targetTab;

        public DialogCollectionSetClosingUnsaved(TabItem targetTab, CollectionSet unsavedCollection, TileManager tileManager)
        {
            InitializeComponent();
            (DataContext as DialogCollectionSetClosingUnsavedViewModel).UnsavedCollection = unsavedCollection;
            this.targetTab = targetTab;
            this.tileManager = tileManager;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(tileManager, targetTab, DialogResult.Yes));
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(tileManager, targetTab, DialogResult.No));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(ClosingUnsavedDialogButtonClickedEventArgs e)
        {
            DialogCompleted?.Invoke(this, e);
        }
    }
}
