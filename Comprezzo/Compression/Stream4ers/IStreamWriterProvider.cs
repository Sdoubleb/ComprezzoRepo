using System.Collections.Generic;
using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    /// <summary>
    /// Интерфейс поставщика низкоуровневого писателя байтовых потоков.
    /// </summary>
    public interface IStreamWriterProvider
    {
        IWriter ProvideNew(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks);
    }
}