using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Observer
{
    class Observer<T> : IObserver
    {
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private string propertyName;

        private T originalValue;
        private T currentValue;

        /// <summary>
        /// The original value
        /// </summary>
        public T OriginalValue => originalValue;

        public dynamic GetOriginalValue()
        {
            return originalValue;
        }

        /// <summary>
        /// The current value of the observed property
        /// </summary>
        public T CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                OnChangeObserved(new ChangeObservedEventArgs(UnsavedChanges, currentValue, this));
            }
        }

        /// <summary>
        /// Returns true if originalValue doesn't match currentValue
        /// </summary>
        public bool UnsavedChanges
        {
            get
            {
                if (originalValue == null && currentValue == null)
                {
                    return false;
                }

                if (currentValue is IVeryObservableCollection currentCollection)
                {
                    if (originalValue is IVeryObservableCollection originalCollection)
                    {
                        return originalCollection.Count != currentCollection.Count && currentCollection.AnyUnsavedChanges;
                    }

                    return true;
                }
                else if (currentValue is IBaseData currentBaseData)
                {
                    if (originalValue is IBaseData originalBaseData)
                    {
                        return currentBaseData.ChangeManager.Compare(originalBaseData.ChangeManager);
                    }
                    return true;
                }
                else if (currentValue is IList currentList)
                {
                    if (originalValue != null && originalValue is IList originalList)
                    {
                        //TODO implement list comparison
                        return false;
                    }

                    return true;
                }
                else
                {
                    return !originalValue.Equals(currentValue);
                }
            }
        }

        /// <summary>
        /// The name of the observed property
        /// </summary>
        public string PropertyName => propertyName;

        /// <summary>
        /// Initialize a new Observer to watch changes
        /// </summary>
        /// <param name="propertyName">The property name to observe</param>
        /// <param name="currentValue">The current value</param>
        public Observer(string propertyName, T currentValue)
        {
            this.propertyName = propertyName;
            this.currentValue = currentValue;
            Reset();
        }

        /// <summary>
        /// Set currentValue as originalValue
        /// </summary>
        public void Reset()
        {
            if (currentValue is IVeryObservableCollection currentCollection)
            {
                originalValue = (T)currentCollection.Copy();
            }
            else if (currentValue is IBaseData currentBaseData)
            {
                originalValue = (T)currentBaseData.Copy();
            }
            else
            {
                originalValue = currentValue;
            }
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
        }

        public override string ToString()
        {
            return "{ Type: " + originalValue.GetType().ToString() + " }";
        }
    }
}
