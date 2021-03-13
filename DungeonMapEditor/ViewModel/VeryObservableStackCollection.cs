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
        private int limit = 1;

        public int Limit => limit;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="collectionName"><inheritdoc/></param>
        /// <param name="limit">The maximum allowed amount of items in this list. If amount is exceeded the last item is removed.</param>
        public VeryObservableStackCollection(string collectionName, int limit) : base(collectionName, false)
        {
            this.limit = limit;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="items"><inheritdoc/></param>
        public override void Add(IEnumerable<T> items)
        {
            base.Add(items.Take(limit));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="items"><inheritdoc/></param>
        /// <param name="sort"><inheritdoc/></param>
        public override void Add(List<T> items, bool sort = false)
        {
            base.Add(items.Take(limit).ToList(), sort);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"><inheritdoc/></param>
        public override void Add(T item)
        {
            if (Items.Count >= limit)
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
