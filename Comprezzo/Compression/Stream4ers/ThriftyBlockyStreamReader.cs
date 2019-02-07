using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    class ThriftyBlockyStreamReader : IBlockyStreamReader
    {
        private readonly IWaitableObjectPool<byte[]> _bytePool;

        public ThriftyBlockyStreamReader(IStreamReaderProvider readerProvider,
            ISizeableStorageProvider<long, NumberedByteBlock> storageProvider,
            IWaitableObjectPool<byte[]> bytePool)
        {
            _bytePool = bytePool;
            StreamReaderProvider = readerProvider;
            StorageProvider = storageProvider;
        }

        private IStreamReaderProvider StreamReaderProvider { get; set; }

        private ISizeableStorageProvider<long, NumberedByteBlock> StorageProvider { get; set; }

        public ISizeableStorage<long, NumberedByteBlock> Read(Stream stream, int blockLength)
        {
            long totalCountOfBlocks = Utils.CalculateCountOfBlocks(stream.Length, blockLength);
            ISizeableStorage<long, NumberedByteBlock> storage = StorageProvider.ProvideNew(totalCountOfBlocks);
            IReader reader = StreamReaderProvider.ProvideNew(stream, blockLength, _bytePool, storage);
            reader.Read();
            return storage;
        }
    }
}