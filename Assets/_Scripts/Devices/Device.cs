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
        public List<Program> Programs;
        public FileSystem FileSystem;

        public Device()
        {
            Programs = new List<Program>();
            FileSystem = new FileSystem();
        }

        public virtual DeviceType DeviceType
        {
            get
            {
                return DeviceType.Normal;
            }
        }

        public virtual IEnumerator TryAccess(Action<bool, Device> callback)
        {
            yield return null;
            if (callback != null)
                callback(true, this);
        }

#if UNITY_EDITOR
        public virtual void DrawDeviceInspector()
        {
            Id = EditorGUILayout.TextField("Id", Id);
            Name = EditorGUILayout.TextField("Name", Name);
        }
#endif
    }
}
