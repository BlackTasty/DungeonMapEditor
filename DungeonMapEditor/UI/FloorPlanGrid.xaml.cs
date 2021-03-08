using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Events;
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

namespace DungeonMapEditor.UI
{
    /// <summary>
    /// Interaction logic for FloorPlanGrid.xaml
    /// </summary>
    public partial class FloorPlanGrid : DockPanel
    {
        public event EventHandler<NameChangedEventArgs> FloorNameChanged;

        public FloorPlanGrid() : this(new FloorPlan())
        {
        }

        public FloorPlanGrid(FloorPlan floorPlan)
        {
            InitializeComponent();
            floorPlan.FloorNameChanged += FloorPlan_FloorNameChanged;
            floorPlan.NameChanged += FloorPlan_FloorNameChanged;

            (DataContext as FloorPlanViewModel).FloorPlan = floorPlan;
        }

        private void FloorPlan_FloorNameChanged(object sender, NameChangedEventArgs e)
        {
            OnFloorNameChanged(e);
        }

        protected virtual void OnFloorNameChanged(NameChangedEventArgs e)
        {
            FloorNameChanged?.Invoke(this, e);
        }
    }
}
