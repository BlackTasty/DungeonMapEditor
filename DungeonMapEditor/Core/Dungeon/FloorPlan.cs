using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.ViewModel;
using DungeonMapEditor.ViewModel.Communication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon
{
    public class FloorPlan : BaseData<FloorPlan>
    {
        public event EventHandler<NameChangedEventArgs> FloorNameChanged;

        private VeryObservableCollection<RoomAssignment> mRoomAssignments = new VeryObservableCollection<RoomAssignment>("RoomAssignments",
                                                                                ViewModelMessage.RoomsChanged);
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

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public FloorPlan(string name, string description, double rotation, string guid, List<RoomAssignment> roomAssignments, string floorName) : 
            base(name, description, rotation, guid)
        {
            RoomAssignments.Clear();
            RoomAssignments.Add(roomAssignments);
            mFloorName = floorName;
        }

        /// <summary>
        /// Loads an existing <see cref="FloorPlan"/> from a json file.
        /// </summary>
        /// <param name="fi">A <see cref="FileInfo"/> object containing the path to the floor plan</param>
        public FloorPlan(FileInfo fi) : base(fi)
        {
            Load();
        }

        /// <summary>
        /// Used to create a new <see cref="FloorPlan"/>
        /// </summary>
        /// <param name="floorName">The name of this floor</param>
        public FloorPlan(string floorName) : base(floorName, "", 0)
        {
            mFloorName = "";
        }

        public void Load()
        {
            if (!filePath.EndsWith("floors"))
            {
                filePath = Path.Combine(filePath, "floors");
            }
            FloorPlan floorPlan = LoadFile();

            Name = floorPlan.Name;
            FloorName = floorPlan.Name;
            RoomAssignments.Clear();
            RoomAssignments.Add(floorPlan.RoomAssignments);
        }

        public void Save(string parentPath = null)
        {
            if (!fromFile)
            {
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    throw new Exception("ParentPath needs to have a value if Collection file is being created!");
                }

                fileName = Name + ".json";
                SaveFile(parentPath, this);
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
