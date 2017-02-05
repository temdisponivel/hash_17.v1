using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MockSystem;
using UnityEngine;

namespace Hash17.Utils
{
    [Serializable]
    public class Tuple<T1, T2>
    {
        public T1 Key;
        public T2 Value;
    }
}
