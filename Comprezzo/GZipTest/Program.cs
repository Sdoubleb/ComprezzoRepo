using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using GZipper;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string operation = args[0];
            string inputFileName = args[1];
            string outputFileName = args[2];

            ICompressor compressor = new ProducerConsumerCompressor(inputFileName, outputFileName);

            string beginMessage, endMessage;
            Action action;

            switch (operation.ToLower())
            {
                case "compress":
                    action = compressor.Compress;
                    beginMessage = "Compressing...";
                    endMessage = "Compressed.";
                    break;
                case "decompress":
                    action = compressor.Decompress;
                    beginMessage = "Decompressing...";
                    endMessage = "Decompressed.";
                    break;
                default:
                    throw new ArgumentException($"Неподдерживаемая операция: {operation}.");
            }

            Console.WriteLine(beginMessage);
            action();
            Console.WriteLine(endMessage);

            Console.ReadLine();
        }
    }
}