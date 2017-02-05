using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.MockSystem
{
    public class SystemVariables
    {
        #region Properties

        private readonly Dictionary<SystemVariableType, string> _innerdiDictionary = new Dictionary<SystemVariableType, string>();

        public event Action<SystemVariableType> OnSystemVariableChange;

        #endregion

        #region IDictionary
        
        public void Add(KeyValuePair<SystemVariableType, string> item)
        {
            _innerdiDictionary.Add(item.Key, item.Value);
            if (OnSystemVariableChange != null)
                OnSystemVariableChange(item.Key);
        }

        public void Clear()
        {
            _innerdiDictionary.Clear();
        }

        public bool Contains(KeyValuePair<SystemVariableType, string> item)
        {
            return _innerdiDictionary.Contains(item);
        }
        
        public bool Remove(KeyValuePair<SystemVariableType, string> item)
        {
            return _innerdiDictionary.Remove(item.Key);
        }

        public int Count
        {
            get { return _innerdiDictionary.Count; }
        }
        
        public void Add(SystemVariableType key, string value)
        {
            _innerdiDictionary.Add(key, value);
        }

        public bool ContainsKey(SystemVariableType key)
        {
            return _innerdiDictionary.ContainsKey(key);
        }

        public bool Remove(SystemVariableType key)
        {
            return _innerdiDictionary.Remove(key);
        }

        public bool TryGetValue(SystemVariableType key, out string value)
        {
            return _innerdiDictionary.TryGetValue(key, out value);
        }

        public string this[SystemVariableType key]
        {
            get
            {
                if (!_innerdiDictionary.ContainsKey(key))
                    this[key] = PlayerPrefs.GetString(key.ToString(), "default");
                return _innerdiDictionary[key];
            }
            set
            {
                _innerdiDictionary[key] = value;
                if (OnSystemVariableChange != null)
                    OnSystemVariableChange(key);
            }
        }

        public ICollection<SystemVariableType> Keys
        {
            get { return _innerdiDictionary.Keys; }
        }

        public ICollection<string> Values
        {
            get { return _innerdiDictionary.Values; }
        }

        #endregion
    }
}