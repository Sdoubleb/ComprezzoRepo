using System;
using System.IO;

namespace Sbb.Compression.Stream4ers.Pumps
{
    public interface IStream2StreamPump : IDisposable
    {
        void Pump(Stream source, Stream target);
    }
}