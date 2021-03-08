using DungeonMapEditor.Core.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class FloorPlanViewModel : ViewModelBase
    {
        private FloorPlan mFloorPlan;

        public FloorPlan FloorPlan
        {
            get => mFloorPlan;
            set
            {
                mFloorPlan = value;
                InvokePropertyChanged();
            }
        }
    }
}
