using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.ViewModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class FloorPlanViewModel : ViewModelBase
    {
        private VeryObservableCollection<RoomPlan> mAvailableRooms = new VeryObservableCollection<RoomPlan>("AvailableRooms");
        private RoomAssignment mSelectedRoomAssignment;
        private FloorPlan mFloorPlan;
        private int mSelectedTabIndex;

        public ProjectFile ProjectFile { get; set; }

        public FloorPlanViewModel()
        {
            Mediator.Instance.Register(o =>
            {
                LoadAvailableRooms();
            }, ViewModelMessage.RoomsChanged);
        }

        private void LoadAvailableRooms()
        {
            List<RoomPlan> roomPlans = new List<RoomPlan>();

            foreach (var roomPlanAssignment in ProjectFile.RoomPlans.ToList())
            {
                if (roomPlanAssignment.RoomPlan != null)
                {
                    roomPlans.Add(roomPlanAssignment.RoomPlan);
                }
            }

            AvailableRooms.Clear();
            AvailableRooms.Add(roomPlans);
        }

        public VeryObservableCollection<RoomPlan> AvailableRooms
        {
            get => mAvailableRooms;
            set
            {
                mAvailableRooms = value;
                InvokePropertyChanged();
            }
        }

        public FloorPlan FloorPlan
        {
            get => mFloorPlan;
            set
            {
                mFloorPlan = value;
                InvokePropertyChanged();
                LoadAvailableRooms();
            }
        }

        public int SelectedTabIndex
        {
            get => mSelectedTabIndex;
            set
            {
                mSelectedTabIndex = value;
                InvokePropertyChanged();
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

        public bool IsRoomAssignmentSelected => SelectedRoomAssignment != null;
    }
}
