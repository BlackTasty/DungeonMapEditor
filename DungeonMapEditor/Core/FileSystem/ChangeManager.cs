using DungeonMapEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.FileSystem
{
    public class ChangeManager : ViewModelBase
    {
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private List<IChangeObserver> changeObservers = new List<IChangeObserver>();

        /// <summary>
        /// Returns true if any observer has unsaved changes
        /// </summary>
        public bool UnsavedChanges => changeObservers.Any(x => x.UnsavedChanges);

        ~ChangeManager()
        {
            foreach (var observer in changeObservers)
            {
                observer.ChangeObserved -= Observer_ChangeObserved;
            }
        }

        /// <summary>
        /// Observes changes on the specified property. Used both to register new observers and update existing ones
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The current value of the property</param>
        /// <param name="propertyName">The property to watch</param>
        public void ObserveProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                return;
            }

            if (changeObservers.FirstOrDefault(x => x.PropertyName == propertyName) is ChangeObserver<T> observer)
            {
                observer.CurrentValue = value;
            }
            else
            {
                observer = new ChangeObserver<T>(propertyName, value);
                observer.ChangeObserved += Observer_ChangeObserved;
                changeObservers.Add(observer);
            }
        }

        /// <summary>
        /// Set currentValue as originalValue on every observer
        /// </summary>
        public void ResetObservers()
        {
            foreach (var observer in changeObservers)
            {
                observer.Reset();
            }
            InvokePropertyChanged("UnsavedChanges");
        }

        private void Observer_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
            InvokePropertyChanged("UnsavedChanges");
        }
    }
}
