using System.Collections.Generic;
using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    class BlockyStreamWriter : IBlockyStreamWriter
    {
        private readonly Stream _stream;

        private readonly IObjectPool<byte[]> _bytePool;
        private readonly IEnumerable<NumberedByteBlock> _byteBlocks;

        public BlockyStreamWriter(Stream stream,
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
                _stream.Write(block.Bytes, 0, block.Length);
                _bytePool.Release(block.Bytes);
            }
        }
    }

    class BlockyStreamWriterProvider : IBlockyStreamWriterProvider
    {
        public IBlockyStreamWriter ProvideNew(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks)
        {
            return new BlockyStreamWriter(stream, bytePool, byteBlocks);
        }
    }
}