using System.IO;

namespace GZipper
{
    public class OrderedByteBlock
    {
        public OrderedByteBlock(long order, byte[] bytes, Stream stream)
        {
            Order = order;
            Bytes = bytes;
            Stream = stream;
        }

        public long Order { get; }
        public byte[] Bytes { get; }
        public Stream Stream { get; } // TODO: убрать отсюда

        public int Length { get; set; }
    }
}