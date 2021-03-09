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
    /// Interaction logic for PlaceableControl.xaml
    /// </summary>
    public partial class PlaceableControl : Border
    {
        public event EventHandler<EventArgs> PlaceableMoved;

        protected bool isDragging;
        private Point clickPosition;
        private Size canvasBounds;

        public PlaceableControl(Placeable placeable, Size canvasBounds) : this(placeable, canvasBounds, new Point())
        {
        }

        public PlaceableControl(PlaceableAssignment placeableAssignment)
        {
            InitializeComponent();
            PlaceableAssignment = placeableAssignment;
        }

        public PlaceableControl(Placeable placeable, Size canvasBounds, Point insertPoint)
        {
            InitializeComponent();
            this.canvasBounds = canvasBounds;
            PlaceableAssignment = new PlaceableAssignment(placeable, insertPoint.X, insertPoint.Y);
        }

        public PlaceableAssignment PlaceableAssignment
        {
            get => (DataContext as PlaceableControlViewModel).PlaceableAssignment;
            set
            {
                value.SetControl(this);
                (DataContext as PlaceableControlViewModel).PlaceableAssignment = value;
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


                PlaceableControlViewModel vm = DataContext as PlaceableControlViewModel;
                vm.PlaceableAssignment.PositionX = Helper.GetUpdatedAxisLocation(currentPosition.X, clickPosition.X);
                vm.PlaceableAssignment.PositionY = Helper.GetUpdatedAxisLocation(currentPosition.Y, clickPosition.Y);
                //vm.PlaceableAssignment.PositionX = Helper.GetUpdatedAxisLocation(currentPosition.X, clickPosition.X, canvasBounds.Width, Width);
                //vm.PlaceableAssignment.PositionY = Helper.GetUpdatedAxisLocation(currentPosition.Y, clickPosition.Y, canvasBounds.Height, Height);

                Canvas.SetLeft(this, vm.PlaceableAssignment.PositionX);
                Canvas.SetTop(this, vm.PlaceableAssignment.PositionY);
                OnPlaceableMoved(EventArgs.Empty);
            }
        }

        protected virtual void OnPlaceableMoved(EventArgs e)
        {
            PlaceableMoved?.Invoke(this, e);
        }

        private void Border_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {

            }
        }
    }
}
