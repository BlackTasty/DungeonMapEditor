using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
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
    /// Interaction logic for DialogClosingUnsaved.xaml
    /// </summary>
    public partial class DialogClosingUnsaved : Border, IClosingDialog
    {
        public event EventHandler<ClosingUnsavedDialogButtonClickedEventArgs> DialogCompleted;

        private dynamic data;
        private TabItem targetTab;

        public TabItem TargetTab => targetTab;

        public DialogClosingUnsaved(TabItem targetTab)
        {
            InitializeComponent();
            this.targetTab = targetTab;
        }

        public void SetObjectValues<T>(T data)
        {
            this.data = data;
            DialogUnsavedChangesViewModel vm = DataContext as DialogUnsavedChangesViewModel;
            if (data is RoomPlanGrid roomPlanGrid)
            {
                vm.ObjectName = "room";
                vm.Name = (roomPlanGrid.DataContext as RoomPlanViewModel).RoomPlan.Name;
            }
            else if (data is FloorPlanGrid floorPlanGrid)
            {
                vm.ObjectName = "floor";
                vm.Name = (floorPlanGrid.DataContext as FloorPlanViewModel).FloorPlan.Name;
            }
            else if (data is ProjectOverview projectOverview)
            {
                vm.ObjectName = "project";
                vm.Name = (projectOverview.DataContext as ProjectOverviewViewModel).ProjectFile.Name;
            }
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(data, targetTab, DialogResult.Yes));
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new ClosingUnsavedDialogButtonClickedEventArgs(data, targetTab, DialogResult.No));
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
