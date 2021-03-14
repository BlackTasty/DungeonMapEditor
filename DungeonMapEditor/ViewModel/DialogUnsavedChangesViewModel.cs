using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class DialogUnsavedChangesViewModel : ViewModelBase
    {
        private string mObjectName;
        private string mName;

        public string ObjectName
        {
            get => mObjectName;
            set
            {
                mObjectName = value;
                InvokePropertyChanged();
            }
        }

        public string Name
        {
            get => mName;
            set
            {
                mName = value;
                InvokePropertyChanged();
            }
        }
    }
}
