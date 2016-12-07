using System;
using System.Collections;

namespace Hash17.Devices.Firewalls
{
    public interface IFirewall
    {
        IEnumerator Access(Action<bool, Device> callback, Device device);
        IFirewall Clone();
    }
}