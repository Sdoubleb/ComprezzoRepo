using System.Collections.Generic;
using System.IO;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    // низкоуровневая реализация писателя потока;
    // запись выполняется синхронно;
    // для записи используется один поток;
    // байтовые массивы, представляющие блоки,
    // берутся из перечислителя хранилища
    // и после записи складываются в пул
    class SnglthrdStreamWriter : StreamWriterBase
    {
        public SnglthrdStreamWriter(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks)
            : base(stream, bytePool, byteBlocks) { }

        public override void Write()
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

    class SnglthrdStreamWriterProvider : StreamWriterProviderBase
    {
        public override IWriter ProvideNew(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks)
        {
            return new SnglthrdStreamWriter(stream, bytePool, byteBlocks);
        }
    }
}