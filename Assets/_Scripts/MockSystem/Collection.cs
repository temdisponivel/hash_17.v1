using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Hash17.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Hash17.MockSystem
{
    public class Collection<T> : NonUnitySingleton<Collection<T>>, ICollection<T>
        where T : IEquatable<int>
    {
        #region Properties

        protected readonly List<T> _all = new List<T>();
        protected readonly List<T> _availableItems = new List<T>();

        public event Action<T> OnItemAdded;

        #endregion

        #region Load

        public void Load(TextAsset textAsset)
        {
            var list = JsonConvert.DeserializeObject<List<T>>(textAsset.text, new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
            });
            for (int i = 0; i < list.Count; i++)
            {
                Add(list[i]);
            }
        }

        #endregion

        #region Accessors

        public virtual List<T> GetAvailableItems()
        {
            return new List<T>(_availableItems);
        }

        public virtual void AddItem(int uniqueId)
        {
            var item = _all.Find(i => i.Equals(uniqueId));
            Add(item);
        }

        #endregion

        #region ICollection

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _availableItems.GetEnumerator();
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _availableItems.GetEnumerator();
        }

        public virtual void Add(T item)
        {
            if (item == null)
            {
                Debug.LogError("TRYING TO ADD A INVALID ITEM OF TYPE {0}.".InLineFormat(typeof(T).Name));
            }

            _availableItems.Add(item);
            if (OnItemAdded != null)
                OnItemAdded(item);
        }

        public virtual void Clear()
        {
            _availableItems.Clear();
        }

        public virtual bool Contains(T item)
        {
            return _availableItems.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _availableItems.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(T item)
        {
            return _availableItems.Remove(item);
        }

        public virtual int Count
        {
            get { return _availableItems.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get { return _availableItems[index]; }
        }

        #endregion
    }
}