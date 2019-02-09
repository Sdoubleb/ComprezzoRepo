using System.Collections.Generic;
using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers.Direct
{
    abstract class StreamWriterBase : IWriter
    {
        protected readonly Stream _stream;

        protected readonly IObjectPool<byte[]> _bytePool;
        protected readonly IEnumerable<NumberedByteBlock> _byteBlocks;

        protected StreamWriterBase(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks)
        {
            _stream = stream;
            _bytePool = bytePool;
            _byteBlocks = byteBlocks;
        }

        public abstract void Write();
    }

    abstract class StreamWriterProviderBase : IStreamWriterProvider
    {
        public abstract IWriter ProvideNew(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks);
    }
}