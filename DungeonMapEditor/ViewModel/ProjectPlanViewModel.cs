using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Enum;
using DungeonMapEditor.ViewModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DungeonMapEditor.ViewModel
{
    class ProjectPlanViewModel : ViewModelBase
    {
        private VeryObservableCollection<FloorPlan> mAvailableFloors = new VeryObservableCollection<FloorPlan>("AvailableFloors");

        private ProjectFile mProjectFile;
        private FloorAssignment mSelectedFloorAssignment;
        private int mSelectedTabIndex;

        public ProjectPlanViewModel()
        {
            Mediator.Instance.Register(o =>
            {
                LoadAvailableFloors();
            }, ViewModelMessage.FloorsChanged);
        }

        private void LoadAvailableFloors()
        {
            List<FloorPlan> floorPlans = new List<FloorPlan>();

            foreach (var floorPlanAssignment in ProjectFile.FloorPlans)
            {
                if (floorPlanAssignment.FloorPlan != null)
                {
                    floorPlans.Add(floorPlanAssignment.FloorPlan);
                }
            }

            AvailableFloors.Clear();
            AvailableFloors.Add(floorPlans);
        }

        public VeryObservableCollection<FloorPlan> AvailableFloors
        {
            get => mAvailableFloors;
            set
            {
                mAvailableFloors = value;
                InvokePropertyChanged();
            }
        }

        public ProjectFile ProjectFile
        {
            get => mProjectFile;
            set
            {
                mProjectFile = value;
                InvokePropertyChanged();
                LoadAvailableFloors();
            }
        }

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

        public bool IsFloorAssignmentSelected => SelectedFloorAssignment != null;

        public int SelectedTabIndex
        {
            get => mSelectedTabIndex;
            set
            {
                mSelectedTabIndex = value;
                InvokePropertyChanged();
            }
        }
    }
}
