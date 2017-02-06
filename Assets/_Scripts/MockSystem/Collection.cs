using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Hash17.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Hash17.MockSystem
{
    public class Collection<T> : NonUnitySingleton<Collection<T>>
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
        
        #endregion

        #region ICollection
        
        public virtual void Add(T item)
        {
            if (item == null)
            {
                Debug.LogError("TRYING TO ADD A INVALID ITEM OF TYPE {0}.".InLineFormat(typeof(T).Name));
            }

            _all.Add(item);
            _availableItems.Add(item);
            if (OnItemAdded != null)
                OnItemAdded(item);
        }
        
        public virtual bool Contains(T item)
        {
            return _availableItems.Contains(item);
        }
        
        public virtual int Count
        {
            get { return _availableItems.Count; }
        }
        
        public T this[int index]
        {
            get { return _availableItems[index]; }
        }

        #endregion
    }
}