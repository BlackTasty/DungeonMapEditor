using DungeonMapEditor.Core.Dungeon.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class FloorControlViewModel : ViewModelBase
    {
        private FloorAssignment mFloorAssignment;

        public FloorAssignment FloorAssignment
        {
            get => mFloorAssignment;
            set
            {
                mFloorAssignment = value;
                InvokePropertyChanged();
            }
        }
    }
}
