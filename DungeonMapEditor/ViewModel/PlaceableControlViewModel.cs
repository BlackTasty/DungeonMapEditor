using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class PlaceableControlViewModel : ViewModelBase
    {
        private PlaceableAssignment mPlaceableAssignment;

        public PlaceableAssignment PlaceableAssignment
        {
            get => mPlaceableAssignment;
            set
            {
                mPlaceableAssignment = value;
                InvokePropertyChanged();
            }
        }
    }
}
