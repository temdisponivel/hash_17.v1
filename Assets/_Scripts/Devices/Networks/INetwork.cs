using System.Collections.Generic;
using Hash17.Devices.Security;

namespace Hash17.Devices.Networks
{
    public interface INetwork : IProtected<INetwork>
    {
        string Name { get; }
        string Identifier { get; }
        List<IProtected> Protecteds { get; }
    }
}