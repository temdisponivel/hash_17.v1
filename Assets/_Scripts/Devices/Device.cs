using System;
using System.Collections;
using System.Collections.Generic;
using Hash17.Devices.Firewalls;
using Hash17.Files;
using Hash17.Programs;
using Hash17.Utils;

namespace Hash17.Devices
{
    [Serializable]
    public class Device
    {
        public string UniqueId;
        public string Name;
        public FileSystem FileSystem;
        public virtual DeviceType DeviceType { get { return DeviceType.Normal; } }
        public FirewallType FirewallType;
        public Dictionary<ProgramId, int> SpecialPrograms;

        public virtual IEnumerator TryAccess(Action<bool, Device> callback)
        {
            yield return Alias.Board.Firewalls[FirewallType].Clone().Access(callback, this);
        }
    }
}
