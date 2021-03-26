using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Enum;
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
    /// Interaction logic for DialogExportProject.xaml
    /// </summary>
    public partial class DialogExportProject : Border, ICreateDialog
    {
        public event EventHandler<CreateDialogCompletedEventArgs> DialogCompleted;

        private ProjectFile projectFile;

        public DialogExportProject(ProjectFile projectFile)
        {
            InitializeComponent();
            this.projectFile = projectFile;
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            DialogExportProjectViewModel vm = DataContext as DialogExportProjectViewModel;
            OnDialogCompleted(new CreateDialogCompletedEventArgs(DialogResult.OK, 
                                        new ProjectExport(projectFile, vm.ExportType, vm.ExportNotes)));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new CreateDialogCompletedEventArgs(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(CreateDialogCompletedEventArgs e)
        {
            DialogCompleted?.Invoke(this, e);
        }
    }
}
