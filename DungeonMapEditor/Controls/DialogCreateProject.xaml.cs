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
    /// Interaction logic for DialogCreateProject.xaml
    /// </summary>
    public partial class DialogCreateProject : Border
    {
        public event EventHandler<CreateDialogCompletedEventArgs<ProjectFile>> DialogCompleted;

        public DialogCreateProject()
        {
            InitializeComponent();
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            CreateProjectViewModel vm = DataContext as CreateProjectViewModel;
            OnDialogCompleted(new CreateDialogCompletedEventArgs<ProjectFile>(DialogResult.OK, new ProjectFile(vm.ProjectName)));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogCompleted(new CreateDialogCompletedEventArgs<ProjectFile>(DialogResult.Abort));
        }

        protected virtual void OnDialogCompleted(CreateDialogCompletedEventArgs<ProjectFile> e)
        {
            DialogCompleted?.Invoke(this, e);
        }
    }
}
