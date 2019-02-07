using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Sbb.Compression.Storages;

namespace Sbb.Compression._Drafts
{
    public class _AsyncCompressor : _ICompressor
    {
        private const int DEFAULT_BLOCK_LENGTH = 1 * 1024 * 1024; // 1 МБ
        private const int DEFAULT_BLOCK_POOL_SIZE = 128;

        private readonly string _inputFileName;
        private readonly string _outputFileName;

        private readonly int _blockLength;
        private readonly long _countOfBlocks;

        private readonly ObjectPool<byte[]> _byteBlockPool;
        private readonly AvoidingLockConcurrentStorage<byte[]> _byteBlocksToCompress;

        public _AsyncCompressor(string inputFileName, string outputFileName)
            : this(inputFileName, outputFileName, DEFAULT_BLOCK_LENGTH) { }

        public _AsyncCompressor(string inputFileName, string outputFileName, int blockLength)
        {
            _inputFileName = inputFileName;
            _outputFileName = outputFileName;

            var file = new FileInfo(_inputFileName);

            _blockLength = blockLength;
            _countOfBlocks = file.Length / _blockLength
                + (file.Length % _blockLength == 0 ? 0 : 1);

            _byteBlockPool = new ObjectPool<byte[]>(new _ByteArrayCreator(_blockLength), null, DEFAULT_BLOCK_POOL_SIZE);
            _byteBlocksToCompress = new AvoidingLockConcurrentStorage<byte[]>(_countOfBlocks);
        }

        public void Compress()
        {
            using (FileStream source = new FileStream(_inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read, _blockLength, FileOptions.SequentialScan | FileOptions.Asynchronous))
            using (FileStream target = new FileStream($"{_outputFileName}.gz", FileMode.Create, FileAccess.Write, FileShare.None, _blockLength, FileOptions.SequentialScan))
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
                    new _OrderedByteBlock(order, bytes, source));
            }

            for (long i = 0; i < _countOfBlocks - 1; i++)
                read(_byteBlockPool.Wait(), i);

            // дочитываем аппендикс
            var appendix = new byte[source.Length - source.Position];
            read(appendix, _countOfBlocks - 1);
        }

        private void EndReadSourceBlock(IAsyncResult readAsyncResult)
        {
            var readByteBlock = (_OrderedByteBlock)readAsyncResult.AsyncState;
            readByteBlock.Stream.EndRead(readAsyncResult);
            _byteBlocksToCompress.Add(readByteBlock.Order, readByteBlock.Bytes);
        }

        private void WriteIntoTarget(GZipStream compression)
        {
            for (long i = 0; i < _countOfBlocks; i++)
            {
                byte[] bytesToWrite;
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