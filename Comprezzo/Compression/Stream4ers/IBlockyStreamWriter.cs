using System.Collections.Generic;
using System.IO;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    public interface IBlockyStreamWriter
    {
        void Write();
    }

    public interface IBlockyStreamWriterProvider
    {
        IBlockyStreamWriter ProvideNew(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks);
    }
}