using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class CreateFloorViewModel : ViewModelBase
    {
        private string mFloorName = "EG";
        private bool mFloorNameExists;

        public string FloorName
        {
            get => mFloorName;
            set
            {
                mFloorName = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsValid");
            }
        }

        public bool FloorNameExists
        {
            get => mFloorNameExists;
            set
            {
                mFloorNameExists = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsValid");
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(mFloorName) && !FloorNameExists;
    }
}
