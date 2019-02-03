using System.IO;

namespace GZipper
{
    class ReadByteBlock
    {
        public ReadByteBlock(long order, byte[] byteBlock, FileStream stream)
        {
            Order = order;
            ByteBlock = byteBlock;
            Stream = stream;
        }

        public long Order { get; }
        public byte[] ByteBlock { get; }
        public FileStream Stream { get; } // TODO убрать отсюда

        public int Length { get; set; }
    }
}