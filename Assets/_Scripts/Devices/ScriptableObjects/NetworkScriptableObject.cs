using UnityEngine;

namespace Hash17.Devices.ScriptableObjects
{
    public class NetworkScriptableObject : ScriptableObject
    {
        public string Name;
        public string Identifier;
        public DeviceScriptableObject[] Devices;
    }
}
