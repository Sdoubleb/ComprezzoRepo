using System.IO;

namespace GZipper
{
    public interface IOrderlyStreamReader
    {
        IStorage<long, OrderedByteBlock> Read(Stream stream, int blockLength);
    }
}