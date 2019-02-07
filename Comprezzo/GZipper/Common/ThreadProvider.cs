using System;
using System.Threading;

namespace Sbb.Compression.Common
{
    class ThreadProvider : IThreadProvider
    {
        private static readonly int DefaultThreadCount = Environment.ProcessorCount;

        protected virtual int ThreadCount { get; } = DefaultThreadCount;

        public Thread[] Provide(ThreadStart start)
        {
            return Provide(() => new Thread(start));
        }

        public Thread[] Provide(ParameterizedThreadStart start)
        {
            return Provide(() => new Thread(start));
        }

        private Thread[] Provide(Func<Thread> creator)
        {
            var threads = new Thread[ThreadCount];
            for (int i = 0; i < threads.Length; i++)
                threads[i] = creator();
            return threads;
        }
    }
}