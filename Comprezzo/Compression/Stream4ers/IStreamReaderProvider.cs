using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    /// <summary>
    /// Интерфейс поставщика низкоуровневого читателя байтовых потоков.
    /// </summary>
    public interface IStreamReaderProvider
    {
        IReader ProvideNew(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks);
    }
}