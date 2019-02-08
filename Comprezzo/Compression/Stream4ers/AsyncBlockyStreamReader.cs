using System;
using System.IO;
using System.Threading;
using Sbb.Compression.Common;
using Sbb.Compression.Storages;

namespace Sbb.Compression.Stream4ers
{
    class AsyncBlockyStreamReader : IReader
    {
        private readonly Stream _stream;

        private readonly int _blockLength;
        private long _currentBlockNumber;

        private readonly IWaitableObjectPool<byte[]> _bytePool;
        private readonly IStorage<long, NumberedByteBlock> _byteBlocks;
        
        private IThreadProvider _threadProvider;
        
        private readonly object _locker = new object();

        public AsyncBlockyStreamReader(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks,
            IThreadProvider threadProvider)
        {
            _stream = stream;
            _blockLength = blockLength;
            _bytePool = bytePool;
            _byteBlocks = byteBlocks;
            _threadProvider = threadProvider;
        }

        public void Read()
        {
            Thread[] threads = _threadProvider.Provide(new ThreadStart(BeginReadingBlock));
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
            var block = (NumberedByteBlock)asyncResult.AsyncState;
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

    class AsyncBlockyStreamReaderProvider : IStreamReaderProvider
    {
        public IReader ProvideNew(Stream stream, int blockLength,
            IWaitableObjectPool<byte[]> bytePool, IStorage<long, NumberedByteBlock> byteBlocks)
        {
            return new AsyncBlockyStreamReader(stream, blockLength, bytePool, byteBlocks, ThreadProvider);
        }

        public IThreadProvider ThreadProvider { get; set; } = new ThreadProvider();
    }
}