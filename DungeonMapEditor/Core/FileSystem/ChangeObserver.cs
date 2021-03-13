using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.FileSystem
{
    class ChangeObserver<T> : IChangeObserver
    {
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private string propertyName;

        private T originalValue;
        private T currentValue;

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

                if (currentValue is IList currentList)
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
        public ChangeObserver(string propertyName, T currentValue)
        {
            this.propertyName = propertyName;
            originalValue = currentValue;
            this.currentValue = currentValue;
        }

        /// <summary>
        /// Set currentValue as originalValue
        /// </summary>
        public void Reset()
        {
            originalValue = currentValue;
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
        }
    }
}
