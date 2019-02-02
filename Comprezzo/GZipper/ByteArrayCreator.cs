namespace GZipper
{
    class ByteArrayCreator : ICreator<byte[]>
    {
        public ByteArrayCreator(int length)
        {
            Length = length;
        }

        public int Length { get; }

        public byte[] Create() => new byte[Length];
    }
}