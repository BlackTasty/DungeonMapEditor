using DungeonMapEditor.Core.Dungeon.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class RoomControlViewModel : ViewModelBase
    {
        private RoomAssignment mRoomAssignment;

        public RoomAssignment RoomAssignment
        {
            get => mRoomAssignment;
            set
            {
                mRoomAssignment = value;
                InvokePropertyChanged();
            }
        }
    }
}
