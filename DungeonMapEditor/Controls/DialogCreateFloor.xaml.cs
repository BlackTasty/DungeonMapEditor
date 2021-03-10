using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
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
    /// Interaction logic for DialogCreateFloor.xaml
    /// </summary>
    public partial class DialogCreateFloor : Border
    {
        public event EventHandler<CreateDialogCompletedEventArgs<FloorPlan>> DialogCompleted;

        private ProjectFile projectFile;

        public DialogCreateFloor(ProjectFile projectFile)
        {
            InitializeComponent();
            this.projectFile = projectFile;
            CheckFloorNameExists();
        }

        private void CreateFloor_Click(object sender, RoutedEventArgs e)
        {
            CreateFloorViewModel vm = DataContext as CreateFloorViewModel;
            OnDialogCompleted(new CreateDialogCompletedEventArgs<FloorPlan>(DialogResult.OK, new FloorPlan(vm.FloorName)));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new CreateDialogCompletedEventArgs<FloorPlan>(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(CreateDialogCompletedEventArgs<FloorPlan> e)
        {
            DialogCompleted?.Invoke(this, e);
        }

        private void FloorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFloorNameExists();
        }

        private void CheckFloorNameExists()
        {
            if (projectFile != null)
            {
                CreateFloorViewModel vm = DataContext as CreateFloorViewModel;
                vm.FloorNameExists = projectFile.FloorPlans.Any(x => x.FloorPlan.Name == vm.FloorName);
            }
        }
    }
}
