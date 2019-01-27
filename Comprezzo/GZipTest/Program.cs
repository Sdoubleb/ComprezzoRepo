using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string operation = args[0];
            string inputFileName = args[1];
            string outputFileName = args[2];

            Action<string, string> action;

            switch (operation.ToLower())
            {
                case "compress":
                    action = Compress;
                    break;
                case "decompress":
                    action = Decompress;
                    break;
                default:
                    throw new ArgumentException($"Неподдерживаемая операция: {operation}.");
            }

            action(inputFileName, outputFileName);

            Console.ReadLine();
        }

        static void Compress(string inputFileName, string outputFileName, int blockLength)
        {            
            using (FileStream source = new FileStream(inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 2 * blockLength))
            using (FileStream target = new FileStream($"{outputFileName}.gz", FileMode.Create, FileAccess.Write, FileShare.None, 2 * blockLength))
            using (GZipStream compression = new GZipStream(target, CompressionMode.Compress))
            {
                long countOfBlocks = source.Length / blockLength
                    + (source.Length % blockLength == 0 ? 0 : 1);

                Thread readThread = new Thread(() => ReadSource(source, blockLength, countOfBlocks));
                Thread writeThread = new Thread(() => WriteIntoTarget(compression, countOfBlocks));

                readThread.Start();
                writeThread.Start();

                readThread.Join();
                writeThread.Join();
            }

            Console.WriteLine("Compressed...");
        }

        private static ProducerConsumerQueue<byte[]> _queueToCompress = new ProducerConsumerQueue<byte[]>();

        static void ReadSource(FileStream source, int blockLength, long countOfBlocks)
        {
            void read(int length)
            {
                byte[] readBytes = new byte[length];
                source.Read(readBytes, 0, length);
                _queueToCompress.Enqueue(readBytes);
            }

            for (long i = 1; i < countOfBlocks; i++)
                read(blockLength);

            // дочитываем аппендикс
            if (source.Position < source.Length)
                read((int)(source.Length - source.Position));
        }

        static void WriteIntoTarget(GZipStream compression, long countOfBlocks)
        {
            for (long i = 1; i <= countOfBlocks; i++)
            {
#pragma warning disable IDE0018 // Объявление встроенной переменной
                byte[] bytesToWrite;
#pragma warning restore IDE0018 // Объявление встроенной переменной

                while (!_queueToCompress.TryDequeue(out bytesToWrite))
                    continue;
                compression.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
        }

        static void Compress(string inputFileName, string outputFileName)
        {
            using (FileStream source = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            using (FileStream target = new FileStream($"{outputFileName}.gz", FileMode.Create, FileAccess.Write))
            using (GZipStream compression = new GZipStream(target, CompressionMode.Compress))
            {
                byte[] bytes = new byte[source.Length];
                source.Read(bytes, 0, bytes.Length);
                compression.Write(bytes, 0, bytes.Length);
            }
            Console.WriteLine("Compressed...");
        }

        static void Decompress(string inputFileName, string outputFileName)
        {
            using (FileStream source = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            using (FileStream target = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
            using (GZipStream decompression = new GZipStream(source, CompressionMode.Decompress))
            {
                checked
                {
                    int byteOrEndOfStream;
                    while ((byteOrEndOfStream = decompression.ReadByte()) >= 0)
                        target.WriteByte((byte)byteOrEndOfStream);
                }
            }
            Console.WriteLine("Decompressed...");
        }
    }
}