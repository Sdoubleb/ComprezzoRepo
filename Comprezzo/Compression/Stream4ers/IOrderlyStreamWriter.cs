using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    public interface IOrderlyStreamWriter
    {
        void Write(Stream stream, int blockLength, ISizeableStorage<long, NumberedByteBlock> storage);
    }
}