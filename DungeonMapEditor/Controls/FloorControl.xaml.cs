using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
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
    /// Interaction logic for FloorControl.xaml
    /// </summary>
    public partial class FloorControl : Border
    {
        public event EventHandler<EventArgs> RoomMoved;

        protected bool isDragging;
        private Point clickPosition;
        private ProjectFile assignedProject;

        public FloorControl(FloorPlan floorPlan, ProjectFile assignedProject, double planOffset) : 
            this(floorPlan, assignedProject, new Point(), planOffset)
        {
        }

        public FloorControl(FloorAssignment floorAssignment, ProjectFile assignedProject, double planOffset, bool showNoteIcon = true)
        {
            InitializeComponent();
            FloorAssignment = floorAssignment;
            this.assignedProject = assignedProject;
            Width = floorAssignment.FloorPlan.FloorPlanImage.Width + planOffset;
            Height = floorAssignment.FloorPlan.FloorPlanImage.Height + planOffset;

            if (!showNoteIcon)
            {
                noteIcon.Source = null;
            }
        }

        public FloorControl(FloorPlan floorPlan, ProjectFile assignedProject, Point insertPoint, double planOffset) : 
            this(new FloorAssignment(floorPlan, assignedProject, (int)insertPoint.X, (int)insertPoint.Y), assignedProject, planOffset)
        {
        }

        public FloorAssignment FloorAssignment
        {
            get => (DataContext as FloorControlViewModel).FloorAssignment;
            set
            {
                value.SetControl(this);
                (DataContext as FloorControlViewModel).FloorAssignment = value;
            }
        }

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            var draggable = sender as Border;
            clickPosition = e.GetPosition(this);
            draggable.CaptureMouse();
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggable = sender as Border;
            draggable.ReleaseMouseCapture();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            var draggable = sender as Border;

            if (isDragging && draggable != null)
            {
                Point currentPosition = e.GetPosition(this.Parent as UIElement);

                FloorControlViewModel vm = DataContext as FloorControlViewModel;
                Size documentSize = assignedProject.DocumentSize;

                vm.FloorAssignment.X = (int)Helper.GetUpdatedAxisLocation_Snap(currentPosition.X, clickPosition.X, documentSize.Width,
                                                            vm.FloorAssignment.FloorPlan.FloorPlanImage.Width, 25);
                vm.FloorAssignment.Y = (int)Helper.GetUpdatedAxisLocation_Snap(currentPosition.Y, clickPosition.Y, documentSize.Height,
                                                            vm.FloorAssignment.FloorPlan.FloorPlanImage.Height, 25);

                Canvas.SetLeft(this, vm.FloorAssignment.X);
                Canvas.SetTop(this, vm.FloorAssignment.Y);
                OnRoomMoved(EventArgs.Empty);
            }
        }

        protected virtual void OnRoomMoved(EventArgs e)
        {
            RoomMoved?.Invoke(this, e);
        }
    }
}
