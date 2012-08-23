using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace GreenField.Gadgets.Helpers
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// To suppress the CollectionChanged Event
        /// </summary>
        private bool _disableNotifications = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addList"></param>
        public RangeObservableCollection(List<T> addList)
        {
            if (addList != null)
            {
                _disableNotifications = true;
                foreach (T item in addList)
                {
                    Add(item);
                }
                _disableNotifications = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public RangeObservableCollection()
        {

        }

        /// <summary>
        /// Collection Changed Event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (!_disableNotifications)
                    base.OnCollectionChanged(e);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Method to add Multiple Items
        /// </summary>
        /// <param name="objList"></param>
        public void AddRange(List<T> objList)
        {
            if (objList != null)
            {
                _disableNotifications = true;
                foreach (T item in objList)
                {
                    Add(item);
                }
                _disableNotifications = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Method to remove Multiple Items
        /// </summary>
        /// <param name="objList"></param>
        public void RemoveRange(List<T> objList)
        {
            if (objList != null)
            {
                _disableNotifications = true;
                foreach (T item in objList)
                {
                    Remove(item);
                }
                _disableNotifications = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}