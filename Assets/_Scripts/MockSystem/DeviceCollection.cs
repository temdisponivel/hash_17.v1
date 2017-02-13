using System;
using System.Collections.Generic;
using Hash17.Devices;
using Hash17.Utils;

namespace Hash17.MockSystem
{
    public class DeviceCollection : Collection<Device>
    {
        #region Properties

        public static Device CurrentDevice { get; private set; }
        public static FileSystem FileSystem { get { return CurrentDevice.FileSystem; } }
        public event Action OnCurrentDeviceChange;
        
        #endregion

        #region Current Device

        public void ChangeCurrentDevice(Device newCurrent)
        {
            CurrentDevice = newCurrent;
            if (OnCurrentDeviceChange != null)
                OnCurrentDeviceChange();
        }

        #endregion

        #region Helper

        public bool GetDeviceByIdForced(int uniqueId, out Device device)
        {
            device = _all.Find(d => d.UniqueId == uniqueId);
            return device != null;
        }

        public bool GetDeviceByIdForced(string id, out Device device)
        {
            device = _all.Find(d => d.Id == id);
            return device != null;
        }

        public bool GetDeviceById(int uniqueId, out Device device)
        {
            return GetDeviceByIdForced(uniqueId, out device) && device.IsAvailable;
        }

        public bool GetDeviceById(string id, out Device device)
        {
            return GetDeviceByIdForced(id, out device) && device.IsAvailable;
        }

        public override void Add(Device item)
        {
            base.Add(item);
            if (item.StartUnlocked)
            {
                Alias.Campaign.Info.UnlockedDevices.Add(item.UniqueId);
                item.FileSystem.UnlockAvailables();
            }
        }

        #endregion

    }
}