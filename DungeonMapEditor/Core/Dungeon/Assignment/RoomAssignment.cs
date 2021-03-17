using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.FileSystem;
using DungeonMapEditor.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Assignment
{
    public class RoomAssignment : Assignment
    {
        private RoomPlan roomPlan;
        private RoomControl control;

        private double mRotationOverride;
        private int mRoomNumberOverride;

        private int x;
        private int y;

        [JsonIgnore]
        public bool AnyUnsavedChanges => UnsavedChanges || (roomPlan?.UnsavedChanges ?? false);

        public string AssignedProjectPath { get; set; }

        [JsonIgnore]
        public RoomPlan RoomPlan => roomPlan;

        [JsonIgnore]
        public RoomControl Control => control;

        public string RoomPlanFile => !string.IsNullOrWhiteSpace(roomPlan.Name) ? roomPlan.Name + ".json" : null;

        public int X
        {
            get => x;
            set
            {
                changeManager.ObserveProperty(value);
                x = value;
            }
        }

        public int Y 
        {
            get => y;
            set
            {
                changeManager.ObserveProperty(value);
                y = value;
            }
        }

        public double RotationOverride
        {
            get => mRotationOverride;
            set
            {
                changeManager.ObserveProperty(value);
                mRotationOverride = value;
                InvokePropertyChanged();
                InvokePropertyChanged("RealRotation");
            }
        }

        [JsonIgnore]
        public double RealRotation => RoomPlan.Rotation + RotationOverride;

        public int RoomNumberOverride
        {
            get => mRoomNumberOverride;
            set
            {
                changeManager.ObserveProperty(value);
                mRoomNumberOverride = value;
                InvokePropertyChanged();
                InvokePropertyChanged("RealRoomNumber");
            }
        }

        [JsonIgnore]
        public int RealRoomNumber => RoomNumberOverride > 0 ? RoomNumberOverride : RoomPlan.RoomNumber;

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public RoomAssignment(string assignedProjectPath, string roomPlanFile, int x, int y, double rotationOverride,
            int roomNumberOverride, string notes) : base(notes)
        {
            roomPlan = new RoomPlan(new FileInfo(Path.Combine(assignedProjectPath, roomPlanFile)));
            AssignedProjectPath = assignedProjectPath;
            X = x;
            Y = y;
            RotationOverride = rotationOverride;
            RoomNumberOverride = roomNumberOverride;
            roomPlan.RoomNumberOverride = roomNumberOverride;
        }

        /// <summary>
        /// Used to create a new position assignment for a room plan.
        /// </summary>
        /// <param name="roomPlan">The room plan to assign</param>
        /// <param name="x">The X position</param>
        /// <param name="y">The Y position</param>
        public RoomAssignment(RoomPlan roomPlan, ProjectFile assignedProject, int x, int y) : this(roomPlan, assignedProject)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Used to create a new position assignment for a room plan. Default position: X = 0; Y = 0
        /// </summary>
        /// <param name="roomPlan">The room plan to assign</param>
        public RoomAssignment(RoomPlan roomPlan, ProjectFile assignedProject)
        {
            this.roomPlan = roomPlan;
            AssignedProjectPath = assignedProject.FilePath;
        }

        public bool SetControl(RoomControl control)
        {
            if (this.control != null)
            {
                return false;
            }

            this.control = control;
            return true;
        }

        protected override void OnChangeObserved(ChangeObservedEventArgs e)
        {
            base.OnChangeObserved(new ChangeObservedEventArgs(AnyUnsavedChanges, e.NewValue, e.Observer));
        }
    }
}
