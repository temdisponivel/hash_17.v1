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

        public readonly Dictionary<int, Device> DevicesById = new Dictionary<int, Device>();

        #endregion

        #region Current Device

        public void ChangeCurrentDevice(Device newCurrent)
        {
            CurrentDevice = newCurrent;
            if (OnCurrentDeviceChange != null)
                OnCurrentDeviceChange();
        }

        #endregion

        #region Overrides

        public override void Add(Device item)
        {
            DevicesById[item.Id] = item;
            base.Add(item);
        }

        #endregion
    }
}