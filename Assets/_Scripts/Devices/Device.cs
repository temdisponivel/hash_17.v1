using System;
using System.Collections;
using System.Collections.Generic;
using Hash17.Devices.Firewalls;
using Hash17.Files;
using Hash17.MockSystem;
using Hash17.Programs;
using Hash17.Utils;
using UnityEngine;

namespace Hash17.Devices
{
    [Serializable]
    public class Device : IEquatable<int>
    {
        public int UniqueId { get; set; }
        public string Id;
        public string Name;
        public FileSystem FileSystem;
        public virtual DeviceType DeviceType { get { return DeviceType.Normal; } }
        public FirewallType FirewallType;
        public Dictionary<ProgramType, int> SpecialPrograms;
        public bool StartUnlocked { get; set; }

        public bool IsAvailable { get { return Application.isPlaying && Alias.Campaign.Info.UnlockedDevices.Contains(UniqueId); } }

        public virtual IEnumerator TryAccess(Action<bool, Device> callback)
        {
            yield return Alias.Game.Firewalls[FirewallType].Clone().Access(callback, this);
        }

        public void OnLoad()
        {
            if (StartUnlocked)
            {
                Alias.Campaign.Info.UnlockedDevices.Add(UniqueId);
                FileSystem.UnlockAvailables();
            }
        }

        #region IEquatable

        public bool Equals(int other)
        {
            return other == UniqueId;
        }

        #endregion
    }
}
