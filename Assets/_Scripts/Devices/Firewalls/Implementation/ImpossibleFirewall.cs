using System;
using System.Collections;

namespace Hash17.Devices.Firewalls.Implementation
{
    public class ImpossibleFirewall : IFirewall
    {
        public IEnumerator Access(Action<bool, Device> callback, Device device)
        {
            yield return null;

            if (callback != null)
                callback(false, device);
        }

        public IFirewall Clone()
        {
            return MemberwiseClone() as ImpossibleFirewall;
        }
    }
}