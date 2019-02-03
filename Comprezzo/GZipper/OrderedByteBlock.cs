using System.IO;

namespace GZipper
{
    class OrderedByteBlock
    {
        public OrderedByteBlock(long order, byte[] byteBlock, Stream stream)
        {
            Order = order;
            ByteBlock = byteBlock;
            Stream = stream;
        }

        public long Order { get; }
        public byte[] ByteBlock { get; }
        public Stream Stream { get; } // TODO убрать отсюда

        public int Length { get; set; }
    }
}