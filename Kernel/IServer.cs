
using System.Threading;

namespace iCSharp.Kernel
{
    using System;

    public interface IServer : IDisposable
    {
        void Start();

        void Stop();

        ManualResetEventSlim GetWaitEvent();
    }
}
