using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers.Direct
{
    // низкоуровневая реализация писателя потока;
    // запись выполняется синхронно;
    // для записи используются несколько потоков;
    // байтовые массивы, представляющие блоки,
    // берутся из перечислителя хранилища
    // и после записи складываются в пул
    class MlthrdStreamWriter : StreamWriterBase
    {
        private readonly IThreadProvider _threadProvider;
        private readonly object _locker = new object();

        public MlthrdStreamWriter(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks,
            IThreadProvider threadProvider) : base(stream, bytePool, byteBlocks)
        {
            _threadProvider = threadProvider;
        }

        public override void Write()
        {
            IEnumerator<NumberedByteBlock> enumerator = _byteBlocks.GetEnumerator();
            var start = new ThreadStart(() => Write(enumerator));
            Thread[] threads = _threadProvider.Provide(start);
            Array.ForEach(threads, t => t.Start());
            Array.ForEach(threads, t => t.Join());
        }

        private void Write(IEnumerator<NumberedByteBlock> enumerator)
        {
            bool continueWriting = true;
            while (continueWriting)
            {
                NumberedByteBlock block = null;
                lock (_locker)
                {
                    if (continueWriting = enumerator.MoveNext())
                    {
                        block = enumerator.Current;

                        // на практике оказалось, что при синхронной записи
                        // достигается гораздо меньший расход памяти
                        _stream.Write(block.Bytes, 0, block.Length);
                    }
                }
                if (block != null)
                    _bytePool.Release(block.Bytes); // TODO: делать в отдельном потоке
            }
        }
    }

    class MlthrdStreamWriterProvider : StreamWriterProviderBase
    {
        public override IWriter ProvideNew(Stream stream,
            IObjectPool<byte[]> bytePool, IEnumerable<NumberedByteBlock> byteBlocks)
        {
            return new MlthrdStreamWriter(stream, bytePool, byteBlocks, ThreadProvider);
        }

        public IThreadProvider ThreadProvider { get; set; } = new ThreadProvider();
    }
}