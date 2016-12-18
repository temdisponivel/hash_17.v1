using System.Collections.Generic;
using UnityEngine;

namespace Hash17.Devices
{
    [CreateAssetMenu(fileName = "Device Collection", menuName = "Hash17/Create/Device collection")]
    public class DeviceCollectionScriptableObject : ScriptableObject
    {
        public List<Device> Devices;
    }
}