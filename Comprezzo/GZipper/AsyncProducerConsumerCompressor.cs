using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipper
{
    public partial class AsyncProducerConsumerCompressor : ICompressor
    {
        private const int DEFAULT_BLOCK_LENGTH = 1 * 1024 * 1024; // 1 МБ

        private readonly string _inputFileName;
        private readonly string _outputFileName;

        private readonly int _blockLength;
        private readonly long _countOfBlocks;

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

            _byteBlocksToCompress = new AvoidingLockConcurrentStorage<byte[]>(_countOfBlocks);
        }

        public void Compress()
        {
            using (FileStream source = new FileStream(_inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4 * _blockLength, true))
            using (FileStream target = new FileStream($"{_outputFileName}.gz", FileMode.Create, FileAccess.Write, FileShare.None, 4 * _blockLength))
            using (GZipStream compression = new GZipStream(target, CompressionMode.Compress))
            {
                Thread readThread = new Thread(() => ReadSource(source, _countOfBlocks));
                Thread writeThread = new Thread(() => WriteIntoTarget(compression, _countOfBlocks));

                readThread.Start();
                writeThread.Start();

                readThread.Join();
                writeThread.Join();
            }
        }

        private void ReadSource(FileStream source, long countOfBlocks)
        {
            void read(int length, long order)
            {
                byte[] readBytes = new byte[length];
                source.BeginRead(readBytes, 0, length, new AsyncCallback(EndReadSourceBlock),
                    new ReadByteBlock(order, readBytes, source));
            }

            for (long i = 0; i < countOfBlocks - 1; i++)
                read(_blockLength, i);

            // дочитываем аппендикс
            if (source.Position < source.Length)
                read((int)(source.Length - source.Position), countOfBlocks - 1);
        }

        private void EndReadSourceBlock(IAsyncResult readAsyncResult)
        {
            var readByteBlock = (ReadByteBlock)readAsyncResult.AsyncState;
            readByteBlock.Stream.EndRead(readAsyncResult);
            _byteBlocksToCompress.Add(readByteBlock.Order, readByteBlock.ByteBlock);
        }

        private void WriteIntoTarget(GZipStream compression, long countOfBlocks)
        {
            for (long i = 0; i < countOfBlocks; i++)
            {
#pragma warning disable IDE0018 // Объявление встроенной переменной
                byte[] bytesToWrite;
#pragma warning restore IDE0018 // Объявление встроенной переменной

                while (!_byteBlocksToCompress.TryGetAndRemove(i, out bytesToWrite))
                    continue;
                compression.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
        }

        public void Decompress() => throw new NotImplementedException();
    }
}