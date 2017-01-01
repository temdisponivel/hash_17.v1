using System;
using Hash17.Blackboard_;
using Hash17.Devices.Firewalls;
using System.Collections;
using UnityEditor;

namespace Hash17.Devices
{
    public class PasswordedDevice : Device
    {
        public string Password;
        public override DeviceType DeviceType { get { return DeviceType.Passworded; } }
    }
}