using System;
using Hash17.Blackboard_;
using Hash17.Devices.Firewalls;
using System.Collections;

namespace Hash17.Devices
{
    public class PasswordedDevice : Device
    {
        public string Password;

        public override IEnumerator TryAccess(Action<bool, Device> callback)
        {
            yield return Blackboard.Instance.Firewalls[FirewallType.Password].Clone().Access(callback, this);
        }
    }
}