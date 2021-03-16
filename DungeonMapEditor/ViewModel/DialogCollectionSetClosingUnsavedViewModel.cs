using DungeonMapEditor.Core.Dungeon.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class DialogCollectionSetClosingUnsavedViewModel : ViewModelBase
    {
        private CollectionSet mUnsavedCollection;

        public CollectionSet UnsavedCollection
        {
            get => mUnsavedCollection;
            set
            {
                mUnsavedCollection = value;
                InvokePropertyChanged();
            }
        }
    }
}
