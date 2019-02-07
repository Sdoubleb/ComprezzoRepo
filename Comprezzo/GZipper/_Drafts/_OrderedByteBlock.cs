using System.IO;

namespace Sbb.Compression
{
    public class _OrderedByteBlock
    {
        public _OrderedByteBlock(long order, byte[] bytes, Stream stream)
        {
            Order = order;
            Bytes = bytes;
            Stream = stream;
        }

        public long Order { get; }
        public byte[] Bytes { get; }
        public Stream Stream { get; }

        public int Length { get; set; }
    }
}