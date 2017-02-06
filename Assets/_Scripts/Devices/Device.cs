using System;
using System.Collections;
using System.Collections.Generic;
using Hash17.Devices.Firewalls;
using Hash17.Files;
using Hash17.MockSystem;
using Hash17.Programs;
using Hash17.Utils;

namespace Hash17.Devices
{
    [Serializable]
    public class Device : IEquatable<int>
    {
        public int UniqueId { get { return UniqueId.GetHashCode(); } }
        public string Id;
        public string Name;
        public FileSystem FileSystem;
        public virtual DeviceType DeviceType { get { return DeviceType.Normal; } }
        public FirewallType FirewallType;
        public Dictionary<ProgramType, int> SpecialPrograms;

        public bool IsAvailable { get { return Alias.Campaign.Info.UnlockedDevices.Contains(UniqueId); } }

        public virtual IEnumerator TryAccess(Action<bool, Device> callback)
        {
            yield return Alias.Game.Firewalls[FirewallType].Clone().Access(callback, this);
        }

        #region IEquatable

        public bool Equals(int other)
        {
            return other == UniqueId;
        }

        #endregion
    }
}
