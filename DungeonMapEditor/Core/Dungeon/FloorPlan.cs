using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon
{
    public class FloorPlan : BaseData
    {
        public event EventHandler<NameChangedEventArgs> FloorNameChanged;

        private VeryObservableCollection<RoomAssignment> mRoomAssignments = new VeryObservableCollection<RoomAssignment>("RoomAssignments");
        private string mFloorName;

        public VeryObservableCollection<RoomAssignment> RoomAssignments
        {
            get => mRoomAssignments;
            set
            {
                mRoomAssignments = value;
                InvokePropertyChanged();
            }
        }

        public string FloorName
        {
            get => mFloorName;
            set
            {
                mFloorName = value;
                InvokePropertyChanged();
            }
        }

        [JsonConstructor]
        public FloorPlan(string name, string description, double rotation, List<RoomAssignment> roomAssignments, string floorName) : 
            base(name, description, rotation)
        {
            mRoomAssignments.Add(roomAssignments);
            mFloorName = floorName;
        }

        public FloorPlan() : base("Untitled room", "", 0)
        {
            mFloorName = "";
        }

        public void Save(string parentPath = null)
        {
            if (!fromFile)
            {
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    throw new Exception("ParentPath needs to have a value if Collection file is being created!");
                }

                SaveFile(parentPath, JsonConvert.SerializeObject(this));
            }
            else
            {
                SaveFile(JsonConvert.SerializeObject(this));
            }
        }

        protected virtual void OnFloorNameChanged(NameChangedEventArgs e)
        {
            FloorNameChanged?.Invoke(this, e);
        }
    }
}
