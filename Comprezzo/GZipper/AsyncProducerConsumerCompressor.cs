using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipper
{
    public partial class AsyncProducerConsumerCompressor : ICompressor
    {
        private const int DEFAULT_BLOCK_LENGTH = 1 * 1024 * 1024; // 1 МБ
        private const int DEFAULT_BLOCK_POOL_SIZE = 128;

        private readonly string _inputFileName;
        private readonly string _outputFileName;

        private readonly int _blockLength;
        private readonly long _countOfBlocks;

        private readonly ObjectPool<byte[]> _byteBlockPool;
        private readonly AvoidingLockConcurrentStorage<byte[]> _byteBlocksToCompress;

        public AsyncProducerConsumerCompressor(string inputFileName, string outputFileName)
            : this(inputFileName, outputFileName, DEFAULT_BLOCK_LENGTH) { }

        public AsyncProducerConsumerCompressor(string inputFileName, string outputFileName, int blockLength)
        {
            _inputFileName = inputFileName;
            _outputFileName = outputFileName;

            var file = new FileInfo(_inputFileName);

            _blockLength = blockLength;
            _countOfBlocks = file.Length / _blockLength
                + (file.Length % _blockLength == 0 ? 0 : 1);

            _byteBlockPool = new ObjectPool<byte[]>(new ByteArrayCreator(_blockLength), null, DEFAULT_BLOCK_POOL_SIZE);
            _byteBlocksToCompress = new AvoidingLockConcurrentStorage<byte[]>(_countOfBlocks);
        }

        public void Compress()
        {
            using (FileStream source = new FileStream(_inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4 * _blockLength, true))
            using (FileStream target = new FileStream($"{_outputFileName}.gz", FileMode.Create, FileAccess.Write, FileShare.None, 4 * _blockLength))
            using (GZipStream compression = new GZipStream(target, CompressionMode.Compress))
            {
                Thread readThread = new Thread(() => ReadSource(source));
                Thread writeThread = new Thread(() => WriteIntoTarget(compression));

                readThread.Start();
                writeThread.Start();

                readThread.Join();
                writeThread.Join();
            }
        }

        private void ReadSource(FileStream source)
        {
            void read(byte[] bytes, long order)
            {
                source.BeginRead(bytes, 0, bytes.Length, new AsyncCallback(EndReadSourceBlock),
                    new ReadByteBlock(order, bytes, source));
            }

            for (long i = 0; i < _countOfBlocks - 1; i++)
                read(_byteBlockPool.Wait(), i);

            // дочитываем аппендикс
            var appendix = new byte[source.Length - source.Position];
            read(appendix, _countOfBlocks - 1);
        }

        private void EndReadSourceBlock(IAsyncResult readAsyncResult)
        {
            var readByteBlock = (ReadByteBlock)readAsyncResult.AsyncState;
            readByteBlock.Stream.EndRead(readAsyncResult);
            _byteBlocksToCompress.Add(readByteBlock.Order, readByteBlock.ByteBlock);
        }

        private void WriteIntoTarget(GZipStream compression)
        {
            for (long i = 0; i < _countOfBlocks; i++)
            {
#pragma warning disable IDE0018 // Объявление встроенной переменной
                byte[] bytesToWrite;
#pragma warning restore IDE0018 // Объявление встроенной переменной

                while (!_byteBlocksToCompress.TryGetAndRemove(i, out bytesToWrite))
                    continue;
                compression.Write(bytesToWrite, 0, bytesToWrite.Length);

                // аппендикс не возвращаем в пул, т.к. он был взят не оттуда
                if (i < _countOfBlocks - 1)
                    _byteBlockPool.Release(bytesToWrite);
            }
        }

        public void Decompress() => throw new NotImplementedException();
    }
}