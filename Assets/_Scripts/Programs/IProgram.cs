using System;

namespace Hash17.Programs
{
    public interface IProgram
    {
        void Execute(string parameters);
        void Stop();
        event Action<IProgram> OnStart;
        event Action<IProgram> OnFinish;
        IProgram Clone();
    }
}