using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.ViewModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class TileControlViewModel : ViewModelBase
    {
        public event EventHandler<EventArgs> TileChanged;

        private TileAssignment mTileAssignment;

        public TileAssignment TileAssignment
        {
            get => mTileAssignment;
            set
            {
                mTileAssignment = value;
                InvokePropertyChanged();
                OnTileChanged(EventArgs.Empty);
            }
        }

        public TileControlViewModel()
        {
        }

        protected virtual void OnTileChanged(EventArgs e)
        {
            TileChanged?.Invoke(this, e);
        }
    }
}
