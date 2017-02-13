using System;
using System.Collections;
using System.Collections.Generic;
using Hash17.Utils;

namespace Hash17.Devices
{
    public class CarDevice : Device
    {
        #region Properties

        public override DeviceType DeviceType
        {
            get { return DeviceType.CarDevice; }
        }

        public List<RunningSystem> CarSystemsStatus { get; set; }

        #endregion
    }
}