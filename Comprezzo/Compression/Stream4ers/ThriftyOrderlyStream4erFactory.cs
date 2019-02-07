using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    class ThriftyOrderlyStream4erFactory : IOrderlyStream4erFactory
    {
        public ThriftyOrderlyStream4erFactory(IBlockyStreamReaderProvider readerProvider,
            IBlockyStreamWriterProvider writerProvider, IWaitableObjectPoolProvider<byte[]> poolProvider,
            ISizeableStorageProvider<long, NumberedByteBlock> storageProvider,
            INumericStorageEnumerableProvider<NumberedByteBlock> storageEnumerableProvider)
        {
            ReaderProvider = readerProvider;
            WriterProvider = writerProvider;
            PoolProvider = poolProvider;
            StorageProvider = storageProvider;
            StorageEnumerableProvider = storageEnumerableProvider;
        }

        public IBlockyStreamReaderProvider ReaderProvider { get; set; }

        public IBlockyStreamWriterProvider WriterProvider { get; set; }

        public IWaitableObjectPoolProvider<byte[]> PoolProvider { get; set; }

        public ISizeableStorageProvider<long, NumberedByteBlock> StorageProvider { get; set; }

        public INumericStorageEnumerableProvider<NumberedByteBlock> StorageEnumerableProvider { get; set; }

        public OrderlyStream4erPair CreateStream4erPair()
        {
            IWaitableObjectPool<byte[]> bytePool = PoolProvider.ProvideNew();
            var reader = new ThriftyOrderlyStreamReader(ReaderProvider, StorageProvider, bytePool);
            var writer = new ThriftyOrderlyStreamWriter(WriterProvider, StorageEnumerableProvider, bytePool);
            return new OrderlyStream4erPair
            {
                StreamReader = reader,
                StreamWriter = writer
            };
        }
    }
}