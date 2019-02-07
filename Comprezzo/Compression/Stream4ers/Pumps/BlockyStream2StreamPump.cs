using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers.Pumps
{
    class BlockyStream2StreamPump : IStream2StreamPump
    {
        public const int DEFAULT_STREAM_BLOCK_LENGTH = 1 * 1024 * 1024; // 1 МБ

        public BlockyStream2StreamPump(IOrderlyStream4erFactory stream4erFactory)
            : this(stream4erFactory.CreateStream4erPair()) { }

        public BlockyStream2StreamPump(OrderlyStream4erPair stream4erPair)
            : this(stream4erPair.StreamReader, stream4erPair.StreamWriter) { }

        public BlockyStream2StreamPump(IOrderlyStreamReader reader, IOrderlyStreamWriter writer)
        {
            FileReader = reader;
            FileWriter = writer;
        }

        private IOrderlyStreamReader FileReader { get; set; }

        private IOrderlyStreamWriter FileWriter { get; set; }

        public int StreamBlockLength { get; set; } = DEFAULT_STREAM_BLOCK_LENGTH;

        public void Pump(Stream source, Stream target)
        {
            ISizeableStorage<long, NumberedByteBlock> byteBlocks = FileReader.Read(source, StreamBlockLength);
            FileWriter.Write(target, StreamBlockLength, byteBlocks);
        }
    }
}