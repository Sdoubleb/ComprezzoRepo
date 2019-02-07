using Sbb.Compression.Common;

namespace Sbb.Compression._Drafts
{
    class _ByteArrayCreator : ICreator<byte[]>
    {
        public _ByteArrayCreator(int length)
        {
            Length = length;
        }

        public int Length { get; }

        public byte[] Create() => new byte[Length];
    }
}