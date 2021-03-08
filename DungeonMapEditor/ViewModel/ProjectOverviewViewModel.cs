using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class ProjectOverviewViewModel : ViewModelBase
    {
        public event EventHandler<NameChangedEventArgs> ProjectNameChanged;

        private ProjectFile mProjectFile;

        public ProjectFile ProjectFile
        {
            get => mProjectFile;
            set
            {
                if (mProjectFile != null)
                {
                    mProjectFile.ProjectNameChanged -= ProjectFile_ProjectNameChanged;
                }
                mProjectFile = value;
                mProjectFile.ProjectNameChanged += ProjectFile_ProjectNameChanged;
                InvokePropertyChanged();
            }
        }

        private void ProjectFile_ProjectNameChanged(object sender, NameChangedEventArgs e)
        {
            OnProjectNameChanged(e);
        }

        protected virtual void OnProjectNameChanged(NameChangedEventArgs e)
        {
            ProjectNameChanged?.Invoke(this, e);
        }
    }
}
