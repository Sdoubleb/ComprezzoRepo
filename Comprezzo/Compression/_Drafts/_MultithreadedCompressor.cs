using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Sbb.Compression.Storages;

namespace Sbb.Compression._Drafts
{
    public class _MultithreadedCompressor : _ICompressor
    {
        private const int DEFAULT_BLOCK_LENGTH = 1 * 1024 * 1024; // 1 МБ
        private const int DEFAULT_BLOCK_POOL_SIZE = 128;

        private readonly string _inputFileName;
        private readonly string _outputFileName;

        private readonly int _blockLength;
        private readonly long _countOfBlocks;
        private long _currentBlockNumber;

        private readonly ObjectPool<byte[]> _byteBlockPool;
        private readonly AvoidingLockConcurrentStorage<_OrderedByteBlock> _byteBlocksToCompress;

        public _MultithreadedCompressor(string inputFileName, string outputFileName)
            : this(inputFileName, outputFileName, DEFAULT_BLOCK_LENGTH) { }

        public _MultithreadedCompressor(string inputFileName, string outputFileName, int blockLength)
        {
            _inputFileName = inputFileName;
            _outputFileName = outputFileName;

            var file = new FileInfo(_inputFileName);

            _blockLength = blockLength;
            _countOfBlocks = file.Length / _blockLength
                + (file.Length % _blockLength == 0 ? 0 : 1);

            _byteBlockPool = new ObjectPool<byte[]>(new _ByteArrayCreator(_blockLength), null, DEFAULT_BLOCK_POOL_SIZE);
            _byteBlocksToCompress = new AvoidingLockConcurrentStorage<_OrderedByteBlock>(_countOfBlocks);
        }

        public void Compress()
        {
            using (FileStream source = new FileStream(_inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read, _blockLength, FileOptions.SequentialScan | FileOptions.Asynchronous))
            using (FileStream target = new FileStream($"{_outputFileName}.gz", FileMode.Create, FileAccess.Write, FileShare.None, _blockLength, FileOptions.SequentialScan))
            using (GZipStream compression = new GZipStream(target, CompressionMode.Compress))
            {
                var readingThreads = new Thread[Environment.ProcessorCount];
                for (int i = 0; i < readingThreads.Length; i++)
                    readingThreads[i] = new Thread(() => ReadSource(source));
                Thread writingThread = new Thread(() => WriteIntoTarget(compression));

                Array.ForEach(readingThreads, t => t.Start());
                writingThread.Start();

                Array.ForEach(readingThreads, t => t.Join());
                writingThread.Join();
            }
        }

        private void ReadSource(FileStream source)
        {
            while (ContinueReading())
                BeginReadSourceBlock(source);
        }

        private bool ContinueReading()
        {
            return Interlocked.CompareExchange(ref _currentBlockNumber, _countOfBlocks, _countOfBlocks) < _countOfBlocks;
        }

        private void BeginReadSourceBlock(FileStream source)
        {
            byte[] bytes = _byteBlockPool.Wait();
            lock (source)
            {
                source.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(EndReadSourceBlock),
                    new _OrderedByteBlock(_currentBlockNumber, bytes, source));
                Interlocked.Increment(ref _currentBlockNumber);
            }
        }

        private void EndReadSourceBlock(IAsyncResult readAsyncResult)
        {
            var readByteBlock = (_OrderedByteBlock)readAsyncResult.AsyncState;
            if ((readByteBlock.Length = readByteBlock.Stream.EndRead(readAsyncResult)) > 0)
                _byteBlocksToCompress.Add(readByteBlock.Order, readByteBlock);
            else
                _byteBlockPool.Release(readByteBlock.Bytes);
        }

        private void WriteIntoTarget(GZipStream compression)
        {
            for (long i = 0; i < _countOfBlocks; i++)
            {
                _OrderedByteBlock byteBlockToWrite;
                while (!_byteBlocksToCompress.TryGetAndRemove(i, out byteBlockToWrite))
                    continue;
                compression.Write(byteBlockToWrite.Bytes, 0, byteBlockToWrite.Length);
                _byteBlockPool.Release(byteBlockToWrite.Bytes);
            }
        }

        public void Decompress() => throw new NotImplementedException();
    }
}