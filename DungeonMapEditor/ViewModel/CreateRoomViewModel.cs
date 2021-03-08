using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class CreateRoomViewModel : ViewModelBase
    {
        private string mRoomName = "Entrance";
        private int mTilesX = 10;
        private int mTilesY = 5;

        public string RoomName
        {
            get => mRoomName;
            set
            {
                mRoomName = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsValid");
            }
        }

        public int TilesX
        {
            get => mTilesX;
            set
            {
                mTilesX = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsValid");
            }
        }

        public int TilesY
        {
            get => mTilesY;
            set
            {
                mTilesY = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsValid");
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(mRoomName) && mTilesX > 0 && mTilesY > 0;
    }
}
