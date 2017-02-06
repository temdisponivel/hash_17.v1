using System;
using System.Collections.Generic;
using Hash17.Devices;

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

        public bool GetDeviceById(int id, out Device device)
        {
            device = _all.Find(d => d.UniqueId == id);
            return device != null && device.IsAvailable;
        }

        #endregion

    }
}