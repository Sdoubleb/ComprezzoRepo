using System;
using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers.Pumps
{
    class BlockyStream2StreamPump : IStream2StreamPump
    {
        public int BlockLength { get; set; }

        private IBlockyStreamReader _reader;
        private IBlockyStreamWriter _writer;

        public BlockyStream2StreamPump(IBlockyStream4erFactory stream4erFactory, int blockLength)
            : this(stream4erFactory.CreateStream4erPair(), blockLength) { }

        public BlockyStream2StreamPump(BlockyStream4erPair stream4erPair, int blockLength)
        {
            _reader = stream4erPair.StreamReader;
            _writer = stream4erPair.StreamWriter;
            BlockLength = blockLength;
        }

        public virtual void Pump(Stream source, Stream target)
        {
            ISizeableStorage<long, NumberedByteBlock> byteBlocks = _reader.Read(source, BlockLength);
            _writer.Write(target, BlockLength, byteBlocks);
        }

        public void Dispose()
        {
            _reader.Dispose();
            _writer.Dispose();
        }
    }
}