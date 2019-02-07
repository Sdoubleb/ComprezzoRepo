using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    class ThriftyBlockyStream4erFactory : IBlockyStream4erFactory
    {
        public ThriftyBlockyStream4erFactory(IStreamReaderProvider readerProvider,
            IStreamWriterProvider writerProvider,
            ISizeableStorageProvider<long, NumberedByteBlock> storageProvider,
            INumericStorageEnumerableProvider<NumberedByteBlock> storageEnumerableProvider,
            IWaitableObjectPoolProvider<byte[]> poolProvider)
        {
            ReaderProvider = readerProvider;
            WriterProvider = writerProvider;
            PoolProvider = poolProvider;
            StorageProvider = storageProvider;
            StorageEnumerableProvider = storageEnumerableProvider;
        }

        public IStreamReaderProvider ReaderProvider { get; set; }

        public IStreamWriterProvider WriterProvider { get; set; }

        public IWaitableObjectPoolProvider<byte[]> PoolProvider { get; set; }

        public ISizeableStorageProvider<long, NumberedByteBlock> StorageProvider { get; set; }

        public INumericStorageEnumerableProvider<NumberedByteBlock> StorageEnumerableProvider { get; set; }

        public BlockyStream4erPair CreateStream4erPair()
        {
            IWaitableObjectPool<byte[]> bytePool = PoolProvider.ProvideNew();
            var reader = new ThriftyBlockyStreamReader(ReaderProvider, StorageProvider, bytePool);
            var writer = new ThriftyBlockyStreamWriter(WriterProvider, StorageEnumerableProvider, bytePool);
            return new BlockyStream4erPair
            {
                StreamReader = reader,
                StreamWriter = writer
            };
        }
    }
}