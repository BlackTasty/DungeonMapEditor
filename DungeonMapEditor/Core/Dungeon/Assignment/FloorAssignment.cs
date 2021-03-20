using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Observer;
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
    public class FloorAssignment : Assignment
    {
        private FloorPlan floorPlan;
        private FloorControl control;

        private double mRotationOverride;

        private double x;
        private double y;

        [JsonIgnore]
        public bool AnyUnsavedChanges => UnsavedChanges || (floorPlan?.UnsavedChanges ?? false);

        public string AssignedProjectName { get; set; }

        [JsonIgnore]
        public FloorPlan FloorPlan => floorPlan;

        [JsonIgnore]
        public FloorControl Control => control;

        public string FloorPlanFile => !string.IsNullOrWhiteSpace(floorPlan.Name) ? floorPlan.Name + ".json" : null;

        public double X 
        {
            get => x;
            set
            {
                changeManager.ObserveProperty(value);
                x = value;
            }
        }

        public double Y 
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
            get => Math.Round(mRotationOverride, 0);
            set
            {
                changeManager.ObserveProperty(value);
                mRotationOverride = value;
                InvokePropertyChanged();
                InvokePropertyChanged("RealRotation");
            }
        }

        [JsonIgnore]
        public double RealRotation => FloorPlan.Rotation + RotationOverride;

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public FloorAssignment(string assignedProjectName, string floorPlanFile, double x, double y, double rotationOverride, 
            string notes) : base(notes)
        {
            floorPlan = new FloorPlan(new FileInfo(Path.Combine(App.ProjectsPath, assignedProjectName, floorPlanFile)));
            AssignedProjectName = assignedProjectName;
            X = x;
            Y = y;
            RotationOverride = rotationOverride;
        }

        /// <summary>
        /// Used to create a new position assignment for a floor.
        /// </summary>
        /// <param name="floorPlan">The floor plan to assign</param>
        /// <param name="x">The X position</param>
        /// <param name="y">The Y position</param>
        public FloorAssignment(FloorPlan floorPlan, ProjectFile assignedProject, double x, double y) : this(floorPlan, assignedProject)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Used to create a new position assignment for a floor. Default position: X = 0; Y = 0
        /// </summary>
        /// <param name="floorPlan">The floor plan to assign</param>
        public FloorAssignment(FloorPlan floorPlan, ProjectFile assignedProject)
        {
            this.floorPlan = floorPlan;
            AssignedProjectName = assignedProject.FilePath;
        }

        public bool SetControl(FloorControl control)
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
