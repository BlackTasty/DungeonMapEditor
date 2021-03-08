using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
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

        private Tile mTile;

        public Tile Tile
        {
            get => mTile;
            set
            {
                mTile = value;
                InvokePropertyChanged();
                OnTileChanged(EventArgs.Empty);
            }
        }

        public TileControlViewModel()
        {
            Mediator.Instance.Register(o =>
            {
                if (o is Tile tile)
                {
                    mTile = tile;
                    InvokePropertyChanged("Tile");
                }
            }, ViewModelMessage.TileChanged);
        }

        protected virtual void OnTileChanged(EventArgs e)
        {
            TileChanged?.Invoke(this, e);
        }
    }
}
