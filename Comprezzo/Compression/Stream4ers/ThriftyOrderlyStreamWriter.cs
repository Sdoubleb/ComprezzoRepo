using System.Collections.Generic;
using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    class ThriftyOrderlyStreamWriter : IOrderlyStreamWriter
    {
        private readonly IObjectPool<byte[]> _bytePool;

        public ThriftyOrderlyStreamWriter(IBlockyStreamWriterProvider writerProvider,
            INumericStorageEnumerableProvider<NumberedByteBlock> storageEnumerableProvider,
            IWaitableObjectPool<byte[]> bytePool)
        {
            _bytePool = bytePool;
            StreamWriterProvider = writerProvider;
            StorageEnumerableProvider = storageEnumerableProvider;
        }

        public IBlockyStreamWriterProvider StreamWriterProvider { get; set; }

        public INumericStorageEnumerableProvider<NumberedByteBlock> StorageEnumerableProvider { get; set; }

        public void Write(Stream stream, int blockLength, IStorage<long, NumberedByteBlock> storage)
        {
            long totalCountOfBlocks = Utils.CalculateCountOfBlocks(stream.Length, blockLength);
            IEnumerable<NumberedByteBlock> blocks = StorageEnumerableProvider
                .ProvideNew(storage, totalCountOfBlocks);
            IBlockyStreamWriter writer = StreamWriterProvider.ProvideNew(stream, _bytePool, blocks);
            writer.Write();
        }
    }
}