using System;

namespace Hash17.Programs
{
    public interface IProgram
    {
        void Execute(string parameters);
        string GetDescription();
        string GetUsage();
        void Stop();
        event Action<IProgram> OnStart;
        event Action<IProgram> OnFinish;
        IProgram Clone();
    }
}