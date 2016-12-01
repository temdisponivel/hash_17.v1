using System.Collections.Generic;
using Hash17.Devices.Networks;
using Hash17.Devices.Security;
using Hash17.Files;
using Hash17.Programs;

namespace Hash17.Devices
{
    public interface IDevice : IProtected<IDevice>
    {
        new string Name { get; set; }
        string Address { get; set; }
        INetwork Network { get; set; }

        IDictionary<string, IProgram> Programs { get; }
        IDictionary<ProgramId, IProgram> SpecialPrograms { get; }

        FileSystem FileSystem { get; }
    }
}