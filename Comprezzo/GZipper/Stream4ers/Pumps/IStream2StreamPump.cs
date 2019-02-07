using System.IO;

namespace Sbb.Compression.Stream4ers.Pumps
{
    public interface IStream2StreamPump
    {
        void Pump(Stream source, Stream target);
    }
}