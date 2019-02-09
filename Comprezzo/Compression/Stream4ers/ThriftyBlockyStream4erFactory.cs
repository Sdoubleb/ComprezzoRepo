using Sbb.Compression.Storages;
using Sbb.Compression.Stream4ers.Direct;

namespace Sbb.Compression.Stream4ers
{
    /// <summary>
    /// Фабрика по созданию пары из бережлиых поблочных чтеца и писца байтовых потоков.
    /// Использует поставщиков низкоуровневых читателя и писателя байтовых потоков,
    /// поставщиков нумерованного хранилища считанных блоков байтов и его перечислителя
    /// и поставщика пула байтовых массивов.
    /// </summary>
    public class ThriftyBlockyStream4erFactory : IBlockyStream4erFactory
    {
        public ThriftyBlockyStream4erFactory(IStreamReaderProvider readerProvider,
            IStreamWriterProvider writerProvider,
            ISizeableStorageProvider<long, NumberedByteBlock> storageProvider,
            INumericStorageEnumerableProvider<NumberedByteBlock> storageEnumerableProvider,
            IWaitableObjectPoolProvider<byte[]> poolProvider)
        {
            ReaderProvider = readerProvider;
            WriterProvider = writerProvider;
            StorageProvider = storageProvider;
            StorageEnumerableProvider = storageEnumerableProvider;
            PoolProvider = poolProvider;
        }

        public IStreamReaderProvider ReaderProvider { get; set; }

        public IStreamWriterProvider WriterProvider { get; set; }

        public ISizeableStorageProvider<long, NumberedByteBlock> StorageProvider { get; set; }

        public INumericStorageEnumerableProvider<NumberedByteBlock> StorageEnumerableProvider { get; set; }

        public IWaitableObjectPoolProvider<byte[]> PoolProvider { get; set; }
        
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