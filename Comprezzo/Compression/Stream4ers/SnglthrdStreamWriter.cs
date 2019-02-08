using System.Collections.Generic;
using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    // низкоуровневая реализация писателя потока;
    // чтение выполняется синхронно;
    // для чтения используется один поток;
    // байтовые массивы, представляющие блоки,
    // берутся из перечислителя хранилища
    // и после чтения складываются в пул
    class SnglthrdStreamWriter : IWriter
    {
        private readonly Stream _stream;

        private readonly IObjectPool<byte[]> _bytePool;
        private readonly IEnumerable<NumberedByteBlock> _byteBlocks;

        public SnglthrdStreamWriter(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks)
        {
            _stream = stream;
            _bytePool = bytePool;
            _byteBlocks = byteBlocks;
        }

        public virtual void Write()
        {
            foreach (var block in _byteBlocks)
            {
                // на практике оказалось, что при синхронной записи
                // достигается гораздо меньший расход памяти
                _stream.Write(block.Bytes, 0, block.Length);

                _bytePool.Release(block.Bytes);
            }
        }
    }

    class SnglthrdStreamWriterProvider : IStreamWriterProvider
    {
        public IWriter ProvideNew(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks)
        {
            return new SnglthrdStreamWriter(stream, bytePool, byteBlocks);
        }
    }
}