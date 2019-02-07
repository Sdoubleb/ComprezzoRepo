using System.Collections.Generic;
using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    public class ThriftyBlockyStreamWriter : IBlockyStreamWriter
    {
        private readonly IObjectPool<byte[]> _bytePool;

        public ThriftyBlockyStreamWriter(IStreamWriterProvider writerProvider,
            INumericStorageEnumerableProvider<NumberedByteBlock> storageEnumerableProvider,
            IObjectPool<byte[]> bytePool)
        {
            _bytePool = bytePool;
            StreamWriterProvider = writerProvider;
            StorageEnumerableProvider = storageEnumerableProvider;
        }

        public IStreamWriterProvider StreamWriterProvider { get; set; }

        public INumericStorageEnumerableProvider<NumberedByteBlock> StorageEnumerableProvider { get; set; }

        public void Write(Stream stream, int blockLength, ISizeableStorage<long, NumberedByteBlock> storage)
        {
            IEnumerable<NumberedByteBlock> blocks = StorageEnumerableProvider.ProvideNew(storage);
            IWriter writer = StreamWriterProvider.ProvideNew(stream, _bytePool, blocks);
            writer.Write();
        }

        public void Dispose() => _bytePool.Dispose();
    }
}