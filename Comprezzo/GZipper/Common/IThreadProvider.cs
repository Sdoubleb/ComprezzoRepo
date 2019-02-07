using System.Threading;

namespace Sbb.Compression.Common
{
    public interface IThreadProvider
    {
        Thread[] Provide(ThreadStart start);

        Thread[] Provide(ParameterizedThreadStart start);
    }
}