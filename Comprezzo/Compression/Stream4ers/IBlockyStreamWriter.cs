using System;
using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    public interface IBlockyStreamWriter : IDisposable
    {
        void Write(Stream stream, int blockLength, ISizeableStorage<long, NumberedByteBlock> storage);
    }
}