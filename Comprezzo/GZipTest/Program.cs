using System;
using System.Diagnostics;
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

            ICompressor compressor = new AsyncProducerConsumerCompressor(inputFileName, outputFileName);

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

            Stopwatch sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();

            Console.WriteLine(endMessage);
            Console.WriteLine($"Elapsed time: {sw.Elapsed}.");

            Console.ReadLine();
        }
    }
}