﻿using DungeonMapEditor.Core;
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
    /// Interaction logic for RoomControl.xaml
    /// </summary>
    public partial class RoomControl : Border
    {
        public event EventHandler<EventArgs> RoomMoved;

        protected bool isDragging;
        private Point clickPosition;

        public RoomControl(RoomPlan roomPlan, ProjectFile assignedProject) : this(roomPlan, assignedProject, new Point())
        {
        }

        public RoomControl(RoomAssignment roomAssignment)
        {
            InitializeComponent();
            RoomAssignment = roomAssignment;
        }

        public RoomControl(RoomPlan roomPlan, ProjectFile assignedProject, Point insertPoint)
        {
            InitializeComponent();
            RoomAssignment = new RoomAssignment(roomPlan, assignedProject, (int)insertPoint.X, (int)insertPoint.Y);
        }

        public RoomAssignment RoomAssignment
        {
            get => (DataContext as RoomControlViewModel).RoomAssignment;
            set
            {
                value.SetControl(this);
                (DataContext as RoomControlViewModel).RoomAssignment = value;
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


                RoomControlViewModel vm = DataContext as RoomControlViewModel;
                vm.RoomAssignment.X = (int)Helper.GetUpdatedAxisLocation_Snap(currentPosition.X, clickPosition.X, 
                                                            -1, vm.RoomAssignment.RoomPlan.TilesX * 25, 25) - 2;
                vm.RoomAssignment.Y = (int)Helper.GetUpdatedAxisLocation_Snap(currentPosition.Y, clickPosition.Y, 
                                                            -1, vm.RoomAssignment.RoomPlan.TilesY * 25, 25) - 2;

                Canvas.SetLeft(this, vm.RoomAssignment.X);
                Canvas.SetTop(this, vm.RoomAssignment.Y);
                OnRoomMoved(EventArgs.Empty);
            }
        }

        protected virtual void OnRoomMoved(EventArgs e)
        {
            RoomMoved?.Invoke(this, e);
        }
    }
}
