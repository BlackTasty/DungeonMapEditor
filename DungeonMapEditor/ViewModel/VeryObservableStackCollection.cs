using DungeonMapEditor.ViewModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    public class VeryObservableStackCollection<T> : VeryObservableCollection<T>
    {
        private int stackSize = 1;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="collectionName"><inheritdoc/></param>
        /// <param name="stackSize">The maximum allowed amount of items in this list. If amount is exceeded the last item is removed.</param>
        /// <param name="message"><inheritdoc/></param>
        public VeryObservableStackCollection(string collectionName, int stackSize, ViewModelMessage message = ViewModelMessage.None) : 
            base(collectionName, message)
        {
            this.stackSize = stackSize;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="collectionName"><inheritdoc/></param>
        /// <param name="stackSize">The maximum allowed amount of items in this list. If amount is exceeded the last item is removed.</param>
        /// <param name="item"><inheritdoc/></param>
        public VeryObservableStackCollection(string collectionName, int stackSize, T item) : base(collectionName, item)
        {
            this.stackSize = stackSize;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="collectionName"><inheritdoc/></param>
        /// <param name="stackSize">The maximum allowed amount of items in this list. If amount is exceeded the last item is removed.</param>
        public VeryObservableStackCollection(string collectionName, int stackSize) : base(collectionName, false)
        {
            this.stackSize = stackSize;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="collectionName"><inheritdoc/></param>
        /// <param name="stackSize">The maximum allowed amount of items in this list. If amount is exceeded the last item is removed.</param>
        /// <param name="items"><inheritdoc/></param>
        public VeryObservableStackCollection(string collectionName, int stackSize, List<T> items) : 
            base(collectionName, items.Take(stackSize))
        {
            this.stackSize = stackSize;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="collectionName"><inheritdoc/></param>
        /// <param name="stackSize">The maximum allowed amount of items in this list. If amount is exceeded the last item is removed.</param>S
        /// <param name="items"><inheritdoc/></param>
        public VeryObservableStackCollection(string collectionName, int stackSize, IEnumerable<T> items) : 
            base(collectionName, items.Take(stackSize))
        {
            this.stackSize = stackSize;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="items"><inheritdoc/></param>
        public override void Add(IEnumerable<T> items)
        {
            base.Add(items.Take(stackSize));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="items"><inheritdoc/></param>
        /// <param name="sort"><inheritdoc/></param>
        public override void Add(List<T> items, bool sort = false)
        {
            base.Add(items.Take(stackSize).ToList(), sort);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"><inheritdoc/></param>
        public override void Add(T item)
        {
            if (Items.Count >= stackSize)
            {
                Items.RemoveAt(Items.Count - 1);
                List<T> itemsCopy = this.Reverse().ToList();
                itemsCopy.Add(item);
                Clear();
                Add(itemsCopy);
            }
            else
            {
                base.Add(item);
            }
        }
    }
}
