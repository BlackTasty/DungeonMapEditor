using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Events
{
    public class HomeScreenSelectionMadeEventArgs : EventArgs
    {
        private HomeScreenSelectionType selection;
        private ProjectFile selectedProject;

        public HomeScreenSelectionType Selection => selection;

        public ProjectFile SelectedProject => selectedProject;

        public HomeScreenSelectionMadeEventArgs(HomeScreenSelectionType selection)
        {
            this.selection = selection;
        }

        public HomeScreenSelectionMadeEventArgs(ProjectFile selectedProject)
        {
            this.selectedProject = selectedProject;
            selection = HomeScreenSelectionType.LoadProject;
        }
    }
}
