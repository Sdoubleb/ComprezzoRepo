using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipper
{
    class ThriftyOrderlyStreamReader : IOrderlyStreamReader
    {
        public ThriftyOrderlyStreamReader(IWaitableObjectPoolProvider<byte[]> poolProvider,
            IStorageProvider<long, OrderedByteBlock> storageProvider)
        {
            PoolProvider = poolProvider;
            StorageProvider = storageProvider;
        }

        private IWaitableObjectPoolProvider<byte[]> PoolProvider { get; set; }

        private IStorageProvider<long, OrderedByteBlock> StorageProvider { get; set; }

        public IStorage<long, OrderedByteBlock> Read(Stream stream, int blockLength)
        {
            long countOfBlocks = CalculateCountOfBlocks(stream, blockLength);

            IWaitableObjectPool<byte[]> pool = PoolProvider.ProvideNew();
            IStorage<long, OrderedByteBlock> storage = StorageProvider.ProvideNew();

            return storage;
        }

        private long CalculateCountOfBlocks(Stream stream, int blockLength)
        {
            return stream.Length / blockLength
                + (stream.Length % blockLength == 0 ? 0 : 1);
        }

        
    }
}