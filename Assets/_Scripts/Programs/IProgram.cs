using System;
using System.Collections.Generic;

namespace Hash17.Programs
{
    public interface IProgram
    {
        void Execute(string parameters);
        string Description { get; }
        string Usage { get; }
        bool DeviceIndependent { get; }
        void Stop();
        event Action<IProgram> OnStart;
        event Action<IProgram> OnFinish;
        IProgram Clone();
    }
}