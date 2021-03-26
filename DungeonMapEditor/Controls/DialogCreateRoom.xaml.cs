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
    public partial class DialogCreateRoom : Border, ICreateDialog
    {
        public event EventHandler<CreateDialogCompletedEventArgs> DialogCompleted;

        private ProjectFile assignedProject;

        public DialogCreateRoom(ProjectFile assignedProject)
        {
            InitializeComponent();
            this.assignedProject = assignedProject;
            (DataContext as CreateRoomViewModel).RoomNumber = assignedProject.RoomPlans.Count + 1;
            CheckRoomNameExists();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CreateRoomViewModel vm = DataContext as CreateRoomViewModel;
            OnDialogCompleted(new CreateDialogCompletedEventArgs(DialogResult.OK, 
                new RoomPlan(vm.RoomName, vm.RoomNumber, vm.TilesX, vm.TilesY, assignedProject)));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new CreateDialogCompletedEventArgs(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(CreateDialogCompletedEventArgs e)
        {
            DialogCompleted?.Invoke(this, e);
        }

        private void RoomName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckRoomNameExists();
        }

        private void CheckRoomNameExists()
        {
            if (assignedProject != null)
            {
                CreateRoomViewModel vm = DataContext as CreateRoomViewModel;
                vm.RoomNameExists = assignedProject.RoomPlans.Any(x => x.RoomPlan.Name == vm.RoomName);
            }
        }
    }
}
