using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    public interface IOrderlyStreamReader
    {
        ISizeableStorage<long, NumberedByteBlock> Read(Stream stream, int blockLength);
    }
}