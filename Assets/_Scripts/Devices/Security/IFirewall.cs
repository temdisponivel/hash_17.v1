using System;

namespace Hash17.Devices.Security
{
    public interface IFirewall
    {
        void Crack(Action<bool> resultCallback);
    }
}
