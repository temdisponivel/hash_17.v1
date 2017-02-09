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

        private readonly Dictionary<string, string> _innerdiDictionary = new Dictionary<string, string>();
        public event Action<string> OnSystemVariableChange;

        public const string USERNAME = "USER_NAME";

        #endregion

        #region IDictionary

        public void Add(string key, string value)
        {
            _innerdiDictionary.Add(key, value);
            if (OnSystemVariableChange != null)
                OnSystemVariableChange(key);
        }

        public void Clear()
        {
            _innerdiDictionary.Clear();
        }
        
        public int Count
        {
            get { return _innerdiDictionary.Count; }
        }

        public bool ContainsKey(string key)
        {
            return _innerdiDictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _innerdiDictionary.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _innerdiDictionary.TryGetValue(key, out value);
        }
        
        public string this[string key]
        {
            get
            {
                if (!_innerdiDictionary.ContainsKey(key))
                    return string.Empty;

                return _innerdiDictionary[key];
            }
            set
            {
                _innerdiDictionary[key] = value;
                if (OnSystemVariableChange != null)
                    OnSystemVariableChange(key);
            }
        }

        #endregion
    }
}