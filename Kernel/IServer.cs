
namespace iCSharp.Kernel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IServer : IDisposable
    {
        void Start();

        void Stop();
    }
}
