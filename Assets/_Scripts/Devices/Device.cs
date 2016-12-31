using System;
using System.Collections;
using System.Collections.Generic;
using Hash17.Files;
using Hash17.Programs;
using UnityEditor;

namespace Hash17.Devices
{
    [Serializable]
    public class Device
    {
        public string Name;
        public string Id;
        public FileSystem FileSystem;
        public virtual DeviceType DeviceType { get { return DeviceType.Normal; } }

        public virtual IEnumerator TryAccess(Action<bool, Device> callback)
        {
            yield return null;
            if (callback != null)
                callback(true, this);
        }
    }
}
