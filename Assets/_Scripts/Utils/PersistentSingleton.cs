using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hash17.Utils
{
    public class PersistentSingleton<T> : Singleton<T>
        where T : PersistentSingleton<T>
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
