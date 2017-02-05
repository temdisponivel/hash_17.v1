using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Hash17.Utils
{
    public class PersistentSingleton<T> : Singleton<T>
        where T : PersistentSingleton<T>
    {
        public virtual void Awake()
        {
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }
    }
}
