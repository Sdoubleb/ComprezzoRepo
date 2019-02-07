using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers.Pumps
{
    class BlockyStream2StreamPump : IStream2StreamPump
    {
        public const int DEFAULT_BLOCK_LENGTH = 1 * 1024 * 1024; // 1 МБ

        public int BlockLength { get; set; } = DEFAULT_BLOCK_LENGTH;

        private IBlockyStreamReader _reader;
        private IBlockyStreamWriter _writer;

        public BlockyStream2StreamPump(IBlockyStream4erFactory stream4erFactory)
            : this(stream4erFactory.CreateStream4erPair()) { }

        public BlockyStream2StreamPump(BlockyStream4erPair stream4erPair)
        {
            _reader = stream4erPair.StreamReader;
            _writer = stream4erPair.StreamWriter;
        }

        public void Pump(Stream source, Stream target)
        {
            ISizeableStorage<long, NumberedByteBlock> byteBlocks = _reader.Read(source, BlockLength);
            _writer.Write(target, BlockLength, byteBlocks);
        }
    }
}