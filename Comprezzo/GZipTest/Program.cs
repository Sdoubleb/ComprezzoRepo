using System;
using System.Diagnostics;
using Sbb.Compression.Compressors;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CompressionParams.Instance = new CompressionParams(args);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Неверно заданы параметры.");
                return;
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);
                return;
            }

            string beginMessage = CompressionParams.Instance.Compress ? "Сжатие..." : "Разжатие...";
            string endMessage = CompressionParams.Instance.Compress ? "Сжато." : "Разжато.";

            Console.WriteLine(beginMessage);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                Work();

                sw.Stop();

                Console.WriteLine(endMessage);
                Console.WriteLine($"Времени затрачено: {sw.Elapsed}.");
            }
            catch (Exception e)
            {          
#if DEBUG
                Debugger.Break();
#endif
                Console.WriteLine("Ошибка: " + e.Message);
            }

            Console.ReadLine();
        }

        private static void Work()
        {
            var creator = new FileCompressorCreator(10 * 1024 * 1024, 10 * 1024 * 1024);
            using (IFileCompressor compressor = creator.Create())
            {
                CompressionParams @params = CompressionParams.Instance;
                if (@params.Compress)
                    compressor.Compress(@params.InputFileName, @params.OutputFileName);
                else
                    compressor.Decompress(@params.InputFileName, @params.OutputFileName);
            }
        }
    }
}