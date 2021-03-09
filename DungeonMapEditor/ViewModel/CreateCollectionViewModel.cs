using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class CreateCollectionViewModel : ViewModelBase
    {
        private string mCollectionName = "Super awesome collection";

        public string CollectionName
        {
            get => mCollectionName;
            set
            {
                mCollectionName = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsValid");
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(mCollectionName);
    }
}
