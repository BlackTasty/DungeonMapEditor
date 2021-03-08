using DungeonMapEditor.Core.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class PlaceableConfiguratorViewModel : ViewModelBase
    {
        private Placeable mPlaceable = new Placeable();

        public Placeable Placeable
        {
            get => mPlaceable;
            set
            {
                mPlaceable = value;
                InvokePropertyChanged();
            }
        }
    }
}
