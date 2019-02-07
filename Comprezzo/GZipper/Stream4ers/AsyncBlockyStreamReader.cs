using System;
using System.IO;
using System.Threading;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    class AsyncBlockyStreamReader : IBlockyStreamReader
    {
        private readonly Stream _stream;

        private readonly int _blockLength;
        private long _currentBlockNumber;

        private readonly IWaitableObjectPool<byte[]> _bytePool;
        private readonly IStorage<long, NumberedByteBlock> _byteBlocks;

        private readonly object _locker = new object();

        public AsyncBlockyStreamReader(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks,
            IThreadProvider threadProvider)
        {
            _stream = stream;
            _blockLength = blockLength;
            _bytePool = bytePool;
            _byteBlocks = byteBlocks;
            ThreadProvider = threadProvider;
        }

        public IThreadProvider ThreadProvider { get; set; }

        public void Read()
        {
            Thread[] threads = ThreadProvider.Provide(new ThreadStart(BeginReadingBlock));
            Array.ForEach(threads, t => t.Start());
        }

        private void BeginReadingBlock()
        {
            byte[] bytes = _bytePool.Wait();
            lock (_locker)
            {
                _stream.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(EndReadingBlock),
                    new NumberedByteBlock(_currentBlockNumber++, bytes));
            }
        }

        private void EndReadingBlock(IAsyncResult asyncResult)
        {
            bool continueReading = false;
            var block = (NumberedByteBlock)asyncResult.AsyncState;
            if ((block.Length = _stream.EndRead(asyncResult)) > 0)
            {
                _byteBlocks.Add(block.Number, block);
                continueReading = true;
            }
            _bytePool.Release(block.Bytes);
            if(continueReading)
                BeginReadingBlock();
        }
    }

    class AsyncBlockyStreamReaderProvider : IBlockyStreamReaderProvider
    {
        public IBlockyStreamReader ProvideNew(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks)
        {
            return new AsyncBlockyStreamReader(stream, blockLength, bytePool, byteBlocks, ThreadProvider);
        }

        public IThreadProvider ThreadProvider { get; set; } = new ThreadProvider();
    }
}