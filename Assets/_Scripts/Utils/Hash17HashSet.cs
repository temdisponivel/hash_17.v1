using System;
using System.Collections.Generic;

namespace Hash17.Utils
{
    public class Hash17HashSet<T>
    {
        #region Properties

        private HashSet<T> _hash;

        public event Action<T> OnItemAdded;
        public event Action<T> OnItemRemoved;

        #endregion

        #region Accessors

        public void Add(T item)
        {
            _hash.Add(item);
            if (OnItemAdded != null)
                OnItemAdded(item);
        }

        public bool Contains(T item)
        {
            return _hash.Contains(item);
        }

        public void Remove(T item)
        {
            _hash.Remove(item);
            if (OnItemRemoved != null)
                OnItemRemoved(item);
        }

        #endregion
    }
}