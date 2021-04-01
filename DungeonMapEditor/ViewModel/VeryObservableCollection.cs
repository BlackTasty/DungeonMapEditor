using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Observer;
using DungeonMapEditor.ViewModel.Communication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    /// <summary>
    /// This class is an extension to the <see cref="ObservableCollection{T}"/>. 
    /// (this removes the need to call the "OnNotifyPropertyChanged" event every time you add, edit or remove an entry.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class VeryObservableCollection<T> : ObservableCollection<T>, INotifyPropertyChanged, IVeryObservableCollection
    {
        public event EventHandler<EventArgs> ObserveChanges;

        protected bool autoSort;
        protected List<string> triggerAlso = new List<string>();
        protected bool observeChanges = true; //If this flag is set to false the collection won't fire CollectionChanged events
        protected ViewModelMessage message;
        protected ObserverManager changeManager;

        new event PropertyChangedEventHandler PropertyChanged;

        public string CollectionName { get; }

        [JsonIgnore]
        public bool UnsavedChanged
        {
            get
            {
                foreach (var observer in changeManager.ChangeObservers)
                {
                    if (observer is Observer<IVeryObservableCollection>)
                    {
                        continue;
                    }

                    if (observer.UnsavedChanges)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [JsonIgnore]
        public bool AnyUnsavedChanges
        {
            get
            {
                foreach (var item in this)
                {
                    if (item is IBaseData data)
                    {
                        return data.UnsavedChanges;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Initializes the collection with the specified name.
        /// </summary>
        /// <param name="collectionName">The name of the collection (must match the property name!)</param>
        public VeryObservableCollection(string collectionName,
            ObserverManager changeManager = null, ViewModelMessage message = ViewModelMessage.None)
        {
            CollectionName = collectionName;
            CollectionChanged += Collection_CollectionChanged;
            this.message = message;
            this.changeManager = changeManager;
        }

        /// <summary>
        /// Initializes the collection with the specified name and if auto-sorting is enabled.
        /// </summary>
        /// <param name="collectionName">The name of the collection (must match the property name!)</param>
        /// <param name="autoSort">If true the list is sorted after every change</param>
        /// <param name="firstKeepPosition">If true the first item in the list is not affected by sorting</param>
        public VeryObservableCollection(string collectionName, bool autoSort,
            ObserverManager changeManager = null) : this(collectionName, changeManager)
        {
            this.autoSort = autoSort;
        }

        /// <summary>
        /// Tell this <see cref="VeryObservableCollection{T}"/> to trigger the PropertyChanged event on another property.
        /// </summary>
        /// <param name="propertyName">The name of the property (the property which is exposed to XAML)</param>
        public void TriggerAlso(string propertyName)
        {
            if (!triggerAlso.Contains(propertyName))
            {
                triggerAlso.Add(propertyName);
            }
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (observeChanges)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Adds multiple objects to the end of the <see cref="Collection{T}"/>.
        /// </summary>
        /// <param name="items">The objects to be added to the end of the <see cref="Collection{T}"/>.</param>
        public virtual void Add(IEnumerable<T> items)
        {
            observeChanges = false;
            foreach (T item in items)
            {
                Add(item);
            }

            observeChanges = true;
            changeManager?.ObserveProperty(this, CollectionName);
        }

        /// <summary>
        /// Adds multiple objects to the end of the <see cref="Collection{T}"/>.
        /// </summary>
        /// <param name="items">The objects to be added to the end of the <see cref="Collection{T}"/>.</param>
        /// <param name="sort">If true, the collection is sorted after adding all items</param>
        public virtual void Add(List<T> items, bool sort = true)
        {
            if (items == null)
            {
                return;
            }
            observeChanges = false;
            foreach (T item in items)
            {
                if (sort)
                {
                    Add(item);
                }
                else
                {
                    base.Add(item);
                }
            }

            observeChanges = true;
            changeManager?.ObserveProperty(this, CollectionName);
        }

        public virtual new void Add(T item)
        {
            base.Add(item);
            if (autoSort)
            {
                List<T> lookupList = Items.OrderBy(x => x.ToString(), StringComparer.CurrentCultureIgnoreCase)
                    .ToList();
                foreach (T obj in lookupList)
                {
                    if (obj.Equals(item))
                    {
                        Remove(item);
                        Insert(lookupList.IndexOf(obj), obj);
                    }
                }
            }

            if (observeChanges)
            {
                changeManager?.ObserveProperty(this, CollectionName);
            }
        }

        public new void Clear()
        {
            try
            {
                base.Clear();
            }
            catch
            {
                observeChanges = false;
                for (int i = 0; i < Count; i++)
                {
                    RemoveAt(0);
                }
                observeChanges = true;
            }
        }

        public void Reset(T value)
        {
            Clear();
            Add(value);
        }

        public void Reset(IEnumerable<T> values)
        {
            Clear();
            Add(values);
        }

        /// <summary>
        /// Removes all items starting at the given index
        /// </summary>
        /// <param name="startIndex">Defines at which index the collection should remove all items</param>
        public void RemoveRange(int startIndex)
        {
            RemoveRange(startIndex, Count);
        }

        /// <summary>
        /// Removes all items in a range starting at the given index
        /// </summary>
        /// <param name="startIndex">Defines at which index the collection should remove all items</param>
        /// <param name="range">How many items shall be removed beginning from the start index</param>
        public void RemoveRange(int startIndex, int range)
        {
            observeChanges = false;
            for (int i = startIndex; i < range; i++)
            {
                RemoveAt(startIndex);
            }
            observeChanges = true;

            changeManager?.ObserveProperty(this, CollectionName);
        }

        public void Refresh()
        {
            if (message != ViewModelMessage.None)
            {
                Mediator.Instance.NotifyColleagues(message, this);
            }
            OnObserveChanges(EventArgs.Empty);

            OnPropertyChanged(this, new PropertyChangedEventArgs(CollectionName));
            if (triggerAlso.Count > 0)
            {
                foreach (string trigger in triggerAlso)
                {
                    OnPropertyChanged(this, new PropertyChangedEventArgs(trigger));
                }
            }

            changeManager?.ObserveProperty(this, CollectionName);
        }

        public IVeryObservableCollection Copy()
        {
            VeryObservableCollection<T> copy = new VeryObservableCollection<T>("");
            copy.Add(this.ToList());

            return copy;
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        protected virtual void OnObserveChanges(EventArgs e)
        {
            ObserveChanges?.Invoke(this, e);
        }
    }
}