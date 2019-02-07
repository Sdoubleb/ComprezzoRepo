using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    public interface IBlockyStreamReader
    {
        void Read();
    }

    public interface IBlockyStreamReaderProvider
    {
        IBlockyStreamReader ProvideNew(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks);
    }
}