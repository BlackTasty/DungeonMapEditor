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

        public DialogCreateFloor()
        {
            InitializeComponent();
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
    }
}
