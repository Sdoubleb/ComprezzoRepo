using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;
using Sbb.Compression.Stream4ers.Direct;

namespace Sbb.Compression.Stream4ers
{
    // содержит высокоуровневую реализацию;
    // принимает байтовый пул и возвращает хранилище считанных байтовых блоков
    /// <summary>
    /// Бережливый поблочный читатель байтовых потоков.
    /// </summary>
    public class ThriftyBlockyStreamReader : IBlockyStreamReader
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

        public IStreamReaderProvider StreamReaderProvider { get; set; }

        public ISizeableStorageProvider<long, NumberedByteBlock> StorageProvider { get; set; }

        /// <param name="blockLength">Длина единоразово считываемого байтового массива.</param>
        public ISizeableStorage<long, NumberedByteBlock> Read(Stream stream, int blockLength)
        {
            long totalCountOfBlocks = Utils.CalculateCountOfBlocks(stream.Length, blockLength);
            ISizeableStorage<long, NumberedByteBlock> storage = StorageProvider.ProvideNew(totalCountOfBlocks);
            IReader reader = StreamReaderProvider.ProvideNew(stream, blockLength, _bytePool, storage);
            reader.Read();
            return storage;
        }

        public void Dispose() => _bytePool.Dispose();
    }
}