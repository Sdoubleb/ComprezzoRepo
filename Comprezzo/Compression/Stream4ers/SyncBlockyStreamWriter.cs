using System.Collections.Generic;
using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    class SyncBlockyStreamWriter : IWriter
    {
        private readonly Stream _stream;

        private readonly IObjectPool<byte[]> _bytePool;
        private readonly IEnumerable<NumberedByteBlock> _byteBlocks;

        public SyncBlockyStreamWriter(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks)
        {
            _stream = stream;
            _bytePool = bytePool;
            _byteBlocks = byteBlocks;
        }

        public void Write()
        {
            foreach (var block in _byteBlocks)
            {
                // на практике оказалось, что при синхронной записи
                // достигается гораздо меньший расход памяти
                _stream.Write(block.Bytes, 0, block.Length);

                _bytePool.Release(block.Bytes);
            }
        }
    }

    class BlockyStreamWriterProvider : IStreamWriterProvider
    {
        public IWriter ProvideNew(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks)
        {
            return new SyncBlockyStreamWriter(stream, bytePool, byteBlocks);
        }
    }
}