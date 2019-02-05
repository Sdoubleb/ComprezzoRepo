using System;
using System.IO;
using System.Threading;

namespace GZipper
{
    class AsyncStreamReader
    {
        private readonly Stream _stream;

        private readonly int _blockLength;
        private long _currentBlockNumber;

        private readonly IWaitableObjectPool<byte[]> _bytePool;
        private readonly IStorage<long, OrderedByteBlock> _byteBlocks;

        private readonly object _locker = new object();

        public AsyncStreamReader(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, OrderedByteBlock> byteBlocks)
        {
            _stream = stream;
            _blockLength = blockLength;
            CountOfBlocks = Utils.CalculateCountOfStreamBlocks(stream, blockLength);

            _bytePool = bytePool;
            _byteBlocks = byteBlocks;
        }

        public long CountOfBlocks { get; }

        public long CurrentBlockNumber => Interlocked.Read(ref _currentBlockNumber); // TEST

        public void StartReading()
        {
            int threadCount = Environment.ProcessorCount;
            var threads = new Thread[threadCount];
            for (int i = 0; i < threads.Length; i++)
                threads[i] = new Thread(() => BeginReadingBlock());
            Array.ForEach(threads, t => t.Start());
        }

        private void BeginReadingBlock()
        {
            byte[] bytes = _bytePool.Wait();
            lock (_locker) // TEST: точно нужен ли?
            {
                // TEST: пре- или постинкремент?
                _stream.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(EndReadingBlock),
                    new OrderedByteBlock(Interlocked.Increment(ref _currentBlockNumber), bytes, _stream));
            }
        }

        private void EndReadingBlock(IAsyncResult asyncResult)
        {
            var block = (OrderedByteBlock)asyncResult.AsyncState;
            if ((block.Length = _stream.EndRead(asyncResult)) > 0)
            {
                _byteBlocks.Add(block.Order, block);
                BeginReadingBlock();
            }
            else
            {
                _bytePool.Release(block.Bytes);
            }
        }
    }
}