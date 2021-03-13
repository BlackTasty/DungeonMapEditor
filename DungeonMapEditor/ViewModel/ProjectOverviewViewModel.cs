using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.Core.FileSystem;
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
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private ProjectFile mProjectFile;
        private RoomAssignment mSelectedRoomAssignment;
        private FloorAssignment mSelectedFloorAssignment;

        public ProjectFile ProjectFile
        {
            get => mProjectFile;
            set
            {
                if (mProjectFile != null)
                {
                    mProjectFile.ProjectNameChanged -= ProjectFile_ProjectNameChanged;
                    mProjectFile.ChangeManager.ChangeObserved -= ChangeManager_ChangeObserved;
                }
                mProjectFile = value;
                mProjectFile.ProjectNameChanged += ProjectFile_ProjectNameChanged;
                mProjectFile.ChangeManager.ChangeObserved += ChangeManager_ChangeObserved;
                InvokePropertyChanged();
            }
        }

        private void ChangeManager_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            if (e.Observer.PropertyName == "Name")
            {
                OnChangeObserved(e);
            }
        }

        public RoomAssignment SelectedRoomAssignment
        {
            get => mSelectedRoomAssignment;
            set
            {
                mSelectedRoomAssignment = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsRoomAssignmentSelected");
            }
        }

        public bool IsRoomAssignmentSelected => mSelectedRoomAssignment != null;

        public FloorAssignment SelectedFloorAssignment
        {
            get => mSelectedFloorAssignment;
            set
            {
                mSelectedFloorAssignment = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsFloorAssignmentSelected");
            }
        }

        public bool IsFloorAssignmentSelected => mSelectedFloorAssignment != null;

        private void ProjectFile_ProjectNameChanged(object sender, NameChangedEventArgs e)
        {
            OnProjectNameChanged(e);
        }

        protected virtual void OnProjectNameChanged(NameChangedEventArgs e)
        {
            ProjectNameChanged?.Invoke(this, e);
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
        }
    }
}
