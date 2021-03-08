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
    /// Interaction logic for DialogCreateRoom.xaml
    /// </summary>
    public partial class DialogCreateRoom : Border
    {
        public event EventHandler<CreateDialogCompletedEventArgs<RoomPlan>> DialogCompleted;

        private ProjectFile assignedProject;

        public DialogCreateRoom(ProjectFile assignedProject)
        {
            InitializeComponent();
            this.assignedProject = assignedProject;
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            CreateRoomViewModel vm = DataContext as CreateRoomViewModel;
            OnDialogCompleted(new CreateDialogCompletedEventArgs<RoomPlan>(DialogResult.OK, 
                new RoomPlan(vm.RoomName, vm.TilesX, vm.TilesY, assignedProject)));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new CreateDialogCompletedEventArgs<RoomPlan>(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(CreateDialogCompletedEventArgs<RoomPlan> e)
        {
            DialogCompleted?.Invoke(this, e);
        }
    }
}
