using System;

namespace Hash17.Devices.Security
{
    public interface IProtected
    {
        string Name { get; } 
    }

    public interface IProtected<T> : IProtected
        where T : IProtected<T>
    {
        void Access(string firewallPassCode, Action<bool, string, T> callback);
    }
}