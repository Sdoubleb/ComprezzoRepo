using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers.Direct
{
    abstract class StreamReaderBase : IReader
    {
        protected readonly Stream _stream;
        protected readonly int _blockLength;

        protected readonly IWaitableObjectPool<byte[]> _bytePool;
        protected readonly IStorage<long, NumberedByteBlock> _byteBlocks;

        protected StreamReaderBase(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks)
        {
            _stream = stream;
            _blockLength = blockLength;
            _bytePool = bytePool;
            _byteBlocks = byteBlocks;
        }

        public abstract void Read();
    }

    abstract class StreamReaderProviderBase : IStreamReaderProvider
    {
        public abstract IReader ProvideNew(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks);
    }
}
