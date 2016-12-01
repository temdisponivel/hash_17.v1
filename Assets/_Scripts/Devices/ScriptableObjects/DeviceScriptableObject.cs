using Hash17.Files;
using Hash17.Files.SO;
using Hash17.Programs;
using UnityEngine;

namespace Hash17.Devices.ScriptableObjects
{
    public class DeviceScriptableObject : ScriptableObject
    {
        public string Name;
        public string Address;
        public FileSystemScriptableObject FileSystem;
        public ProgramScriptableObject[] Programs;
    }
}