using System;
using System.IO;
using System.IO.Compression;

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