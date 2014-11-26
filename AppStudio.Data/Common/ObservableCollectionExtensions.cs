using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AppStudio.Data
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRangeUnique<T>(this ObservableCollection<T> oldCollection, IEnumerable<T> newItems)
        {
            if (oldCollection != null && newItems != null)
            {
                // Gets all equal items to sync them.
                var modifiedItems = oldCollection.Intersect(newItems).OfType<ISyncItem<T>>();
                foreach (var modifiedItem in modifiedItems)
                {
                    T newItem = newItems.First(ci => ci.Equals(modifiedItem));
                    if (modifiedItem.NeedSync(newItem))
                    {
                        modifiedItem.Sync(newItem);
                    }
                }

                // Removes removed items in the old collection.
                var itemsToRemove = oldCollection.Except(newItems).ToList();
                foreach (var item in itemsToRemove)
                {
                    oldCollection.Remove(item);
                }

                // Adds newly added items.
                var itemsToAdd = newItems.Except(oldCollection);
                foreach (var item in itemsToAdd)
                {
                    oldCollection.AddSorted(item);
                }
            }
        }

        public static void AddSorted<T>(this ObservableCollection<T> collection, T item)
        {
            if (item is IComparable<T>)
            {
                for (var i = 0; i < collection.Count(); i++)
                {
                    IComparable<T> element = (IComparable<T>)collection.ElementAt(i);
                    if (element.CompareTo(item) >= 0)
                    {
                        collection.Insert(i, item);
                        return;
                    }
                }
            }

            collection.Add(item);
        }
    }
}