using System;
using System.IO;
using System.Threading;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers.Direct
{
    // низкоуровневая реализация читателя потока;
    // чтение выполняется асинхронно;
    // для чтения используются несколько потоков;
    // байтовые массивы, представляющие блоки,
    // берутся из пула и после чтения складываются в хранилище
    // с номерами блоков в качестве ключей
    class AsyncMlthrdStreamReader : IReader
    {
        private readonly Stream _stream;

        private readonly int _blockLength;
        private long _currentBlockNumber; // номер текущего считываемого блока

        private readonly IWaitableObjectPool<byte[]> _bytePool;
        private readonly IStorage<long, NumberedByteBlock> _byteBlocks;
        
        private readonly IThreadProvider _threadProvider;        
        private readonly object _locker = new object();

        public AsyncMlthrdStreamReader(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks,
            IThreadProvider threadProvider)
        {
            _stream = stream;
            _blockLength = blockLength;
            _bytePool = bytePool;
            _byteBlocks = byteBlocks;
            _threadProvider = threadProvider;
        }

        public virtual void Read()
        {
            Thread[] threads = _threadProvider.Provide(new ThreadStart(BeginReadingBlock));
            Array.ForEach(threads, t => t.Start());
        }

        private void BeginReadingBlock()
        {
            byte[] bytes = _bytePool.Wait();
            lock (_locker)
            {
                // поскольку в отдельно взятый момент времени доступ к байтовому потоку
                // имеет только один поток времени выполнения,
                // нет необходимости синхронизировать сам объект байтового потока;
                // то же самое касается потокобезопасного инкремента счётчика
                _stream.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(EndReadingBlock),
                    new NumberedByteBlock(_currentBlockNumber++, bytes));
            }
        }

        private void EndReadingBlock(IAsyncResult asyncResult)
        {
            var block = (NumberedByteBlock)asyncResult.AsyncState;

            // после смещения позиции байтового потока до конца,
            // потоки времени выполнения будут ещё некоторое время пытаться прочитать его,
            // пока не осознают, что читать уже нечего и не прекратят свой жизненный путь
            if ((block.Length = _stream.EndRead(asyncResult)) > 0)
            {
                _byteBlocks.Add(block.Number, block);
                BeginReadingBlock();
            }
            else
            {
                _bytePool.Release(block.Bytes);
            }
        }
    }

    class AsyncMlthrdStreamReaderProvider : IStreamReaderProvider
    {
        public IReader ProvideNew(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks)
        {
            return new AsyncMlthrdStreamReader(stream, blockLength, bytePool, byteBlocks, ThreadProvider);
        }

        public IThreadProvider ThreadProvider { get; set; } = new ThreadProvider();
    }
}