using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Sbb.Compression._Drafts
{
    public class _ProducerConsumerCompressor : _ICompressor
    {
        private const int DEFAULT_BLOCK_LENGTH = 1 * 1024 * 1024; // 1 МБ

        private string _inputFileName;
        private string _outputFileName;
        private int _blockLength;

        private readonly _ProducerConsumerQueue<byte[]> _queueToCompress = new _ProducerConsumerQueue<byte[]>();

        public _ProducerConsumerCompressor(string inputFileName, string outputFileName)
            : this(inputFileName, outputFileName, DEFAULT_BLOCK_LENGTH) { }

        public _ProducerConsumerCompressor(string inputFileName, string outputFileName, int blockLength)
        {
            _inputFileName = inputFileName;
            _outputFileName = outputFileName;
            _blockLength = blockLength;
        }

        public void Compress()
        {
            using (FileStream source = new FileStream(_inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read, _blockLength, FileOptions.SequentialScan))
            using (FileStream target = new FileStream($"{_outputFileName}.gz", FileMode.Create, FileAccess.Write, FileShare.None, _blockLength, FileOptions.SequentialScan))
            using (GZipStream compression = new GZipStream(target, CompressionMode.Compress))
            {
                long countOfBlocks = source.Length / _blockLength
                    + (source.Length % _blockLength == 0 ? 0 : 1);

                Thread readThread = new Thread(() => ReadSource(source, countOfBlocks));
                Thread writeThread = new Thread(() => WriteIntoTarget(compression, countOfBlocks));

                readThread.Start();
                writeThread.Start();

                readThread.Join();
                writeThread.Join();
            }
        }

        private void ReadSource(FileStream source, long countOfBlocks)
        {
            void read(int length)
            {
                byte[] readBytes = new byte[length];
                source.Read(readBytes, 0, length);
                _queueToCompress.Enqueue(readBytes);
            }

            for (long i = 0; i < countOfBlocks - 1; i++)
                read(_blockLength);

            // дочитываем аппендикс
            if (source.Position < source.Length)
                read((int)(source.Length - source.Position));
        }

        private void WriteIntoTarget(GZipStream compression, long countOfBlocks)
        {
            for (long i = 0; i < countOfBlocks; i++)
            {
                byte[] bytesToWrite;
                while (!_queueToCompress.TryDequeue(out bytesToWrite))
                    continue;
                compression.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
        }
        
        public void Decompress() => throw new NotImplementedException();
    }
}